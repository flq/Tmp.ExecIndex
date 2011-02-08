using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;

namespace ExecIndex
{
    public interface IModifyAssembly
    {
        bool HasAccess { get; }
        void AddCallsWithTheseAssemblies(IEnumerable<Assembly> assemblies);
        void RemoveCallsToTheseAssemblies(IEnumerable<Assembly> assemblies);
    }

    public class AssemblyUpdater : IDisposable, IModifyAssembly
    {
        private readonly string _assemblyFile;
        private readonly ModuleDefinition _module;
        private readonly TypeDefinition _type;
        private MethodDefinition _methodDefinition;
        private MethodInfoBasedScanner<CallIn> _scanner;
        private readonly AssemblyResolveUpdater _resolveUpdater;

        private readonly Dictionary<string,AssemblyDefinition> _knownAssemblyDefinitions = new Dictionary<string, AssemblyDefinition>();

        public AssemblyUpdater(string assemblyFile)
        {
            _assemblyFile = assemblyFile;
            _module = ModuleDefinition.ReadModule(assemblyFile);
            _type = _module.GetType("TheIndex", "EntryPoint");
            _resolveUpdater = new AssemblyResolveUpdater(_module);
        }

        public IModifyAssembly For(Expression<Action<CallIn>> methodSelector)
        {
            _methodDefinition = _type.Methods.SingleOrDefault(md => md.IsSignatureEquivalent(methodSelector.GetMethodInfo()));
            _scanner = new MethodInfoBasedScanner<CallIn>(methodSelector);
            return this;
        }

        public void Dispose()
        {
            _module.Write(_assemblyFile);
        }

        bool IModifyAssembly.HasAccess
        {
            get { return _methodDefinition != null; }
        }

        void IModifyAssembly.AddCallsWithTheseAssemblies(IEnumerable<Assembly> assemblies)
        {
            var proc = _methodDefinition.Body.GetILProcessor();

            foreach (var a in assemblies)
                AddAssemblyResolveCall(a);

            foreach (var methodReference in _scanner.Scan(assemblies).Select(MethodReferenceFromMethodInfo))
                InsertCall(proc, methodReference);
        }

        void IModifyAssembly.RemoveCallsToTheseAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var assemblyName in _scanner.Scan(assemblies).Select(mi => mi.DeclaringType.Assembly.FullName).Distinct())
                RemoveCallTo(assemblyName);
        }

        private void RemoveCallTo(string assemblyName)
        {
            var ldargCount = _methodDefinition.Parameters.Count; 
            var calls = (from inst in _methodDefinition.Body.Instructions.Select((ins,index)=> new {ins.Operand,index})
                        let mr = inst.Operand as MethodReference
                        where mr != null && mr.DeclaringType.Scope.ToString() == assemblyName
                        select Enumerable.Range(inst.index - ldargCount, ldargCount + 1).Select(i=>_methodDefinition.Body.Instructions[i]))
                        .SelectMany(s=>s)
                        .ToList();

            var proc = _methodDefinition.Body.GetILProcessor();
            foreach (var c in calls)
                proc.Remove(c);
            
        }

        private void InsertCall(ILProcessor proc, MethodReference mr)
        {
            //by design argument count-equivalence, as many args must be loaded on the stack
            var ldargs = mr.Parameters.Select((_, i) => proc.Create(OpCodes.Ldarg, i+1)).ToList();
            var returnIns = _methodDefinition.Body.Instructions.Last();

            foreach (var ldarg in ldargs)
                proc.InsertBefore(returnIns, ldarg);

            var call = proc.Create(OpCodes.Call, mr);
            proc.InsertBefore(returnIns, call);
        }

        private MethodReference MethodReferenceFromMethodInfo(MethodBase mi)
        {
            AssemblyDefinition ad;
            var assemblyLocation = mi.DeclaringType.Assembly.Location;
            var success = _knownAssemblyDefinitions.TryGetValue(assemblyLocation, out ad);

            if (!success)
            {
                ad = AssemblyDefinition.ReadAssembly(assemblyLocation);
                _knownAssemblyDefinitions.Add(assemblyLocation, ad);
            }

            var mr = ad.MainModule.Import(mi);
            mr = _module.Import(mr);
            return mr;
        }

        private void AddAssemblyResolveCall(Assembly assembly)
        {
            _resolveUpdater.AddResolveCallFor(assembly);
        }
    }
}
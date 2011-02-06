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
        void ReindexWithTheseAssemblies(IEnumerable<Assembly> assemblies);
    }

    public class AssemblyUpdater : IDisposable, IModifyAssembly
    {
        private readonly string _assemblyFile;
        private readonly ModuleDefinition _module;
        private readonly TypeDefinition _type;
        private MethodDefinition _methodDefinition;
        private MethodInfoBasedScanner<CallIn> _scanner;
        private AssemblyResolveUpdater _resolveUpdater;

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

        public void ReindexWithTheseAssemblies(IEnumerable<Assembly> assemblies)
        {
            var proc = _methodDefinition.Body.GetILProcessor();

            foreach (var a in assemblies)
                AddAssemblyResolveCall(a);

            foreach (var methodReference in _scanner.Scan(assemblies).Select(MethodReferenceFromMethodInfo))
                InsertCall(proc, methodReference);
        }

        private void InsertCall(ILProcessor proc, MethodReference mr)
        {
            var i1 = proc.Create(OpCodes.Ldarg_1);
            var i2 = proc.Create(OpCodes.Ldarg_2);
            var i3 = proc.Create(OpCodes.Call, mr);
            var returnIns = _methodDefinition.Body.Instructions.Last();
            proc.InsertBefore(returnIns, i1);
            proc.InsertAfter(i1, i2);
            proc.InsertAfter(i2, i3);
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
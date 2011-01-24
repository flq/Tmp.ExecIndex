using System;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using System.Linq;

namespace ExecIndex
{
    public interface IModifyAssembly
    {
        void AddCallTo(MethodInfo method);
        bool HasAccess { get; }
    }

    public class AssemblyUpdater : IDisposable, IModifyAssembly
    {
        private readonly string _assemblyFile;
        private readonly ModuleDefinition _module;
        private readonly TypeDefinition _type;
        private MethodDefinition _currentMethodDefinition;

        public AssemblyUpdater(string assemblyFile)
        {
            _assemblyFile = assemblyFile;
            _module = ModuleDefinition.ReadModule(assemblyFile);
            _type = _module.GetType("TheIndex", "EntryPoint");
        }

        public IModifyAssembly For(Expression<Action<CallIn>> methodSelector)
        {
            _currentMethodDefinition = _type.Methods.SingleOrDefault(md => md.IsSignatureEquivalent(methodSelector.GetMethodInfo()));
            return this;
        }

        public void Dispose()
        {
            _module.Write(_assemblyFile);
        }

        void IModifyAssembly.AddCallTo(MethodInfo method)
        {
            throw new NotImplementedException();
        }

        bool IModifyAssembly.HasAccess
        {
            get { return _currentMethodDefinition != null; }
        }
    }
}
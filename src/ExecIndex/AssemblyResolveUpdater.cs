using System.Reflection;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;

namespace ExecIndex
{
    internal class AssemblyResolveUpdater
    {

        private readonly ModuleDefinition _module;
        private readonly MethodDefinition _initMethod;
        private readonly MethodDefinition _addResolveMethod;
        private readonly ILProcessor _proc;

        public AssemblyResolveUpdater(ModuleDefinition module)
        {
            _module = module;
            var type = _module.GetType("TheIndex", "Resolver");
            _initMethod = type.Methods.Single(m => m.Name == "DictionaryInitialization");
            _addResolveMethod = type.Methods.Single(m => m.Name == "Add");
            _proc = _initMethod.Body.GetILProcessor();
        }

        private Instruction Last
        {
            get { return _initMethod.Body.Instructions[_initMethod.Body.Instructions.Count - 1]; }
        }

        public void AddResolveCallFor(Assembly asmbly)
        {
            _proc.InsertBefore(Last, _proc.Create(OpCodes.Ldstr, asmbly.FullName));
            _proc.InsertBefore(Last, _proc.Create(OpCodes.Ldstr, asmbly.Location));
            _proc.InsertBefore(Last, _proc.Create(OpCodes.Call, _addResolveMethod));
        }

    }
}
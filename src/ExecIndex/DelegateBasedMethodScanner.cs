using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExecIndex
{
    /// <summary>
    /// T may be any of Func or Action
    /// </summary>
    public class DelegateBasedMethodScanner<T> : IMethodScanner
    {
        private static Type[] _inputParameterTypes;
        private readonly BindingFlags _bindingFlags;

        public DelegateBasedMethodScanner(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
            var t = typeof (T);
            if (!t.Name.StartsWith("Func") && !t.Name.StartsWith("Action"))
                throw new ArgumentException("Unlikely that this will work with type " + t.Name);
        }


        public IEnumerable<MethodInfo> Scan(IEnumerable<Assembly> assemblies)
        {
            return from a in assemblies
                   from t in a.GetTypes()
                   from m in t.GetMethods(_bindingFlags)
                   where MatchesSignature(m)
                   select m;
        }

        private static bool MatchesSignature(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType == typeof(void) && LookingForFunc)
                return false;
            
            var parameterTypes = methodInfo.GetParameters().Select(pi=>pi.ParameterType).ToArray();
            
            if (parameterTypes.Length != NumberOfInputParameters)
                return false;
            
            if (!parameterTypes.SequenceEqual(InputParameterTypes))
                return false;

            if (methodInfo.ReturnType != ReturnType)
                return false;

            return true;

        }

        private static Type ReturnType
        {
            get
            {
                if (IsParameterlessAction || LookingForAction)
                    return typeof(void);
                return Type.GetGenericArguments().Last();
            }
        }

        private static bool LookingForFunc { get { return typeof (T).Name.StartsWith("Func"); } }
        private static bool LookingForAction { get { return typeof(T).Name.StartsWith("Action"); } }
        private static bool IsParameterlessAction { get { return typeof(T) == typeof(Action); } }
        private static Type Type { get { return typeof (T);  } }

        private static Type[] InputParameterTypes
        {
            get
            {
                if (_inputParameterTypes != null)
                    return _inputParameterTypes;
                return _inputParameterTypes = GetParameterTypes();
            }
        }

        private static int NumberOfInputParameters
        {
            get
            {
                return InputParameterTypes.Length;
            }
        }

        private static Type[] GetParameterTypes()
        {
            if (IsParameterlessAction)
                return new Type[0];
            if (LookingForAction)
                return Type.GetGenericArguments();
            if (LookingForFunc)
            {
                var types = Type.GetGenericArguments();
                return types.Take(types.Length - 1).ToArray();
            }
            throw new InvalidOperationException("Kernel Panic");
        }
    }
}

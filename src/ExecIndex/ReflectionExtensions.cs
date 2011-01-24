using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;

namespace ExecIndex
{
    public static class ReflectionExtensions
    {
        public static bool Is<T>(this Type type)
        {
            return typeof (T) == type;
        }

        public static MethodInfo GetMethodInfo<T>(this Expression<Action<T>> methodSelector)
        {
            if (!(methodSelector.Body is MethodCallExpression))
                return null;

            return ((MethodCallExpression)methodSelector.Body).Method;
        }

        public static bool IsSignatureEquivalent(this MethodInfo @this, MethodInfo other)
        {
            return other.ReturnType.Equals(@this.ReturnType) && parametersEqual(@this, other);
        }

        private static bool parametersEqual(MethodInfo @this, MethodInfo other)
        {
            var parms1 = @this.GetParameters();
            var parms2 = other.GetParameters();
            return parms1.Length == parms2.Length &&
                   parms1.Select(pi => pi.ParameterType)
                       .SequenceEqual(parms2.Select(pi => pi.ParameterType));
        }


        public static bool IsSignatureEquivalent(this IMethodSignature @this, MethodInfo other)
        {
            return other.ReturnType.FullName.Equals(@this.MethodReturnType.ReturnType.FullName) && parametersEqual(@this, other);
        }

        private static bool parametersEqual(IMethodSignature @this, MethodInfo other)
        {
            if (@this.Parameters.Count != other.GetParameters().Length)
                return false;

            var pairs = @this.Parameters.Zip(other.GetParameters(),
                                             (pd, pi) => Tuple.Create(pd.ParameterType, pi.ParameterType));
            var truth = pairs.All(pair => detailedEquality(pair.Item1, pair.Item2));

            return truth;
        }

        private static bool detailedEquality(TypeReference cecilParameterType, Type methInfoParameterType)
        {
            if (cecilParameterType == null || methInfoParameterType == null)
                return false;
            
            var isNameEqual = cecilParameterType.Namespace == methInfoParameterType.Namespace && cecilParameterType.Name == methInfoParameterType.Name;
            
            if (!isNameEqual)
              return false;

            var genericsEquivalent = genericArgsEquivalence(cecilParameterType, methInfoParameterType);

            return genericsEquivalent;

        }

        private static bool genericArgsEquivalence(TypeReference cecilType, Type type)
        {
            if (!(cecilType is GenericInstanceType) && !type.IsGenericType)
                return true;
            // Type Reference also has GenericParameters, but they are empty, the call to resolve seems to 
            // bring that particular info to life
            var pairs = ((GenericInstanceType)cecilType).GenericArguments.Zip(type.GetGenericArguments(), Tuple.Create);
            return pairs.All(pair => detailedEquality(pair.Item1, pair.Item2));
        }
    }
}
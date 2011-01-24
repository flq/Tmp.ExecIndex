using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExecIndex
{
    public class MethodInfoBasedScanner<T> : IMethodScanner
    {
        private readonly MethodInfo _methInfo;

        public MethodInfoBasedScanner(Expression<Action<T>> methodSelector)
        {
            _methInfo = methodSelector.GetMethodInfo();
        }

        public IEnumerable<MethodInfo> Scan(IEnumerable<Assembly> assemblies)
        {
            return from a in assemblies
                   from t in a.GetTypes()
                   from m in t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                   where m.IsSignatureEquivalent(_methInfo)
                   select m;
        }
    }
}
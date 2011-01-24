using System.Collections.Generic;
using System.Reflection;

namespace ExecIndex
{
    public interface IMethodScanner
    {
        IEnumerable<MethodInfo> Scan(IEnumerable<Assembly> assemblies);
    }
}
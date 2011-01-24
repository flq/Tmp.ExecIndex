using System.Collections.Generic;

namespace ExecIndex.Tests.CompileArtefacts
{
    public class AnImplementation
    {
        public static void Install(string name, IList<string> list)
        {
            list.Add("Hello Visited with " + name);
        }
    }
}
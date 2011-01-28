using System.Collections.Generic;

namespace ExecIndex.Tests.CompileArtefacts
{
    public class ClassWithPublicAndPrivate
    {
        public static void VoidMethodOneParam(string parameter)
        {
            
        }

        public static void VoidMethodTwoParam(string parameter, int anotherParameter)
        {

        }

        public static void StringAndList(string name, IList<string> items)
        {
            items.Add("Hello from compile artefact " + name);
        }

        private static void TwoParam(string parameter, int anotherParameter)
        {

        }

        private static void OneParam(string parameter)
        {
            
        }
    }
}
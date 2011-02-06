using System;
using System.Collections.Generic;
using ExecIndex;

namespace TheIndex
{
    public class EntryPoint : CallIn
    {
        static EntryPoint()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver.Resolve;
        }

        public void Install(string name, IList<string> dependencies)
        {
            
        }

        public void Uninstall(int sequence, string name, IList<string> dependencies)
        {
            
        }
    }
}

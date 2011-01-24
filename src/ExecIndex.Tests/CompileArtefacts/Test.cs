using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ExecIndex.Tests.CompileArtefacts
{
    public class Test
    {
        private static readonly Dictionary<string, string> assemblyLocations =
            new Dictionary<string, string>
                {
                    {"A", "B"},
                    {"C", "D"}
                };

        static Test()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolve;
        }

        private static Assembly OnResolve(object sender, ResolveEventArgs args)
        {
            string file;
            var success = assemblyLocations.TryGetValue(args.Name, out file);
            if (!success)
                return null;
            var a = Assembly.LoadFrom(file);
            return a;
        }


        public void Go(string name, IList<string> input)
        {
        }
    }
}
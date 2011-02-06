using System;
using System.Collections.Generic;
using System.Reflection;

namespace TheIndex
{
    internal class Resolver
    {
        private static readonly Dictionary<string,string> nameToPath = new Dictionary<string, string>();

        static Resolver()
        {
            DictionaryInitialization();
        }

        public static void DictionaryInitialization()
        {
            Add("bogusAssembly", "a path");
        }

        public static void Add(string assemblyName, string path)
        {
            nameToPath[assemblyName] = path;
        }

        public static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            string path;
            var success = nameToPath.TryGetValue(args.Name, out path);
            return success ? Assembly.LoadFrom(path) : null;
        }
    }
}
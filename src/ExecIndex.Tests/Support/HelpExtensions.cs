using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ExecIndex.Tests.Support
{
    public static class HelpExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            return new[] {item};
        }
    }

    public static class TestExtensions
    {
        public static void HasCount(this IEnumerable items, int expected)
        {
            Assert.AreEqual(expected, items.Cast<object>().Count());
        }

        public static void IsEqualTo<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void IsTrue(this bool actual)
        {
            Assert.IsTrue(actual);
        }
    }
}
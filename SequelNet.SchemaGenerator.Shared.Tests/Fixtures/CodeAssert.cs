using System;
using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Fixtures;

internal static class CodeAssert
{
    public static string NormalizeNewlines(string s) => s.Replace("\r\n", "\n");

    public static void ContainsInOrder(string text, params string[] fragments)
    {
        var index = 0;
        foreach (var fragment in fragments)
        {
            var next = text.IndexOf(fragment, index, StringComparison.Ordinal);
            Assert.True(next >= 0, $"Expected to find fragment in order: {fragment}");
            index = next + fragment.Length;
        }
    }
}
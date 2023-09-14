using System;
using System.Collections.Generic;

namespace SequelNet;

internal static class ObjectExtensions
{
    private static HashSet<Type> NumericTypes = new HashSet<Type>
    {
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(Byte),
        typeof(SByte),
        typeof(UInt16),
        typeof(Int16),
        typeof(UInt32),
        typeof(Int32),
        typeof(UInt64),
        typeof(Int64),
    };

    internal static bool IsOfNumericType(this object value)
    {
        if (value == null) return false;
        return NumericTypes.Contains(value.GetType());
    }
}

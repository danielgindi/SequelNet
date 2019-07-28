// Guids.cs
// MUST match guids.h
using System;

namespace SequelNet_SchemaGenerator_VSIX
{
    static class GuidList
    {
        public const string guidSequelNet_SchemaGenerator_VSIXPkgString = "0a283a43-25bf-454c-b5eb-3571c5c1864d";
        public const string guidSequelNet_SchemaGenerator_VSIXCmdSetString = "14b92cc9-41de-45e1-b4cd-f2ca6c8315cb";

        public static readonly Guid guidSequelNet_SchemaGenerator_VSIXCmdSet = new Guid(guidSequelNet_SchemaGenerator_VSIXCmdSetString);
    };
}
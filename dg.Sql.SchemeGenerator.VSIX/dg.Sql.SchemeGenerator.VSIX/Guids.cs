// Guids.cs
// MUST match guids.h
using System;

namespace dg_Sql_SchemeGenerator_VSIX
{
    static class GuidList
    {
        public const string guiddg_Sql_SchemeGenerator_VSIXPkgString = "0a283a43-25bf-454c-b5eb-3571c5c1864d";
        public const string guiddg_Sql_SchemeGenerator_VSIXCmdSetString = "14b92cc9-41de-45e1-b4cd-f2ca6c8315cb";

        public static readonly Guid guiddg_Sql_SchemeGenerator_VSIXCmdSet = new Guid(guiddg_Sql_SchemeGenerator_VSIXCmdSetString);
    };
}
using System;

namespace SequelNet.Migrations;

public class MigrationState
{
    public Int64 StartVersion;
    public Int64 TargetVersion;
    public Int64 CurrentVersion;

    public bool IsRollingBack = false;
    public bool IsInIntermediateState = false;
}

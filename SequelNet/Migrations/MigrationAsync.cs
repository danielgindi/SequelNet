﻿namespace SequelNet.Migrations;

public abstract class MigrationAsync : IMigration
{
    /// <summary>
    /// What to do when migrating up to this version?
    /// </summary>
    public virtual System.Threading.Tasks.Task UpAsync()
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// What to do when migrating down from this version?
    /// </summary>
    public virtual System.Threading.Tasks.Task DownAsync()
    {
        return System.Threading.Tasks.Task.CompletedTask;
    }
}

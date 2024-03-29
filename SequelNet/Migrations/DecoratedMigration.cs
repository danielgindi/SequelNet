﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SequelNet.Migrations;

public class DecoratedMigration
{
    public Type Type { get; internal set; }
    internal IMigration _Migration;
    public MigrationAttribute Attribute { get; internal set; }

    internal DecoratedMigration(Type migrationType)
    {
        this.Type = migrationType;

        var attrs = migrationType.GetCustomAttributes(typeof(MigrationAttribute), true);
        if (attrs.Length > 0)
        {
            this.Attribute = attrs[0] as MigrationAttribute;
        }
    }

    internal DecoratedMigration(IMigration migration) : this(migration.GetType())
    {
        this._Migration = migration;
    }

    internal IMigration GetMigration(MigrationController.InstanceCreator creator)
    {
        if (_Migration == null)
        {
            if (creator != null)
                _Migration = creator(Type);
            else _Migration = Activator.CreateInstance(Type) as IMigration;
        }

        return _Migration;
    }

    internal string Description
    {
        get
        {
            if (this.Attribute?.Description != null)
                return this.Attribute?.Description;

            return SnakeCase(this.Type.Name);
        }
    }

    private static string SnakeCase(string value)
    {
        var values = new List<string>();
        var matches = Regex.Matches(value, @"[^A-Z._-]+|[A-Z\d]+(?![^._-])|[A-Z\d]+(?=[A-Z])|[A-Z][^A-Z._-]*", RegexOptions.ECMAScript);
        foreach (Match match in matches)
            values.Add(match.Value);

        return string.Join("_", values.Select(x => x.ToLowerInvariant()));
    }
}

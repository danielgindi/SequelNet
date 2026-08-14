# SequelNet Schema Generator for VS Code

Select a SequelNet DAL macro in a C# document and run **SequelNet: Generate or Update DAL from Macro**. The macro remains in the document; generated code is stored immediately after it between `<sequelnet-generated>` markers and is safely replaced on subsequent runs.

Install the generator CLI and ensure `sequelnet-schema-generator` is on `PATH`:

```powershell
dotnet tool install --global SequelNet.SchemaGenerator.Cli
```

Alternatively, set `sequelnet.generatorCommand` to a compatible command.

## Finding a macro

You can select a macro, place the cursor in its /* ... */ or consecutive // comment block, or leave the cursor elsewhere when the file has exactly one valid macro. A valid macro has class and table lines followed by at least one Column: ... line. If multiple macros exist, select one or place the cursor inside it.

## Release package

Run 
pm run package to create _Release/sequelnet-schema-generator-<version>.vsix. The version comes from package.json.

## Bundled generator

The packaged extension includes the SequelNet generator CLI and invokes it through the .NET 8 runtime. Users do not need to install the CLI as a global .NET tool. Set sequelnet.generatorCommand only to override the bundled executable with a custom generator.

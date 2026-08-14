# SequelNet repository guidance

## DAL macros and generated records

A SequelNet DAL macro is a C# block comment that declares an `AbstractRecord` schema. Its following `// <sequelnet-generated>` region is derived output.

- Read [Macro Structure.MD](Macro%20Structure.MD) before changing macro syntax.
- For schema or generated-code changes, edit the macro and invoke `sequelnet-schema-generator`; do not hand-edit generated regions.
- Use the CLI response as structured JSON. Review its warnings before applying the output.
- Only replace a generated region that directly follows the macro, separated solely by whitespace.
- Preserve hand-written partial classes and source outside generated markers.
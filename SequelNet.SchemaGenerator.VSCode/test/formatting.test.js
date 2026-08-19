const assert = require("assert");
const { indentGeneratedRegion } = require("../.tmp/formatting.js");

const generatedCode = [
  "// <sequelnet-generated>",
  "public class Record",
  "{",
  "    public void Save()",
  "    {",
  "        return;",
  "    }",
  "}",
  "// </sequelnet-generated>"
].join("\n");

assert.strictEqual(
  indentGeneratedRegion(generatedCode, "  ", { tabSize: 2, insertSpaces: true }),
  [
    "  // <sequelnet-generated>",
    "  public class Record",
    "  {",
    "    public void Save()",
    "    {",
    "      return;",
    "    }",
    "  }",
    "  // </sequelnet-generated>"
  ].join("\n"));

assert.strictEqual(
  indentGeneratedRegion(generatedCode, "\t", { tabSize: 4, insertSpaces: false }),
  [
    "\t// <sequelnet-generated>",
    "\tpublic class Record",
    "\t{",
    "\t\tpublic void Save()",
    "\t\t{",
    "\t\t\treturn;",
    "\t\t}",
    "\t}",
    "\t// </sequelnet-generated>"
  ].join("\n"));
console.log("generated-region indentation: passed");

const assert = require("assert");
const { findMacro, normalizeMacro } = require("../.tmp/macro.js");

const lineComment = "// Customer\n// customers\n// Id: PRIMARY KEY; INT64; Identifier\n// @Index: NAME(IX_Customers_Id); [Id ASC]";
const blockComment = "/*\n * Customer\n * customers\n * Id: PRIMARY KEY; INT64; Identifier\n */";

assert.equal(normalizeMacro(lineComment), "Customer\ncustomers\nId: PRIMARY KEY; INT64; Identifier\n@Index: NAME(IX_Customers_Id); [Id ASC]");
assert.equal(findMacro(`namespace Test;\n${lineComment}`, 0).macro, normalizeMacro(lineComment));
assert.equal(findMacro(`namespace Test;\n${blockComment}`, 0).macro, "Customer\ncustomers\nId: PRIMARY KEY; INT64; Identifier");
assert.equal(findMacro("// Not a macro\n// It has only two lines", 0), undefined);

console.log("macro comment detection: passed");
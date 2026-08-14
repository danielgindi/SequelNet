const assert = require("assert");
const { buildStartMarker, createOrUpdateAfterMacro, startMarker } = require("../.tmp/generatedRegion.js");
const { findMacro } = require("../.tmp/macro.js");

const macro = "/* macro */";
const duplicated = `${macro}\n// <sequelnet-generated>\nfirst\n// <sequelnet-generated>\nsecond\n// </sequelnet-generated>\nthird\n// </sequelnet-generated>\ncustom`;
const change = createOrUpdateAfterMacro(duplicated, macro.length, "new");
const updated = duplicated.substring(0, change.start) + change.text + duplicated.substring(change.end);

assert.equal(updated, `${macro}\n// <sequelnet-generated>\nnew\n// </sequelnet-generated>\ncustom`);
assert.throws(() => createOrUpdateAfterMacro(`${macro}\n// <sequelnet-generated>\nold`, macro.length, "new"), /missing its end marker/);

console.log("generated region ownership: passed");
const lineMacro = "// Customer\n// customers\n// Id: PRIMARY KEY; INT;";
const lineDocument = `${lineMacro}\n${startMarker}\nold\n// </sequelnet-generated>`;
const locatedMacro = findMacro(lineDocument, 0);
const lineChange = createOrUpdateAfterMacro(lineDocument, locatedMacro.end, "new");
const lineUpdated = lineDocument.substring(0, lineChange.start) + lineChange.text + lineDocument.substring(lineChange.end);
assert.equal((lineUpdated.match(/<sequelnet-generated>/g) || []).length, 1);
assert.equal(lineUpdated, `${lineMacro}\n${startMarker}\nnew\n// </sequelnet-generated>`);

console.log("line-comment macro region replacement: passed");
const documentedLineDocument = `${lineMacro}\n/// Customer record documentation\n/* Keep this note. */\n${startMarker}\nold\n// </sequelnet-generated>`;
const documentedLineMacro = findMacro(documentedLineDocument, 0);
assert.equal(documentedLineMacro.end, lineMacro.length);
const documentedLineChange = createOrUpdateAfterMacro(documentedLineDocument, documentedLineMacro.end, "new");
const documentedLineUpdated = documentedLineDocument.substring(0, documentedLineChange.start) + documentedLineChange.text + documentedLineDocument.substring(documentedLineChange.end);
assert.equal(documentedLineUpdated, `${lineMacro}\n/// Customer record documentation\n/* Keep this note. */\n${startMarker}\nnew\n// </sequelnet-generated>`);

console.log("documentation between macro and region: passed");
const namedStartMarker = buildStartMarker("Customer");
const legacyChange = createOrUpdateAfterMacro(lineDocument, locatedMacro.end, "new", "Customer");
const legacyUpdated = lineDocument.substring(0, legacyChange.start) + legacyChange.text + lineDocument.substring(legacyChange.end);
assert.equal(legacyUpdated, `${lineMacro}\n${namedStartMarker}\nnew\n// </sequelnet-generated>`);

const namedElsewhere = `${namedStartMarker}\nold\n// </sequelnet-generated>\n${lineMacro}`;
const namedChange = createOrUpdateAfterMacro(namedElsewhere, namedElsewhere.length, "new", "Customer");
const namedUpdated = namedElsewhere.substring(0, namedChange.start) + namedChange.text + namedElsewhere.substring(namedChange.end);
assert.equal(namedUpdated, `${namedStartMarker}\nnew\n// </sequelnet-generated>\n${lineMacro}`);

console.log("named region lookup and legacy migration: passed");
const namedLineDocument = `${lineMacro}\n${namedStartMarker}\nold\n// </sequelnet-generated>`;
assert.equal(findMacro(namedLineDocument, 0).end, lineMacro.length);
console.log("named marker terminates line-comment macro: passed");
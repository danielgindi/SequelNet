export const startMarkerPrefix = "// <sequelnet-generated";
export const startMarker = `${startMarkerPrefix}>`;
export const endMarker = "// </sequelnet-generated>";

export interface GeneratedRegionChange {
  start: number;
  end: number;
  text: string;
}

export function buildStartMarker(recordName?: string): string {
  return recordName?.trim() ? `${startMarkerPrefix} record="${recordName.trim()}">` : startMarker;
}

export function createOrUpdateAfterMacro(documentText: string, macroEndOffset: number, generatedCode: string, recordName?: string): GeneratedRegionChange {
  const newLine = documentText.includes("\r\n") ? "\r\n" : "\n";
  const namedStartMarker = recordName?.trim() ? buildStartMarker(recordName) : undefined;
  let regionStart = namedStartMarker ? findUniqueNamedRegionStart(documentText, namedStartMarker) : -1;
  if (regionStart < 0) regionStart = findAdjacentRegionStart(documentText, macroEndOffset);

  if (regionStart < 0) return { start: macroEndOffset, end: macroEndOffset, text: newLine + render(generatedCode, newLine, buildStartMarker(recordName)) };
  return { start: regionStart, end: findMatchingRegionEnd(documentText, regionStart), text: render(generatedCode, newLine, buildStartMarker(recordName)) };
}

function findUniqueNamedRegionStart(documentText: string, namedStartMarker: string): number {
  const firstMatch = documentText.indexOf(namedStartMarker);
  if (firstMatch < 0) return -1;
  if (documentText.indexOf(namedStartMarker, firstMatch + namedStartMarker.length) >= 0) {
    throw new Error("Multiple SequelNet generated regions match this record name.");
  }
  return firstMatch;
}

function findAdjacentRegionStart(documentText: string, macroEndOffset: number): number {
  let position = macroEndOffset;
  while (true) {
    while (position < documentText.length && /\s/.test(documentText[position])) position++;
    if (findNextStartMarker(documentText, position) === position) return position;

    const documentationEnd = skipDocumentationComment(documentText, position);
    if (documentationEnd >= 0) {
      position = documentationEnd;
      continue;
    }

    const blockCommentEnd = skipNonMacroBlockComment(documentText, position);
    if (blockCommentEnd >= 0) {
      position = blockCommentEnd;
      continue;
    }

    return -1;
  }
}

function findNextStartMarker(documentText: string, searchStart: number): number {
  let position = searchStart;
  while (true) {
    position = documentText.indexOf(startMarkerPrefix, position);
    if (position < 0) return -1;
    const suffix = documentText[position + startMarkerPrefix.length];
    if (suffix === ">" || /\s/.test(suffix ?? "")) return position;
    position += startMarkerPrefix.length;
  }
}

function skipDocumentationComment(documentText: string, position: number): number {
  if (!documentText.startsWith("///", position)) return -1;
  const lineEnd = documentText.slice(position).search(/\r\n|\r|\n/);
  return lineEnd < 0 ? documentText.length : position + lineEnd;
}

function skipNonMacroBlockComment(documentText: string, position: number): number {
  if (!documentText.startsWith("/*", position)) return -1;
  const commentEnd = documentText.indexOf("*/", position + 2);
  if (commentEnd < 0 || isMacroShapedComment(documentText.slice(position + 2, commentEnd))) return -1;
  return commentEnd + 2;
}

function isMacroShapedComment(content: string): boolean {
  const lines = content.replace(/\r\n|\r/g, "\n").split("\n")
    .map(line => line.replace(/^\s*\*?\s?/, "").trim())
    .filter(Boolean);
  return lines.length >= 3 && lines.slice(2).some(line => !line.startsWith("@") && line.indexOf(":") > 0);
}

function findMatchingRegionEnd(documentText: string, regionStart: number): number {
  let depth = 1;
  let searchStart = regionStart + startMarkerPrefix.length;

  while (true) {
    const nextStart = findNextStartMarker(documentText, searchStart);
    const nextEnd = documentText.indexOf(endMarker, searchStart);
    if (nextEnd < 0) throw new Error("The SequelNet generated region is missing its end marker.");

    if (nextStart >= 0 && nextStart < nextEnd) {
      depth++;
      searchStart = nextStart + startMarkerPrefix.length;
      continue;
    }

    depth--;
    searchStart = nextEnd + endMarker.length;
    if (depth === 0) return searchStart;
  }
}

function render(generatedCode: string, newLine: string, regionStartMarker: string): string {
  const normalizedCode = generatedCode.replace(/\r\n|\r/g, "\n").replace(/\n/g, newLine).replace(/[\r\n]+$/, "");
  return `${regionStartMarker}${newLine}${normalizedCode}${newLine}${endMarker}`;
}
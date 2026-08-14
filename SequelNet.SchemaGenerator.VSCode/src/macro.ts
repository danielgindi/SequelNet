export interface MacroRange {
  start: number;
  end: number;
  macro: string;
  recordName: string;
}

interface CommentRange {
  start: number;
  end: number;
  text: string;
  kind: "line" | "block";
}

/** Normalizes a raw SequelNet macro or either supported C# comment form. */
export function normalizeMacro(rawText: string): string {
  const trimmed = rawText.trim();
  if (trimmed.startsWith("/*") && trimmed.endsWith("*/")) {
    return normalizeBlockComment(trimmed.substring(2, trimmed.length - 2));
  }

  const lines = rawText.replace(/\r\n|\r/g, "\n").split("\n");
  const nonEmptyLines = lines.filter(line => line.trim().length > 0);
  if (nonEmptyLines.length > 0 && nonEmptyLines.every(line => /^\s*\/\//.test(line))) {
    return lines.map(line => line.replace(/^\s*\/\/?\s?/, "")).join("\n").trim();
  }

  return trimmed;
}

/**
 * Finds a valid-looking macro comment. A cursor-contained match wins; otherwise
 * a sole macro comment in the file is used. Ambiguous files require selection.
 */
export function findMacro(documentText: string, cursorOffset: number): MacroRange | undefined {
  const candidates = findComments(documentText)
    .map(comment => ({ ...comment, macro: comment.kind === "block"
      ? normalizeBlockComment(comment.text.substring(2, comment.text.length - 2))
      : normalizeMacro(comment.text) }))
    .filter(candidate => isMacroShape(candidate.macro));

  const atCursor = candidates.filter(candidate => cursorOffset >= candidate.start && cursorOffset <= candidate.end);
  const chosen = atCursor.length === 1 ? atCursor[0] : candidates.length === 1 ? candidates[0] : undefined;
  return chosen && { start: chosen.start, end: chosen.end, macro: chosen.macro, recordName: getRecordName(chosen.macro) };
}

export function getRecordName(macro: string): string {
  return macro.split(/\r\n|\r|\n/).map(line => line.trim()).find(Boolean) ?? "";
}
function findComments(documentText: string): CommentRange[] {
  const comments: CommentRange[] = [];
  const blockPattern = /\/\*[\s\S]*?\*\//g;
  for (let match = blockPattern.exec(documentText); match; match = blockPattern.exec(documentText)) {
    comments.push({ start: match.index, end: match.index + match[0].length, text: match[0], kind: "block" });
  }

  const lines = documentText.matchAll(/.*(?:\r\n|\n|\r|$)/g);
  let groupStart = -1;
  let groupEnd = -1;
  let groupText = "";
  for (const match of lines) {
    if (match[0].length === 0) continue;
    const lineText = match[0];
    if (isGeneratedRegionMarker(lineText) || isDocumentationComment(lineText)) {
      if (groupStart >= 0) {
        comments.push({ start: groupStart, end: groupEnd, text: groupText, kind: "line" });
        groupStart = -1;
        groupText = "";
      }
    } else if (/^\s*\/\//.test(lineText)) {
      if (groupStart < 0) groupStart = match.index;
      groupEnd = match.index + lineText.replace(/\r\n|\n|\r$/, "").length;
      groupText += lineText;
    } else if (groupStart >= 0) {
      comments.push({ start: groupStart, end: groupEnd, text: groupText, kind: "line" });
      groupStart = -1;
      groupText = "";
    }  }
  if (groupStart >= 0) comments.push({ start: groupStart, end: groupEnd, text: groupText, kind: "line" });

  return comments;
}

function isGeneratedRegionMarker(line: string): boolean {
  const trimmed = line.trim();
  return trimmed === "// </sequelnet-generated>" ||
    /^\/\/ <sequelnet-generated(?:\s+record="[^"]+")?>$/.test(trimmed);
}

function isDocumentationComment(line: string): boolean {
  return line.trimStart().startsWith("///");
}
function normalizeBlockComment(content: string): string {
  return content
    .replace(/\r\n|\r/g, "\n")
    .split("\n")
    .map(line => line.replace(/^\s*\*?\s?/, ""))
    .join("\n")
    .trim();
}

function isMacroShape(macro: string): boolean {
  const lines = macro.split(/\r\n|\r|\n/).map(line => line.trim()).filter(Boolean);
  return lines.length >= 3 && lines[0].length > 0 && lines[1].length > 0 && lines.slice(2).some(line => /^[^@:\s][^:]*:/.test(line));
}
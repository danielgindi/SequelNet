export interface IndentationOptions {
  tabSize?: number | string;
  insertSpaces?: boolean | string;
}

export function indentGeneratedRegion(text: string, baseIndentation: string, options: IndentationOptions): string {
  const indentUnit = resolveIndentUnit(options);
  return text.replace(/^([ ]*)(?=\S)/gm, (_, indentation: string) => {
    const levels = Math.floor(indentation.length / 4);
    const remainder = indentation.slice(levels * 4);
    return baseIndentation + indentUnit.repeat(levels) + remainder;
  });
}

function resolveIndentUnit(options: IndentationOptions): string {
  if (options.insertSpaces === false) return "\t";

  const tabSize = typeof options.tabSize === "number" && options.tabSize > 0
    ? Math.floor(options.tabSize)
    : 4;
  return " ".repeat(tabSize);
}

import { spawn } from "child_process";
import { existsSync } from "fs";
import { join } from "path";
import * as vscode from "vscode";
import { indentGeneratedRegion } from "./formatting";
import { createOrUpdateAfterMacro } from "./generatedRegion";
import { findMacro, normalizeMacro } from "./macro";

interface GenerateResponse { success: boolean; code?: string; recordName?: string; warnings: string[]; errors: string[]; }
interface GeneratorInvocation { command: string; args: string[]; }

export function activate(context: vscode.ExtensionContext): void {
  context.subscriptions.push(vscode.commands.registerCommand("sequelnet.generateOrUpdateDal", () => generateOrUpdate(context)));
}

async function generateOrUpdate(context: vscode.ExtensionContext): Promise<void> {
  const editor = vscode.window.activeTextEditor;
  if (!editor) return;

  const selectedMacro = !editor.selection.isEmpty
    ? (() => { const macro = normalizeMacro(editor.document.getText(editor.selection)); return { start: editor.document.offsetAt(editor.selection.start), end: editor.document.offsetAt(editor.selection.end), macro }; })()
    : findMacro(editor.document.getText(), editor.document.offsetAt(editor.selection.active));
  if (!selectedMacro) return void vscode.window.showInformationMessage("Select a SequelNet macro, place the cursor inside one, or keep exactly one macro comment in the file.");
  if (!selectedMacro.macro) return void vscode.window.showInformationMessage("The SequelNet macro is empty.");

  let response: GenerateResponse;
  try { response = await invokeGenerator(context, selectedMacro.macro); }
  catch (error) { return void vscode.window.showErrorMessage(`SequelNet generation failed: ${errorMessage(error)}`); }

  if (!response.success || !response.code) return void vscode.window.showErrorMessage(`SequelNet macro is invalid: ${response.errors.join("; ") || "Unknown error"}`);

  const document = editor.document;
  const change = createOrUpdateAfterMacro(document.getText(), selectedMacro.end, response.code, response.recordName);
  const changeText = indentGeneratedRegion(
    change.text,
    leadingWhitespace(document.lineAt(document.positionAt(change.start)).text),
    editor.options);
  const edit = new vscode.WorkspaceEdit();
  edit.replace(document.uri, new vscode.Range(document.positionAt(change.start), document.positionAt(change.end)), changeText);
  if (!await vscode.workspace.applyEdit(edit)) return void vscode.window.showErrorMessage("SequelNet generated code could not be written to the editor.");

  await formatGeneratedRegion(editor, change.start, changeText.length);

  if (response.warnings.length > 0) void vscode.window.showWarningMessage(`SequelNet generated code with warnings: ${response.warnings.join("; ")}`);
}

async function formatGeneratedRegion(editor: vscode.TextEditor, start: number, length: number): Promise<void> {
  try {
    editor.selection = new vscode.Selection(
      editor.document.positionAt(start),
      editor.document.positionAt(start + length));
    await vscode.commands.executeCommand("editor.action.formatSelection");
  } catch {
    // Generation succeeds when the active editor has no selection formatter.
  }
}

function leadingWhitespace(text: string): string {
  return text.match(/^\s*/)?.[0] ?? "";
}

function invokeGenerator(context: vscode.ExtensionContext, script: string): Promise<GenerateResponse> {
  const invocation = resolveGenerator(context);

  return new Promise((resolve, reject) => {
    const child = spawn(invocation.command, invocation.args, { windowsHide: true, stdio: ["pipe", "pipe", "pipe"] });
    let stdout = "";
    let stderr = "";

    child.stdout.setEncoding("utf8");
    child.stderr.setEncoding("utf8");
    child.stdout.on("data", (chunk: string) => stdout += chunk);
    child.stderr.on("data", (chunk: string) => stderr += chunk);
    child.on("error", reject);
    child.on("close", () => {
      try {
        resolve(JSON.parse(stdout) as GenerateResponse);
      } catch {
        reject(new Error(stderr.trim() || "The generator returned an invalid response."));
      }
    });
    child.stdin.end(JSON.stringify({ script }));
  });
}

function resolveGenerator(context: vscode.ExtensionContext): GeneratorInvocation {
  const configuredCommand = vscode.workspace.getConfiguration("sequelnet").get<string>("generatorCommand", "").trim();
  if (configuredCommand) return { command: configuredCommand, args: [] };

  const bundledCli = context.asAbsolutePath(join("bin", "cli", "sequelnet-schema-generator.dll"));
  if (existsSync(bundledCli)) return { command: "dotnet", args: [bundledCli] };

  return { command: "sequelnet-schema-generator", args: [] };
}

function errorMessage(error: unknown): string { return error instanceof Error ? error.message : String(error); }
export function deactivate(): void { }
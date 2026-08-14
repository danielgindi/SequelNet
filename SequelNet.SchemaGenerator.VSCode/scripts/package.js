const { execFileSync } = require("child_process");
const fs = require("fs");
const path = require("path");

const extensionDirectory = path.resolve(__dirname, "..");
const repositoryDirectory = path.resolve(extensionDirectory, "..");
const packageJson = require(path.join(extensionDirectory, "package.json"));
const cliProject = path.join(repositoryDirectory, "SequelNet.SchemaGenerator.Cli", "SequelNet.SchemaGenerator.Cli.csproj");
const bundledCliDirectory = path.join(extensionDirectory, "bin", "cli");
const bundledCli = path.join(bundledCliDirectory, "sequelnet-schema-generator.dll");
const releaseDirectory = path.join(repositoryDirectory, "_Release");
const outputPath = path.join(releaseDirectory, `sequelnet-schema-generator-${packageJson.version}.vsix`);
const vsce = require.resolve("@vscode/vsce/vsce", { paths: [extensionDirectory] });

console.log("[STEP 1/2] Publishing the bundled SequelNet generator CLI");
fs.rmSync(bundledCliDirectory, { recursive: true, force: true });
execFileSync("dotnet", ["publish", cliProject, "--configuration", "Release", "--output", bundledCliDirectory, "--nologo", "-p:SignAssembly=false"], {
  cwd: repositoryDirectory,
  stdio: "inherit"
});
if (!fs.existsSync(bundledCli)) throw new Error(`Bundled CLI was not produced: ${bundledCli}`);
console.log("[OK] Bundled CLI ready");

console.log("[STEP 2/2] Packaging the VS Code extension");
fs.mkdirSync(releaseDirectory, { recursive: true });
execFileSync(process.execPath, [vsce, "package", "--out", outputPath], {
  cwd: extensionDirectory,
  stdio: "inherit"
});
console.log(`[OK] Created ${outputPath}`);
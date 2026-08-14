using System.Text.Json;

namespace SequelNet.SchemaGenerator.Cli;

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 1 && (args[0] == "--help" || args[0] == "-h"))
        {
            await Console.Out.WriteLineAsync("Reads a SequelNet macro from standard input and writes a JSON generation response.");
            return 0;
        }

        if (args.Length > 0)
        {
            await Console.Error.WriteLineAsync("Unsupported arguments. Use --help for usage information.");
            return 2;
        }

        var input = await Console.In.ReadToEndAsync();
        var request = ReadRequest(input);
        var response = GeneratorCommand.Generate(request);

        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response, JsonOptions));
        return response.Success ? 0 : 1;
    }

    private static GenerateRequest ReadRequest(string input)
    {
        if (input.TrimStart().StartsWith("{", StringComparison.Ordinal))
        {
            try
            {
                return JsonSerializer.Deserialize<GenerateRequest>(input, JsonOptions) ?? new GenerateRequest();
            }
            catch (JsonException)
            {
                return new GenerateRequest { Script = input };
            }
        }

        return new GenerateRequest { Script = input };
    }
}

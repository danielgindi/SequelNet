using SequelNet.SchemaGenerator;

namespace SequelNet.SchemaGenerator.Cli;

public static class GeneratorCommand
{
    public static GenerateResponse Generate(GenerateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Script))
            return GenerateResponse.Invalid("The request must include a non-empty 'script' value.");

        try
        {
            var result = GeneratorCore.GenerateDalClass(request.Script);
            return new GenerateResponse
            {
                Success = true,
                Code = result.Code,
                RecordName = result.Context.ClassName,
                Warnings = result.Warnings.ToArray(),
            };
        }
        catch (Exception exception)
        {
            return GenerateResponse.Invalid(exception.Message);
        }
    }
}

public sealed class GenerateRequest
{
    public string? Script { get; init; }
}

public sealed class GenerateResponse
{
    public bool Success { get; init; }
    public string? Code { get; init; }
    public string? RecordName { get; init; }
    public string[] Warnings { get; init; } = Array.Empty<string>();
    public string[] Errors { get; init; } = Array.Empty<string>();

    public static GenerateResponse Invalid(string error) => new()
    {
        Success = false,
        Errors = new[] { error },
    };
}

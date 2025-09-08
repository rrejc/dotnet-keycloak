namespace ApiClient.Configuration;

public class AuthenticationConfig
{
    public const string SectionName = "Authentication";

    public required string AuthUrl { get; init; }
    public required string TokenUrl { get; init; }
    public required string Audience { get; init; }
    public required string MetadataAddress { get; init; }
    public required string ValidIssuer { get; init; }
}
namespace ResolveDeskAI.Api.Configuration
{
    public sealed class AiOptions
    {
        public const string SectionName = "AI";

        public string Provider { get; set; } = "MockAI";

        public string OpenAIModel { get; set; } = "gpt-4o-mini";

        public string? OpenAIApiKey { get; set; }
    }
}
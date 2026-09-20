using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Services.AI
{
    public sealed class MockTicketAnalyzer : ITicketAnalyzer
    {
        public Task<TicketAnalysisResult> AnalyzeAsync(
            TicketAnalysisInput input,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var title = input.Title.Trim();
            var description = input.Description.Trim();
            var searchableText =
                $"{title} {description}".ToLowerInvariant();

            var category = DetermineCategory(searchableText);
            var priority = DeterminePriority(searchableText);

            var missingInformation = DetermineMissingInformation(
                searchableText,
                description);

            var result = new TicketAnalysisResult(
                Summary: Truncate(
                    $"{title}: {description}",
                    1000),
                SuggestedCategory: category,
                SuggestedPriority: priority,
                SuggestedResponse: BuildSuggestedResponse(category),
                MissingInformation: missingInformation,
                Provider: "MockAI",
                Model: "deterministic-v1");

            return Task.FromResult(result);
        }

        private static TicketCategory DetermineCategory(
            string searchableText)
        {
            if (ContainsAny(
                searchableText,
                "login",
                "password",
                "authentication",
                "access",
                "account"))
            {
                return TicketCategory.AccountAccess;
            }

            if (ContainsAny(
                searchableText,
                "invoice",
                "billing",
                "payment",
                "charge",
                "refund"))
            {
                return TicketCategory.Billing;
            }

            if (ContainsAny(
                searchableText,
                "feature",
                "enhancement",
                "new functionality"))
            {
                return TicketCategory.FeatureRequest;
            }

            if (ContainsAny(
                searchableText,
                "error",
                "failure",
                "timeout",
                "crash",
                "portal",
                "application"))
            {
                return TicketCategory.Technical;
            }

            return TicketCategory.Other;
        }

        private static TicketPriority DeterminePriority(
            string searchableText)
        {
            if (ContainsAny(
                searchableText,
                "outage",
                "unavailable for all",
                "data loss",
                "security breach"))
            {
                return TicketPriority.Critical;
            }

            if (ContainsAny(
                searchableText,
                "cannot",
                "unable",
                "error",
                "failure",
                "timeout"))
            {
                return TicketPriority.High;
            }

            if (ContainsAny(
                searchableText,
                "information",
                "question",
                "how to"))
            {
                return TicketPriority.Low;
            }

            return TicketPriority.Medium;
        }

        private static IReadOnlyList<string>
            DetermineMissingInformation(
                string searchableText,
                string description)
        {
            var missingInformation = new List<string>();

            if (!ContainsAny(
                searchableText,
                "error code",
                "error message"))
            {
                missingInformation.Add(
                    "Exact error message or error code");
            }

            if (!ContainsAny(
                searchableText,
                "steps to reproduce",
                "reproduction steps"))
            {
                missingInformation.Add(
                    "Steps required to reproduce the issue");
            }

            if (description.Length < 80)
            {
                missingInformation.Add(
                    "More detailed description of the issue");
            }

            return missingInformation;
        }

        private static string BuildSuggestedResponse(
            TicketCategory category)
        {
            return category switch
            {
                TicketCategory.AccountAccess =>
                    "Thank you for the report. We are reviewing "
                    + "the account access issue. Please provide any "
                    + "error message shown during sign-in.",

                TicketCategory.Billing =>
                    "Thank you for contacting support. We are "
                    + "reviewing the billing information and will "
                    + "provide an update shortly.",

                TicketCategory.FeatureRequest =>
                    "Thank you for the suggestion. We have recorded "
                    + "the requested functionality for evaluation.",

                TicketCategory.Technical =>
                    "Thank you for the report. We are investigating "
                    + "the technical issue. Please provide the steps "
                    + "needed to reproduce it.",

                _ =>
                    "Thank you for contacting support. We are "
                    + "reviewing your request and will respond shortly."
            };
        }

        private static bool ContainsAny(
            string source,
            params string[] values)
        {
            return values.Any(source.Contains);
        }

        private static string Truncate(
            string value,
            int maximumLength)
        {
            return value.Length <= maximumLength
                ? value
                : value[..maximumLength];
        }
    }
}
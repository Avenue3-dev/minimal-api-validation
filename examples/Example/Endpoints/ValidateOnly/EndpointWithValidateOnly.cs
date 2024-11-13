namespace Example.Endpoints.ValidateOnly;

using A3.MinimalApiValidation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public class EndpointWithValidateOnly : IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapPost("with-validate-only", (
            [FromQuery] string requiredString,
            [FromHeader(Name = "x-required-header")] string requiredHeader,
            [FromBody] TestRecord body
        ) => Results.StatusCode(500))
        .WithValidateOnly();
    }

    public record TestRecord(string Name, int Age);

    public class TestRecordValidator : AbstractValidator<TestRecord>
    {
        public TestRecordValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Age).InclusiveBetween(1, 100);
        }
    }
}

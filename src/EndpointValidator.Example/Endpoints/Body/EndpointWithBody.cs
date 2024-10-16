namespace EndpointValidator.Example.Endpoints.Body;

using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public class EndpointWithBody : IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapPost("with-body", ([FromBody] TestRecord body) => TypedResults.Ok());
    }
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

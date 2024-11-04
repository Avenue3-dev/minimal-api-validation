namespace Example.Endpoints.QueryList;

using A3.MinimalApiValidation.Binders;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public class EndpointWithQueryList : IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapGet("with-query-list", (
            FromQuery<IdList> ids
        ) => TypedResults.Ok(ids.Value));
    }
}

public record IdList(
    [property: FromQuery(Name = "id")]int[] Ids
);

public class IdListValidator : AbstractValidator<IdList>
{
    public IdListValidator()
    {
        RuleForEach(x => x.Ids)
            .GreaterThan(0)
            .WithMessage("Each provided id must be greater than 0");
    }
}

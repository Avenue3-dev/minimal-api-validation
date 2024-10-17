namespace Example.Endpoints.Query;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class EndpointWithQueryString : IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapGet("with-query-string", (
            [FromQuery] string requiredString,
            [FromQuery] string? optionalString,
            [FromQuery, Range(1, 5)] int requiredRangedInt,
            [FromQuery, Range(6, 9)] int? optionalRangedInt
        ) => TypedResults.Ok(new
        {
            requiredString,
            optionalString,
            requiredRangedInt,
            optionalRangedInt,
        }));
    }
}

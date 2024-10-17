namespace Example.Endpoints.Headers;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class EndpointWithHeaders : IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapGet("with-headers", (
            [FromHeader(Name = "x-required")] string header1,
            [FromHeader(Name = "x-optional")] string? header2,
            [FromHeader(Name = "x-required-int-with-range"), Range(1, 5)] int header3,
            [FromHeader(Name = "x-optional-int-with-range"), Range(6, 9)] int? header4
        ) => TypedResults.Ok(new
        {
            header1,
            header2,
            header3,
            header4,
        }));
    }
}

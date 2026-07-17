namespace Example.Endpoints.QueryCollection;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class EndpointWithQueryCollection : IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        // Repeated query keys (?tag=a&tag=b) bind to the collection and are validated per element.
        // Validation attributes apply to the collection itself, so [MaxLength] limits the item count.
        app.MapGet("with-query-collection", (
            [FromQuery] string[]? tag,
            [FromQuery, MaxLength(3)] int[]? id
        ) => TypedResults.Ok(new
        {
            tag,
            id,
        }));
    }
}

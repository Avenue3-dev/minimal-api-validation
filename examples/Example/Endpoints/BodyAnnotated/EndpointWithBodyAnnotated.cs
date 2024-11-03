namespace Example.Endpoints.BodyAnnotated;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

public class EndpointWithBodyAnnotated : IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapPost("with-body-annotated", ([FromBody] TestRecordAnnotated body) => TypedResults.Ok(body));
    }
}

public class TestRecordAnnotated
{
    [Required]
    [MinLength(1)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    [Required]
    [Range(1, 100)]
    [JsonPropertyName("age")]
    public int Age { get; set; }
}

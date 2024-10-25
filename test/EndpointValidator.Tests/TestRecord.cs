namespace EndpointValidator.Tests;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;

public record TestRecord(string Name, int Age);

public class TestRecordValidator : AbstractValidator<TestRecord>
{
    public TestRecordValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).InclusiveBetween(1, 100);
    }
}

public record TestRecordAnnotated
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

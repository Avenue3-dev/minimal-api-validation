namespace A3.MinimalApiValidation.Tests._utils;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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

public record TestRecordRenamed(
    [property: FromQuery(Name = "has-cake")] bool HasCake,
    [property: FromQuery(Name = "eaten")]bool HasEatenIt
);

public class TestRecordRenamedValidator : AbstractValidator<TestRecordRenamed>
{
    public TestRecordRenamedValidator()
    {
        RuleFor(x => x)
            .Must(x => x is not {HasCake: true, HasEatenIt: true})
            .WithMessage("You can't have your cake and eat it too!");
    }
}

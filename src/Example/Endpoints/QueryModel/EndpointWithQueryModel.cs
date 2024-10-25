namespace Example.Endpoints.QueryModel;

using A3.MinimalApiValidation.Binders;
using FluentValidation;

public class EndpointWithQueryModel: IEndpoint
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapGet("with-query-model", (
            FromQuery<MyQueryModel> data
        ) => TypedResults.Ok(data));
    }
}

public record MyQueryModel(string Name, int Age)
{
    public string Name { get; } = Name;

    public int Age { get; } = Age;
}

public class MyQueryModelValidator : AbstractValidator<MyQueryModel>
{
    public MyQueryModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).InclusiveBetween(1, 150);
    }
}

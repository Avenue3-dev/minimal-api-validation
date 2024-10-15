var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/test", () =>
    {
        return TypedResults.Ok(new
        {
            status = "test ok",
        });
    });

app.Run();

# CLAUDE.md

Guidance for working in this repo. Keep changes consistent with the conventions below.

## What this is

`Avenue3.MinimalApiValidation` (`A3.MinimalApiValidation`) — middleware + binders that automatically
validate request **headers**, **query parameters**, and the **request body** in ASP.NET Core minimal
API endpoints, using FluentValidation and `ValidationAttribute`s. Targets `net10.0`.

## Build & test

```bash
dotnet build MinimalApiValidation.sln
dotnet test                                              # full suite
dotnet test --filter "FullyQualifiedName~<Namespace>"    # subset
```

CI (`.github/workflows/pr-workflow.yml`) builds and tests in `Release` and collects coverage.

## Layout

- `src/A3.MinimalApiValidation/` — the library (only shipped project).
- `examples/Example/` — runnable sample endpoints; each implements `IEndpoint` and self-registers.
- `test/A3.MinimalApiValidation.Tests/` — xUnit tests (mostly full-pipeline integration tests).
- `test/TestClient/` — minimal host exposing `Program` for `WebApplicationFactory<Program>`.

## Architecture — two validation paths

1. **Middleware** (`Internal/Middleware/ValidationMiddleware`) — runs before the endpoint. Reads
   endpoint parameter metadata (`ParameterAttributeInfo`, cached) and validates any parameter
   attributed `[FromBody]` / `[FromQuery]` / `[FromHeader]`. Dispatches to `Body` or `HeaderOrQuery`.
   Un-attributed parameters are ignored.
2. **Custom binders** (`Binders/FromQuery<T>`, `QueryTypeInfo`) — bind a group of query params to a
   model, validated via `Internal/Filter/RequestModelValidationFilter`. Use this path for per-element
   collection rules (`RuleForEach`) and `List<T>`/`IEnumerable<T>` collection properties.

Supporting pieces:
- `Internal/Utils` — `TryCastValue` (scalar `TypeParsers` dict + a parse-convention fallback for
  enums / `IParsable<T>` / static `TryParse`), `IsEnumerable` / `IsCollectionType`, DataAnnotations
  fallback, JSON helpers.
- `Internal/Filter/ValidateOnlyFilter` — short-circuits with `202` when the validate-only header is set.
- `ValidationAttributes/` — custom attributes (`Min`, `Max`, `GreaterThan`, …).
- `EndpointValidatorOptions` — `FallbackToDataAnnotations`, `PreferExplicitRequestModelValidation`,
  `ValidateOnlyHeader`, `JsonSerializerOptions`.

Wire-up: `services.AddEndpointValidation(...)` + `app.UseEndpointValidation()`.

## Conventions

- File-scoped namespaces; `using` directives **inside** the namespace; nullable + implicit usings on.
- Validation of query/header collections is **collection-level** (attributes apply to the materialised
  collection, matching `System.ComponentModel.DataAnnotations`). Per-element rules go through
  `FromQuery<T>` + FluentValidation. A parameter type is validatable if the framework can bind it
  (scalar, enum, `IParsable<T>`, static `TryParse(string, out T)`, or an array of those).
- Tests: one `class : TestBase` per file; override `AddTestEndpoint`; use the per-instance unique
  `Path`; assert with `response.EnsureSuccessStatusCode()` / `response.EnsureErrorFor("key")`;
  snake_case test names; include `// Arrange` / `// Act` / `// Assert` markers.
- Keep the primitive/scalar validation path unchanged when extending — it is covered by a large
  existing test matrix under `ApiIntegrationTests/`.

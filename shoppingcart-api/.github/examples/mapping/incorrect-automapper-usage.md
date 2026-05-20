# Incorrect: AutoMapper Usage

**Why this is incorrect:**
AutoMapper is prohibited in this repository. Reflection-based and convention-based mapping hides the property assignments, makes mapping behavior difficult to trace, and introduces a framework dependency that violates the explicit mapping governance.

```csharp
services.AddAutoMapper(typeof(MappingProfile)); // Prohibited in this repository
```

**What to do instead:**
Write an explicit static mapping function. See [correct-explicit-mapper.md](correct-explicit-mapper.md) for the required pattern.

Also prohibited:
- Mapster
- `ObjectMapper<T>` or similar generic reflection mappers
- `JsonSerializer.Deserialize` used as a cross-model mapper

---
description: "Modern .NET 10 and C# coding standards for this repository."
name: ".NET 10 C# Standards"
applyTo: "src/**/*.cs"
---
# .NET 10 and C# Standards

## Language And API Quality
- Use nullable reference types correctly.
- Prefer readable names and cohesive methods.
- Use modern C# syntax only when clarity improves.

## Async And Cancellation
- Use async and await for I/O work.
- Do not use `.Result`, `.Wait()`, or sync-over-async patterns.
- Use `CancellationToken` on async public methods where appropriate.

## Dependency Injection
- Prefer constructor injection.
- Do not use service locator patterns.
- Avoid static mutable state.

## Exceptions And Logging
- Keep exception handling purposeful and explicit.
- Do not swallow exceptions.
- Use structured logging templates.
- Do not log secrets, tokens, passwords, PII, or sensitive business data.

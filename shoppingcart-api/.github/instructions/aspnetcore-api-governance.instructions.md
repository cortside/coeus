---
description: "ASP.NET Core API and MVC governance for thin controllers and correct boundaries."
name: "ASP.NET Core API Governance"
applyTo: "src/**/*Api/**/*.cs"
---
# ASP.NET Core MVC and API Governance

## Controller Responsibilities
- Bind and validate request DTOs.
- Map request DTOs to internal request models.
- Delegate business workflow to facades.
- Map facade results to response DTOs.

## Controller Prohibitions
- No business logic in controllers.
- No direct repository access.
- No direct DbContext or EF Core access.

## API Conventions
- Use proper HTTP status codes.
- Maintain model validation behavior.
- Preserve existing route and versioning conventions if present.
- Preserve existing problem-details and OpenAPI conventions.

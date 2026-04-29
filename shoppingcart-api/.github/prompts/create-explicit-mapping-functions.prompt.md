---
description: "Create explicit mapping functions between layer models."
name: "Create Explicit Mapping Functions"
argument-hint: "Source model, target model, and mapping rules"
---
Create explicit mapping code.

Rules:
- Do not use AutoMapper.
- Do not use reflection-based mapping.
- Keep mapping readable and testable.
- Place mapping at the correct layer boundary.

Testing:
- Add tests for defaults, computed values, enums, nullability, and nested structures where applicable.

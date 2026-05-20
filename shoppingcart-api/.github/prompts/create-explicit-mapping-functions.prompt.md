---
description: "Create explicit mapping functions between layer models."
name: "Create Explicit Mapping Functions"
argument-hint: "Source model name, target model name, and layer boundary where mapping lives"
---
Create explicit mapping between two layer models.

1. Write a static or instance mapping function with explicit property assignments.
2. Place the function at the correct layer boundary: controller for request/response, facade for domain-to-DTO, repository for entity-to-domain.
3. Do not use AutoMapper or reflection-based mapping.
4. Add unit tests for: default values, computed fields, enum conversions, null inputs, and nested structures where applicable.

For a detailed step-by-step playbook use the `explicit-mapping` skill.

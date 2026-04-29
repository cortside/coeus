---
description: "Add an API endpoint while preserving architecture boundaries."
name: "Add Endpoint With Architecture Rules"
argument-hint: "Endpoint description and request/response contract"
---
Implement an endpoint using this repository architecture.

Required behavior:
- Controller performs transport-bound validation and mapping.
- Controller delegates workflow to facade.
- Facade orchestrates domain services.
- Domain services interact with repositories.
- Repositories encapsulate persistence.

Prohibited behavior:
- No controller business logic.
- No controller direct repository or DbContext access.
- No AutoMapper.

Validation:
- Add unit tests.
- Add integration tests when endpoint pipeline behavior is affected.

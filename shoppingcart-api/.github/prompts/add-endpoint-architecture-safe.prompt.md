---
description: "Add an API endpoint while preserving architecture boundaries."
name: "Add Endpoint With Architecture Rules"
argument-hint: "Endpoint route, HTTP method, request/response contract, and target facade"
---
Add a new API endpoint following repository architecture.

1. Create a thin controller action: bind, validate, map to internal command, delegate to facade, map result to response DTO.
2. Add or update the facade method to orchestrate domain services.
3. Add explicit request-to-command and result-to-response mapping at the controller boundary.
4. Add unit tests for controller and facade behavior.
5. Add a lightweight integration test for the endpoint pipeline.

Do not place business logic in the controller.
Do not call repositories or DbContext directly from the controller.
Do not use AutoMapper.

For a detailed step-by-step playbook use the `add-api-endpoint` skill.

---
description: "Add unit tests for a selected layer with clear behavioral coverage."
name: "Add Unit Tests For Layer"
argument-hint: "Component name and behavior or method to test"
---
Create unit tests for the specified component.

1. Cover: success path, validation failure, error/exception path, null/empty inputs, and important edge cases.
2. Use Arrange / Act / Assert structure.
3. Keep tests isolated — mock or stub only what is necessary to clarify behavior.
4. Test observable behavior, not implementation details.
5. Include mapping behavior tests where the component performs meaningful transforms.

For a detailed step-by-step playbook use the `add-unit-tests` skill.

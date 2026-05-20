# Correct: Troubleshooting Artifact Format

**Why this is correct:**
Each iteration is documented in its own file inside a timestamped artifact folder. No code changes are made before the analysis iteration completes. Each subsequent iteration requires explicit approval.

Artifact folder name: `artifacts/20260520-143015-fix-order-total-calculation/`

```markdown
# Iteration 001 — Analysis
- What was inspected: OrderFacade.CheckoutAsync, DiscountDomainService.ApplyDiscountAsync
- What was changed: Nothing — analysis only
- Why changed: N/A
- Tests run: None — analysis only
- Result: Root cause candidate identified: discount applied after tax calculation instead of before
- Next recommended step: Modify DiscountDomainService.ApplyDiscountAsync to apply discount before tax; run unit tests
```

**Key governance points:**
- Folder name includes date-time stamp and short description.
- Iteration 001 is always analysis-only — no code changes.
- Each iteration has its own file: iteration-001-analysis.md, iteration-002-change-and-test.md.
- Copilot stops after each file and waits for user approval before creating the next.

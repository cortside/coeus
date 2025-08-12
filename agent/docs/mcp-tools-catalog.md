# MCP Tools Catalog

This document describes all available MCP tools exposed over HTTP.

The MCP client connects to the MCP server at the configured `MCP_BASE_URL` and invokes tools via:
```curl
POST /tools/{tool_name}
Content-Type: application/json
Body: JSON matching the tool's input schema
```

Responses are JSON matching the tool's output schema.

---

## 📌 General Rules for Tool Integration
- Always call tools asynchronously using `MCPClient.call_tool()` from `mcp_client.py`.
- Validate output against the Pydantic schema before returning to the caller.
- Record metrics for tool calls in Prometheus and instrument with OTEL spans.
- Retry failed calls up to the configured limit with exponential backoff.

---

## Tool List

### 1. `summarize`
**Description:** Summarizes a given text into a shorter form.  
**Endpoint:** `POST /tools/summarize`  
**Input Schema:**
```json
{
  "text": "string"
}
```
Output Schema:

```json
{
  "result": "string"
}
```
Pydantic Output Model:

```python
class SummarizeOutput(BaseModel):
    result: str
```    
Example Request:

```json
{
  "text": "The quick brown fox jumps over the lazy dog."
}
```
Example Response:

```json
{
  "result": "A quick fox jumps over a lazy dog."
}
2. extract_entities
Description: Extracts named entities from text (people, locations, organizations).
Endpoint: POST /tools/extract_entities
Input Schema:

```json
{
  "text": "string"
}
```
Output Schema:

```json
{
  "entities": [
    {
      "type": "string",
      "value": "string"
    }
  ]
}
```
Pydantic Output Model:

```python
class Entity(BaseModel):
    type: str
    value: str

class ExtractEntitiesOutput(BaseModel):
    entities: List[Entity]
```    
Example Request:

```json
{
  "text": "Barack Obama visited Paris in July."
}
```
Example Response:

```json
{
  "entities": [
    { "type": "PERSON", "value": "Barack Obama" },
    { "type": "LOCATION", "value": "Paris" },
    { "type": "DATE", "value": "July" }
  ]
}
```
3. translate
Description: Translates text from a source language to a target language.
Endpoint: POST /tools/translate
Input Schema:

```json
{
  "text": "string",
  "source_lang": "string",
  "target_lang": "string"
}
```
Output Schema:

```json
{
  "translated": "string"
}
```
Pydantic Output Model:

```python
class TranslateOutput(BaseModel):
    translated: str
```    
Example Request:

```json
{
  "text": "Hello world",
  "source_lang": "en",
  "target_lang": "fr"
}
```
Example Response:

```json
{
  "translated": "Bonjour le monde"
}
```

🚀 Adding a New Tool
Add tool details here (name, description, endpoint, schemas, examples).

Create a Pydantic output schema in mcp_client.py or /schemas package.

Call via:

```python
result = await mcp.call_tool(
    tool_name="new_tool",
    payload={"key": "value"},
    output_schema=NewToolOutputSchema
)
```
Add unit tests under /tests/test_mcp_tools/.

```yaml

---

### Why this is useful for Copilot
When you **keep this file updated**, Copilot Chat can:
- Auto-generate correct function signatures for each tool
- Scaffold API routes that wrap tool calls
- Provide sample payloads instantly  
- Avoid schema mismatches

---

If you want, I can now also **merge this catalog directly into the Copilot recipes** so that when you paste a “create endpoint for tool X” request, it automatically pulls the right schema and example calls.  

Do you want me to **update the `/docs/copilot-recipes.md`** so each tool has a pre-made recipe for backend + frontend integration? That way you can integrate tools in one command.
```

# ContentParserApi

ASP.NET Core Web API endpoint that accepts a Base64-encoded payload and parses it as `CSV` or `INTERNAL_JSON`.

## Requirements

- .NET 8 SDK or newer

## Run locally

```bash
dotnet restore
dotnet run --project ContentParserApi
```

After startup, the app listens on:

```text
http://localhost:5299
```

## Endpoint

- **Method:** `POST`
- **Path:** `/api/v1/parse-content`
- **Header:** `Content-Type: application/json`

### Request body

```json
{
  "type": "CSV | INTERNAL_JSON",
  "content": "<Base64-encoded raw data>"
}
```

| Field | Description |
|---|---|
| `type` | Content format inside `content` |
| `content` | Raw data encoded with Base64 |

### Response body

Success:

```json
{
  "status": "Success",
  "count": 3,
  "data": [
    { "name": "Ann", "age": "30", "city": "Warsaw" }
  ],
  "error": null
}
```

Error:

```json
{
  "status": "Error",
  "count": 0,
  "data": [],
  "error": "Unsupported content type"
}
```

HTTP status codes:

- `200 OK` — parsing succeeded
- `400 Bad Request` — invalid type, missing content, invalid Base64, invalid CSV/JSON

## Supported formats

### CSV

Comma-separated values with a header row.

Example raw content:

```text
name,age,city
Ann,30,Warsaw
Bob,25,Krakow
Ola,28,Gdansk
```

### INTERNAL_JSON

JSON array of flat objects. Supported value types: `string`, `number`, `boolean`, `null`.  
Nested objects and arrays are not supported.

Example raw content:

```json
[
  { "name": "Ann", "age": "30", "city": "Warsaw" },
  { "name": "Bob", "age": "25", "city": "Krakow" },
  { "name": "Ola", "age": "28", "city": "Gdansk" }
]
```

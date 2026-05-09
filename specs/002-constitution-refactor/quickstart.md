# Quickstart / Validation Guide: Constitution Compliance Refactor

**Branch**: `002-constitution-refactor` | **Date**: 2026-05-09

## How to verify the refactor is complete

Run each check in order. All must pass before merging.

### 1. Solution compiles with zero errors

```powershell
dotnet build skyress.sln
# Expected: "Build succeeded. 0 Error(s)"
```

### 2. No handler returns a domain entity

```powershell
# Search for handlers that still return domain aggregates directly
# These patterns should produce zero matches after the refactor:
Select-String -Path "Skyress.Application\**\*.cs" -Pattern "Result<Item>" -Recurse | Where-Object { $_ -notmatch "ItemResponse" }
Select-String -Path "Skyress.Application\**\*.cs" -Pattern "Result<Customer>" -Recurse
Select-String -Path "Skyress.Application\**\*.cs" -Pattern "Result<Invoice>" -Recurse | Where-Object { $_ -notmatch "InvoiceResponse" }
```

### 3. All commands have validators

```powershell
# Count command files vs validator files per feature
$commands = (Get-ChildItem -Recurse -Filter "*Command.cs" "Skyress.Application" | Where-Object Name -notmatch "Handler").Count
$validators = (Get-ChildItem -Recurse -Filter "*Validator.cs" "Skyress.Application").Count
Write-Output "Commands: $commands, Validators: $validators"
# Expected: validators count >= commands count
```

### 4. All handlers have logging

```powershell
# Every handler file should contain at least one LogInformation call
Get-ChildItem -Recurse -Filter "*Handler.cs" "Skyress.Application" |
  Where-Object { !(Select-String -Path $_.FullName -Pattern "LogInformation") } |
  Select-Object Name
# Expected: no output (all handlers log)
```

### 5. ExceptionHandlingMiddleware is registered

```powershell
Select-String -Path "Skyress\Program.cs" -Pattern "ExceptionHandlingMiddleware"
# Expected: at least one match
```

### 6. No raw string errors in endpoints

```powershell
Select-String -Path "Skyress\Endpoints\**\*.cs" -Pattern "BadRequest<string>" -Recurse
# Expected: zero matches
```

### 7. All namespace references updated after folder renames

```powershell
Select-String -Path "**\*.cs" -Pattern "Skyress\.Domain\.primitives" -Recurse
# Expected: zero matches (all updated to Skyress.Domain.Primitives)

Select-String -Path "**\*.cs" -Pattern "TagAssignmnet" -Recurse
# Expected: zero matches (all updated to TagAssignment)
```

### 8. Manual smoke test

Start the API and call one endpoint to verify happy path still works:

```powershell
dotnet run --project Skyress\Skyress.API.csproj
# In another terminal:
curl -X GET http://localhost:5000/api/items
# Expected: HTTP 200 with JSON array
```

Deliberately trigger a validation error:

```powershell
curl -X POST http://localhost:5000/api/items -H "Content-Type: application/json" -d "{}"
# Expected: HTTP 400 with ProblemDetails JSON body (not a raw string)
```

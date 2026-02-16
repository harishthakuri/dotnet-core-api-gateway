### Generate JWT Token for authentication

NOTE: Generating token is just for demo purpose NOT a production ready solution.

```bash
cd api-gateway

./generate-token.sh

#ouput
'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsImp0aSI6IjA4MjcwM2MzLTQ1MjItNGRhYy1hMzk0LWY4NDM4MGI5NTY1YyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJ0ZXN0dXNlciIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzY5NTcxNjU1LCJpc3MiOiJBcGlHYXRld2F5IiwiYXVkIjoiQXBpR2F0ZXdheUNsaWVudHMifQ.oRpNqgq0_xWAGP4gtQCG1KbWJ5X6CZRE3kh6BldAT-A'

```

### Run API Gateway and Product Service

```PlainText
API Gateway: http://localhost:5000
Product Service: http://localhost:5002

# Client will access product service via API Gateway, not directly
http://localhost:5000/api/products

```

### Access service using Curl command

```bash
curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsImp0aSI6IjA4MjcwM2MzLTQ1MjItNGRhYy1hMzk0LWY4NDM4MGI5NTY1YyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJ0ZXN0dXNlciIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzY5NTcxNjU1LCJpc3MiOiJBcGlHYXRld2F5IiwiYXVkIjoiQXBpR2F0ZXdheUNsaWVudHMifQ.oRpNqgq0_xWAGP4gtQCG1KbWJ5X6CZRE3kh6BldAT-A" \
  http://localhost:5000/api/products
  

  
```

✅ Response should look like this:

```json
[
  {
    "id": 1,
    "name": "Laptop",
    "price": 999.99,
    "category": "Electronics"
  },
  {
    "id": 2,
    "name": "Mouse",
    "price": 29.99,
    "category": "Electronics"
  },
  {
    "id": 3,
    "name": "Keyboard",
    "price": 79.99,
    "category": "Electronics"
  },
  {
    "id": 4,
    "name": "Monitor",
    "price": 299.99,
    "category": "Electronics"
  },
  {
    "id": 5,
    "name": "Desk Chair",
    "price": 199.99,
    "category": "Furniture"
  }
]
```

**Use OAuth2/OpenID Connect Providers**

- Azure AD / Entra ID (Microsoft)
- Auth0
- Okta
- AWS Cognito
- Google Identity
- Keycloak (open-source)

---

### The ProductService port configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Urls": "http://localhost:5002"
}
```

**Alternatively**, you can run it with the port specified in the command:

```bash
dotnet run --urls "http://localhost:5002"
```

Or update the `launchSettings.json` if it exists in the `Properties` folder:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5002",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

After making this change, run:

```bash
dotnet run
```

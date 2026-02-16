## Run API Gateway and Product Service

```PlainText
API Gateway: http://localhost:5000
Product Service: http://localhost:5002

# Client will access product service via API Gateway, not directly
http://localhost:5000/api/products

```

#### Curl command

```bash
curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsImp0aSI6IjkzZTA4NzllLWI1N2ItNDg4Zi04NTA5LTEzMzRlM2U5ZjRhOSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJ0ZXN0dXNlciIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzcxMjY2NTYyLCJpc3MiOiJBcGlHYXRld2F5IiwiYXVkIjoiQXBpR2F0ZXdheUNsaWVudHMifQ.-pVvz0at00aOCF2nQ1tiysDg10xyUkv-5SPhEjuXqPA" \
  http://localhost:5000/api/products

```

# Ocelot Route Configuration Explained

This is a **single route configuration** in `ocelot.json` that defines how the API Gateway handles requests for product details.

## Configuration Breakdown

```json
{
  "UpstreamPathTemplate": "/api/products/{id}",
  "UpstreamHttpMethod": ["Get"],
  "DownstreamPathTemplate": "/products/{id}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "product-service",
      "Port": 80
    }
  ]
}
```

## What Each Property Means

### 1. **UpstreamPathTemplate** (Client-Facing Route)

```json
{
  "UpstreamPathTemplate": "/api/products/{id}"
}
```

- This is the **public API route** that clients call
- Example client request: `GET https://api-gateway.com/api/products/123`
- `{id}` is a **route parameter** (placeholder) that captures the product ID
- The gateway listens for this pattern

### 2. **UpstreamHttpMethod** (Allowed HTTP Methods)

```json
{
  "UpstreamHttpMethod": [
    "Get"
  ]
}
```

- Only **GET** requests are allowed on this route
- Any POST, PUT, DELETE requests to this path will be rejected
- You can specify multiple methods: `["Get", "Post", "Put"]`

### 3. **DownstreamPathTemplate** (Microservice Route)

```json
{"DownstreamPathTemplate": "/products/{id}"}
```

- This is the **actual route** on the downstream Product Service
- Notice: Gateway strips `/api` prefix before forwarding
- The `{id}` parameter is forwarded automatically
- Example: Gateway forwards to `http://product-service:80/products/123`

### 4. **DownstreamScheme** (Protocol)

```json
{
  "DownstreamScheme": "http"
}
```

- Use `http` or `https` to connect to the downstream service
- In production, prefer `https` for secure communication
- For local/internal services, `http` is often used

### 5. **DownstreamHostAndPorts** (Service Location)

```json
{
  "DownstreamHostAndPorts": [
    {
      "Host": "product-service",
      "Port": 80
    }
  ]
}
```

- **Host**: DNS name or IP address of the microservice
    - `product-service` - Docker container name (in Docker Compose)
    - `localhost` - Local development
    - `10.0.1.5` - Static IP
    - `product-api.internal.com` - Internal DNS
- **Port**: The port the microservice listens on
- You can specify **multiple hosts** for load balancing:
  ```json
  {
  "DownstreamHostAndPorts": [
    { "Host": "product-service-1", "Port": 80 },
    { "Host": "product-service-2", "Port": 80 }
  ]
  }
  ```

## Request Flow Example

### Client Request

```http
GET https://api-gateway.com/api/products/123
Authorization: Bearer <token>
```

### Gateway Processing

1. Matches route: `/api/products/{id}` with `id = 123`
2. Validates HTTP method: `GET` ✓
3. Constructs downstream URL: `http://product-service:80/products/123`
4. Forwards request to Product Service

### Downstream Request

```http
GET http://product-service:80/products/123
Authorization: Bearer <token>  # Forwarded automatically
```

### Response Flow

Product Service → Gateway → Client (same response body/headers)

## Common Variations

### Multiple HTTP Methods

```json
{
  "UpstreamPathTemplate": "/api/products",
  "UpstreamHttpMethod": ["Get", "Post"],
  "DownstreamPathTemplate": "/products",
  "DownstreamScheme": "https",
  "DownstreamHostAndPorts": [{ "Host": "product-service", "Port": 443 }]
}
```

### With Service Discovery (Consul)

```json
{
  "UpstreamPathTemplate": "/api/products/{id}",
  "UpstreamHttpMethod": ["Get"],
  "DownstreamPathTemplate": "/products/{id}",
  "DownstreamScheme": "http",
  "ServiceName": "product-service", 
  "LoadBalancerOptions": {
    "Type": "RoundRobin"
  }
}
```

### With Authentication & Rate Limiting

```json
{
  "UpstreamPathTemplate": "/api/products/{id}",
  "UpstreamHttpMethod": ["Get"],
  "DownstreamPathTemplate": "/products/{id}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [{ "Host": "product-service", "Port": 80 }],
  "AuthenticationOptions": {
    "AuthenticationProviderKey": "Bearer"
  },
  "RateLimitOptions": {
    "EnableRateLimiting": true,
    "Period": "1s",
    "Limit": 10
  }
}
```

## Key Takeaways

✅ **Upstream** = What clients see (public API)  
✅ **Downstream** = Where the actual microservice lives (internal)  
✅ Gateway acts as a **reverse proxy** and **router**  
✅ Route parameters like `{id}` are automatically forwarded  
✅ You can apply **authentication, rate limiting, caching** per route

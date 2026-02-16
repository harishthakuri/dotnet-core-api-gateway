## Running client and API (API gateway and backend API)

## 1. Run `src/ApiGateway.Api`

```bash
cd src/ApiGateway.Api

dotnet run

#  http://localhost:5000
```


## 2. Run `src/ProductService`

```bash
cd src/ProductService

dotnet run

# http://localhost:5002

```

## 3. Run React application `client/`

```bash
npm install

npm run dev

# http://localhost:5173/
```
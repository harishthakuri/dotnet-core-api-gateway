# React SPA Application

```PlainText
[Auth0 React App] ─────┐
                       ├──> [API Gateway (Ocelot)] ──> [Internal Token] ──> [Your APIs]
```

## Register SPA application in Auth0

1. Go to https://manage.auth0.com/dashboard/
2. Click 'Create Application' → Single Page Application
3. Set Allowed Callback URLs: http://localhost:5173
4. Set Allowed Logout URLs: http://localhost:5173
5. Set Allowed Web Origins: http://localhost:5173
6. Update .env file with your Domain and Client ID

#### Create a new React project for this Quickstart

npm create vite@latest auth0-react -- --template react-ts

#### Open the project

cd auth0-react

#### Install the Auth0 React SDK

npm add @auth0/auth0-react && npm install

---

#### `.env`

```
VITE_AUTH0_DOMAIN=###.us.auth0.com
VITE_AUTH0_CLIENT_ID=nyB####gMIZ
VITE_API_BASE_URL=http://localhost:5000

```

#### `main.tsx`

```javascript
ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    <Auth0Provider
      domain={domain}
      clientId={clientId}
      authorizationParams={{
        redirect_uri: window.location.origin,
        audience: "https://api.rabbitgateway.com", // Required for API access tokens
        scope: "openid profile email read:data write:data",
      }}
      cacheLocation="localstorage"
      useRefreshTokens={true}
    >
      <App />
    </Auth0Provider>
  </React.StrictMode>,
);
```

#### Run application

```bash
npm run dev
```

http://localhost:5173/

---

### Detailed Flow: React App Login & Token Acquisition

#### Step 1: User Clicks Login

```javascript
// In your React app
import { useAuth0 } from "@auth0/auth0-react";

function LoginButton() {
  const { loginWithRedirect } = useAuth0();

  return <button onClick={() => loginWithRedirect()}>Log In</button>;
}
```

### **Behind the Scenes (PKCE Flow)**

When `loginWithRedirect()` is called, here's what happens:

```
┌─────────────┐                                    ┌─────────────┐
│ React App   │                                    │   Auth0     │
│ (Browser)   │                                    │             │
└──────┬──────┘                                    └──────┬──────┘
       │                                                  │
       │ 1. Generate random code_verifier                │
       │    code_verifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk"
       │                                                  │
       │ 2. Create code_challenge                        │
       │    code_challenge = SHA256(code_verifier)       │
       │    code_challenge = "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM"
       │                                                  │
       │ 3. Redirect to Auth0 with code_challenge        │
       │    (NO CLIENT SECRET SENT!)                     │
       │─────────────────────────────────────────────────>│
       │                                                  │
       │ GET /authorize?                                  │
       │   response_type=code                            │
       │   client_id=YOUR_CLIENT_ID                      │ ✅ Only Client ID
       │   redirect_uri=http://localhost:3000/callback   │
       │   code_challenge=E9Melhoa2OwvFrEMTJguCHaoeK1... │ ✅ Code challenge
       │   code_challenge_method=S256                    │
       │   scope=openid profile email                    │
       │   audience=https://api.yourapp.com              │
       │                                                  │
       │ 4. Auth0 shows login page                       │
       │<─────────────────────────────────────────────────│
       │                                                  │
       │ 5. User enters credentials                      │
       │─────────────────────────────────────────────────>│
       │                                                  │
       │ 6. Auth0 validates & stores code_challenge      │
       │                                                  │
       │ 7. Redirect back with authorization CODE        │
       │<─────────────────────────────────────────────────│
       │                                                  │
       │ http://localhost:3000/callback?code=AUTH_CODE   │
       │                                                  │
```

### **Exchange Code for Token (The Key Part!)**

Now the React app exchanges the authorization code for an access token:

```
┌─────────────┐                                    ┌─────────────┐
│ React App   │                                    │   Auth0     │
└──────┬──────┘                                    └──────┬──────┘
       │                                                  │
       │ 8. Exchange code for token                      │
       │─────────────────────────────────────────────────>│
       │                                                  │
       │ POST /oauth/token                               │
       │   grant_type=authorization_code                 │
       │   code=AUTH_CODE                                │ ✅ Authorization code
       │   client_id=YOUR_CLIENT_ID                      │ ✅ Client ID
       │   code_verifier=dBjftJeZ4CVP-mB92K27uhbUJU1p1r │ ✅ Original verifier
       │   redirect_uri=http://localhost:3000/callback   │
       │   (NO CLIENT SECRET!)                           │
       │                                                  │
       │                        9. Auth0 validates:       │
       │                           - Is this the correct  │
       │                             client_id?           │
       │                           - Is the code valid?   │
       │                           - Does SHA256(code_    │
       │                             verifier) match the  │
       │                             code_challenge we    │
       │                             stored earlier?      │
       │                                                  │
       │ 10. Auth0 returns tokens                        │
       │<─────────────────────────────────────────────────│
       │                                                  │
       │ {                                               │
       │   "access_token": "eyJhbGci...",                │ ✅ Access Token (JWT)
       │   "id_token": "eyJhbGci...",                    │
       │   "refresh_token": "v1.Mkj...",                 │
       │   "token_type": "Bearer",                       │
       │   "expires_in": 86400                           │
       │ }                                               │
```

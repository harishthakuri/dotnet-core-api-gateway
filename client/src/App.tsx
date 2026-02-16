import { useAuth0 } from "@auth0/auth0-react";
import LoginButton from "./LoginButton";
import LogoutButton from "./LogoutButton";

import ApiDemo from "./ApiDemo";
import { useEffect } from "react";

function App() {
  const { isAuthenticated, isLoading, error, user, getAccessTokenSilently } =
    useAuth0();

  useEffect(() => {
    const getToken = async () => {
      if (!isAuthenticated) {
        return;
      }

      try {
        console.log("Getting access token with audience and scope...");
        console.log("Is authenticated:", isAuthenticated);

        const token = await getAccessTokenSilently({
          authorizationParams: {
            audience: "https://api.rabbitgateway.com",
            scope: "openid profile email read:data write:data",
          },
        });

        if (token && token.length > 0) {
          console.log("Access Token retrieved successfully");
          console.log("Access Token preview:", token);
        } else {
          throw new Error("Token is empty or undefined");
        }
      } catch (error: any) {
        const errorMessage = error?.message || "Unknown error occurred";
        console.error("Error getting access token:", error);
        console.error("Error details:", {
          message: errorMessage,
          error: error,
          stack: error?.stack,
        });
      }
    };

    getToken();
  }, [getAccessTokenSilently, isAuthenticated]);

  if (isLoading) {
    return (
      <div className="app-container">
        <div className="loading-state">
          <div className="loading-text">Loading...</div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="app-container">
        <div className="error-state">
          <div className="error-title">Oops!</div>
          <div className="error-message">Something went wrong</div>
          <div className="error-sub-message">{error.message}</div>
        </div>
      </div>
    );
  }

  return (
    <div className="app-container">
      <div className="main-card-wrapper">
        <img
          src="https://cdn.auth0.com/quantum-assets/dist/latest/logos/auth0/auth0-lockup-en-ondark.png"
          alt="Auth0 Logo"
          className="auth0-logo"
          onError={(e) => {
            e.currentTarget.style.display = "none";
          }}
        />
        <h1 className="main-title">React SPA</h1>

        {isAuthenticated && user ? (
          <div className="logged-in-section">
            <div className="logged-in-message">
              ✅ Successfully authenticated!
            </div>
            <div>Please,Check console for access token.</div>
            {/* <h2 className="profile-section-title">Your Profile</h2>
            <div className="profile-card">
              <Profile />
            </div> */}
            <h2 className="profile-section-title"> {user.name}</h2>
            <LogoutButton />
            <ApiDemo />
          </div>
        ) : (
          <div className="action-card">
            <p className="action-text">
              Get started by signing in to your account
            </p>
            <LoginButton />
          </div>
        )}
      </div>
    </div>
  );
}

export default App;

import { useAuth0 } from "@auth0/auth0-react";
import { useState, useEffect } from "react";

const Profile = () => {
  const { user, isAuthenticated, isLoading, getAccessTokenSilently } =
    useAuth0();
  const [accessToken, setAccessToken] = useState<string>("");
  const [tokenError, setTokenError] = useState<string>("");

  useEffect(() => {
    const getToken = async () => {
      if (!isAuthenticated) {
        setAccessToken("");
        setTokenError("");
        return;
      }

      try {
        setTokenError("");
        console.log("Getting access token with audience and scope...");
        console.log("Is authenticated:", isAuthenticated);

        const token = await getAccessTokenSilently({
          authorizationParams: {
            audience: "https://api.rabbitgateway.com",
            scope: "openid profile email read:data write:data",
          },
        });

        if (token && token.length > 0) {
          setAccessToken(token);
          console.log("Access Token retrieved successfully");
          console.log("Access Token length:", token.length);
          console.log("Access Token parts:", token.split(".").length);
          console.log("Access Token preview:", token.substring(0, 50) + "...");
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
        setTokenError(`Failed to get token: ${errorMessage}`);
        setAccessToken("");
      }
    };

    getToken();
  }, [getAccessTokenSilently, isAuthenticated]);

  if (isLoading) {
    return <div className="loading-text">Loading profile...</div>;
  }

  return isAuthenticated && user ? (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: "1rem",
      }}
    >
      <img
        src={
          user.picture ||
          `data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='110' height='110' viewBox='0 0 110 110'%3E%3Ccircle cx='55' cy='55' r='55' fill='%2363b3ed'/%3E%3Cpath d='M55 50c8.28 0 15-6.72 15-15s-6.72-15-15-15-15 6.72-15 15 6.72 15 15 15zm0 7.5c-10 0-30 5.02-30 15v3.75c0 2.07 1.68 3.75 3.75 3.75h52.5c2.07 0 3.75-1.68 3.75-3.75V72.5c0-9.98-20-15-30-15z' fill='%23fff'/%3E%3C/svg%3E`
        }
        alt={user.name || "User"}
        className="profile-picture"
        style={{
          width: "110px",
          height: "110px",
          borderRadius: "50%",
          objectFit: "cover",
          border: "3px solid #63b3ed",
        }}
        onError={(e) => {
          const target = e.target as HTMLImageElement;
          target.src = `data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='110' height='110' viewBox='0 0 110 110'%3E%3Ccircle cx='55' cy='55' r='55' fill='%2363b3ed'/%3E%3Cpath d='M55 50c8.28 0 15-6.72 15-15s-6.72-15-15-15-15 6.72-15 15 6.72 15 15 15zm0 7.5c-10 0-30 5.02-30 15v3.75c0 2.07 1.68 3.75 3.75 3.75h52.5c2.07 0 3.75-1.68 3.75-3.75V72.5c0-9.98-20-15-30-15z' fill='%23fff'/%3E%3C/svg%3E`;
        }}
      />
      <div style={{ textAlign: "center" }}>
        <div
          className="profile-name"
          style={{
            fontSize: "2rem",
            fontWeight: "600",
            color: "#f7fafc",
            marginBottom: "0.5rem",
          }}
        >
          {user.name}
        </div>
        <div
          className="profile-email"
          style={{ fontSize: "1.15rem", color: "#a0aec0" }}
        >
          {user.email}
        </div>
      </div>
      <div style={{ marginTop: "1.5rem", width: "100%", maxWidth: "450px" }}>
        <div
          style={{
            fontSize: "1.1rem",
            fontWeight: "600",
            color: "#f7fafc",
            marginBottom: "0.5rem",
          }}
        >
          Access Token
        </div>
        {tokenError ? (
          <div
            style={{
              backgroundColor: "#742a2a",
              padding: "1rem",
              borderRadius: "8px",
              fontSize: "0.9rem",
              color: "#ffcccc",
              border: "1px solid #a53e3e",
            }}
          >
            <div style={{ fontWeight: "600", marginBottom: "0.5rem" }}>
              ⚠️ Error
            </div>
            <div>{tokenError}</div>
            <div
              style={{
                marginTop: "0.5rem",
                fontSize: "0.8rem",
                color: "#ffaaaa",
              }}
            >
              Check browser console for details
            </div>
          </div>
        ) : accessToken ? (
          <div
            style={{
              backgroundColor: "#1a1e27",
              padding: "1rem",
              borderRadius: "8px",
              fontSize: "0.85rem",
              color: "#cbd5e0",
              wordBreak: "break-all",
              fontFamily: "monospace",
              maxHeight: "150px",
              overflowY: "auto",
              border: "1px solid #2d313c",
            }}
          >
            {accessToken}
          </div>
        ) : (
          <div
            style={{
              backgroundColor: "#2d313c",
              padding: "1rem",
              borderRadius: "8px",
              fontSize: "0.9rem",
              color: "#a0aec0",
              textAlign: "center",
              border: "1px solid #3d414c",
            }}
          >
            Loading token...
          </div>
        )}
      </div>
    </div>
  ) : null;
};

export default Profile;

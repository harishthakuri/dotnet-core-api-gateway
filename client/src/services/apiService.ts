import type { Auth0ContextInterface } from "@auth0/auth0-react";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:5000";

/**
 * Call a protected API endpoint with authentication
 */
export const callProtectedApi = async <T = unknown>(
  endpoint: string,
  getAccessTokenSilently: Auth0ContextInterface["getAccessTokenSilently"],
  options?: RequestInit,
): Promise<T> => {
  try {
    // Get the access token from Auth0
    const token = await getAccessTokenSilently({
      authorizationParams: {
        audience: "https://api.rabbitgateway.com",
        scope: "openid profile email read:data write:data",
      },
    });

    // Make the API call with the token
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers: {
        ...options?.headers,
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(
        `API call failed: ${response.status} ${response.statusText} - ${errorText}`,
      );
    }

    return await response.json();
  } catch (error) {
    console.error("Error calling protected API:", error);
    throw error;
  }
};

/**
 * Call a public API endpoint (no authentication required)
 */
export const callPublicApi = async <T = unknown>(
  endpoint: string,
  options?: RequestInit,
): Promise<T> => {
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers: {
        ...options?.headers,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(
        `API call failed: ${response.status} ${response.statusText} - ${errorText}`,
      );
    }

    return await response.json();
  } catch (error) {
    console.error("Error calling public API:", error);
    throw error;
  }
};

// Example API calls
export const weatherApi = {
  // Protected endpoint - requires authentication
  getWeatherData: (
    getAccessTokenSilently: Auth0ContextInterface["getAccessTokenSilently"],
  ) => callProtectedApi("/api/weatherforecast", getAccessTokenSilently),

  // Public endpoint - no authentication required
  getPublicData: () => callPublicApi("/api/weatherforecast/public"),
};

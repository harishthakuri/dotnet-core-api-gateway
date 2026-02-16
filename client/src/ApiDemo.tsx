import { useAuth0 } from "@auth0/auth0-react";
import { useState } from "react";
import { callProtectedApi, callPublicApi } from "./services/apiService";

interface ProductData {
  id?: number;
  name?: string;
  price?: number;
  category?: string;
}

const ApiDemo = () => {
  const { getAccessTokenSilently, isAuthenticated } = useAuth0();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [data, setData] = useState<ProductData[] | ProductData | null>(null);

  const handleProtectedApiCall = async () => {
    setLoading(true);
    setError(null);
    setData(null);

    try {
      const result = await callProtectedApi<ProductData[]>(
        "/api/products",
        getAccessTokenSilently,
      );
      setData(result);
      console.log("Protected API Response:", result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred");
    } finally {
      setLoading(false);
    }
  };

  const handlePublicApiCall = async () => {
    setLoading(true);
    setError(null);
    setData(null);

    try {
      const result = await callPublicApi<ProductData>("/health");
      setData(result);
      console.log("Public API Response:", result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        padding: "2rem",
        maxWidth: "800px",
        margin: "0 auto",
      }}
    >
      <h2
        style={{
          fontSize: "1.75rem",
          fontWeight: "600",
          color: "#f7fafc",
          marginBottom: "1.5rem",
        }}
      >
        🚀 API Demo
      </h2>

      <div style={{ display: "flex", gap: "1rem", marginBottom: "1.5rem" }}>
        <button
          onClick={handleProtectedApiCall}
          disabled={!isAuthenticated || loading}
          style={{
            padding: "0.75rem 1.5rem",
            fontSize: "1rem",
            fontWeight: "600",
            color: "#fff",
            backgroundColor: isAuthenticated ? "#4299e1" : "#718096",
            border: "none",
            borderRadius: "8px",
            cursor: isAuthenticated && !loading ? "pointer" : "not-allowed",
            transition: "all 0.2s",
            opacity: loading ? 0.6 : 1,
          }}
        >
          {loading ? "Loading..." : "🔒 Call Protected API"}
        </button>

        <button
          onClick={handlePublicApiCall}
          disabled={loading}
          style={{
            padding: "0.75rem 1.5rem",
            fontSize: "1rem",
            fontWeight: "600",
            color: "#fff",
            backgroundColor: "#48bb78",
            border: "none",
            borderRadius: "8px",
            cursor: loading ? "not-allowed" : "pointer",
            transition: "all 0.2s",
            opacity: loading ? 0.6 : 1,
          }}
        >
          {loading ? "Loading..." : "🌍 Call Public API"}
        </button>
      </div>

      {!isAuthenticated && (
        <div
          style={{
            padding: "1rem",
            backgroundColor: "#2d3748",
            borderLeft: "4px solid #ed8936",
            borderRadius: "4px",
            marginBottom: "1rem",
          }}
        >
          <p style={{ color: "#f7fafc", margin: 0 }}>
            ⚠️ You must be logged in to call the protected API
          </p>
        </div>
      )}

      {error && (
        <div
          style={{
            padding: "1rem",
            backgroundColor: "#742a2a",
            borderLeft: "4px solid #fc8181",
            borderRadius: "4px",
            marginBottom: "1rem",
          }}
        >
          <h3
            style={{
              color: "#fc8181",
              margin: "0 0 0.5rem 0",
              fontSize: "1rem",
              fontWeight: "600",
            }}
          >
            ❌ Error
          </h3>
          <p style={{ color: "#feb2b2", margin: 0, fontSize: "0.875rem" }}>
            {error}
          </p>
        </div>
      )}

      {data && (
        <div
          style={{
            padding: "1.5rem",
            backgroundColor: "#1a202c",
            borderRadius: "8px",
            border: "1px solid #2d3748",
          }}
        >
          <h3
            style={{
              color: "#48bb78",
              margin: "0 0 1rem 0",
              fontSize: "1.25rem",
              fontWeight: "600",
            }}
          >
            ✅ API Response
          </h3>
          <pre
            style={{
              backgroundColor: "#0d1117",
              padding: "1rem",
              borderRadius: "4px",
              overflow: "auto",
              fontSize: "0.875rem",
              color: "#e2e8f0",
              margin: 0,
              fontFamily: "monospace",
            }}
          >
            {JSON.stringify(data, null, 2)}
          </pre>
        </div>
      )}
    </div>
  );
};

export default ApiDemo;

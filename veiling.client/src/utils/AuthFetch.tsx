export async function authFetch(url: string, options: RequestInit = {}) {
  const token = localStorage.getItem("access_token");

  if (!token) {
        throw new Error("No authentication token found");
    }

  const headers: Record<string, string> = {
    ...(options.headers as Record<string, string> || {}),
    "Authorization": `Bearer ${token}`,
  };

  if (!(options.body instanceof FormData)) {
    headers["Content-Type"] = "application/json";
  }

  // 3. Pass the computed headers object to fetch
  const response = await fetch(url, {
    ...options,
    headers: headers // <--- usage of the variable we created above
  });

  // Handle 401 Unauthorized
  if (response.status === 401) {
    // Try to refresh token
    const refreshed = await tryRefreshToken();
    
    if (refreshed) {
      // Retry original request with new token
      return authFetch(url, options);
      } else {
          localStorage.removeItem("access_token");
          localStorage.removeItem("refresh_token");
          throw new Error("Authentication failed");
      }

  }

  return response;
}

async function tryRefreshToken(): Promise<boolean> {
  const refreshToken = localStorage.getItem("refresh_token");
  
  if (!refreshToken) {
    return false;
  }

  try {
    const response = await fetch("/refresh", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ refreshToken })
    });

    if (response.ok) {
      const data = await response.json();
      localStorage.setItem("access_token", data.accessToken);
      
      if (data.refreshToken) {
        localStorage.setItem("refresh_token", data.refreshToken);
      }
      
      return true;
    }
  } catch (error) {
    console.error("Token refresh failed:", error);
  }

  return false;
}
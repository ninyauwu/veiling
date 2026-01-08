export async function authFetch(url: string, options: RequestInit = {}) {
  const token = localStorage.getItem("access_token");

  if (!token) {
    // Redirect to login if no token
    window.location.href = "/login";
    throw new Error("No authentication token found");
  }

  const response = await fetch(url, {
    ...options,
    headers: {
      ...(options.headers || {}),
      "Authorization": `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  });

  // Handle 401 Unauthorized
  if (response.status === 401) {
    // Try to refresh token
    const refreshed = await tryRefreshToken();
    
    if (refreshed) {
      // Retry original request with new token
      return authFetch(url, options);
    } else {
      // Refresh failed, redirect to login
      localStorage.removeItem("access_token");
      localStorage.removeItem("refresh_token");
      window.location.href = "/login";
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
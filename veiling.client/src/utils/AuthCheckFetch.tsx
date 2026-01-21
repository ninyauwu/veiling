export async function authCheckFetch(url: string) {
  const token = localStorage.getItem("access_token");

  if (!token) {
    return null; // not logged in
  }

  try {
    const response = await fetch(url, {
      headers: {
        "Authorization": `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      return null;
    }

    return await response.json();
  } catch {
    return null;
  }
}

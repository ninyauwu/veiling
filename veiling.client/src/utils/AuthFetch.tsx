export function authFetch(url: string, options: any = {}) {
    const token = localStorage.getItem("access_token");

    return fetch(url, {
        ...options,
        headers: {
            ...(options.headers || {}),
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
        }
    });
}

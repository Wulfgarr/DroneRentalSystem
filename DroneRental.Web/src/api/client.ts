const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function apiGet<T>(url: string): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${url}`);

    if (!response.ok) {
        throw new Error(`API error: ${response.status}`);
    }

    return response.json();
}
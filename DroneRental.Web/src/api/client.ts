const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export class ApiError extends Error {
    readonly status: number;

    constructor(status: number) {
        super(`API error: ${status}`);
        this.name = 'ApiError';
        this.status = status;
    }
}

export async function apiGet<T>(url: string): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${url}`);

    if (!response.ok) {
        throw new ApiError(response.status);
    }

    return response.json();
}
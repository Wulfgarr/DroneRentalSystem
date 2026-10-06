const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export class ApiError extends Error {
    readonly status: number;

    constructor(status: number, message?: string) {
        super(message ?? `API error: ${status}`);
        this.name = 'ApiError';
        this.status = status;
    }
}

async function readErrorMessage(response: Response): Promise<string> {
    const fallback = `Request failed (HTTP ${response.status}).`;

    // Error general message due the server error.
    if (response.status >= 500) {
        return 'The server encountered a problem. Please try again later.';
    }

    const contentType = response.headers.get('content-type') ?? '';

    try {
        // Message, when backend return plain text.
        if (contentType.includes('text/plain')) {
            const message = await response.text();
            return message.trim() || fallback;
        }

        // Check, do other responses are JSON.
        if (!contentType.includes('json')) {
            return fallback;
        }

        const data: unknown = await response.json();

        // JSON can contain just the text.
        if (typeof data === 'string') {
            return data.trim() || fallback;
        }

        // Checking, if it is an object.
        if (typeof data !== 'object' || data === null) {
            return fallback;
        }

        if (
            'errors' in data &&
            typeof data.errors === 'object' &&
            data.errors !== null
        ) {
            const messages: string[] = [];

            //Each field can have an array of several messages.
            for (const fieldErrors of Object.values(data.errors)) {
                if (!Array.isArray(fieldErrors)) {
                    continue;
                }

                for (const message of fieldErrors) {
                    if (typeof message === 'string' && message.trim() !== '') {
                        messages.push(message.trim());
                    }
                }
            }

            if (messages.length > 0) {
                return messages.join(' ');
            }
        }

        // If we haven't found any field errors, we try using the title.
        if ('title' in data && typeof data.title === 'string') {
            return data.title.trim() || fallback;
        }

        return fallback;
    } catch {
        // The read operation may fail, for example, if the JSON is invalid.
        return fallback;
    }
}

export async function apiGet<T>(
    url: string,
    token?: string
): Promise<T> {
    const headers = new Headers();

    if (token) {
        headers.set('Authorization', `Bearer ${token}`);
    }

    const response = await fetch(`${API_BASE_URL}${url}`, {
        headers,
    });

    if (!response.ok) {
        const message = await readErrorMessage(response);
        throw new ApiError(response.status, message);
    }

    return response.json();
}

export async function apiPost<TResponse, TRequest>(
    url: string,
    data: TRequest,
    token?: string
): Promise<TResponse> {
    const headers = new Headers({
        'Content-Type' : 'application/json',
    });

    if (token) {
        headers.set('Authorization', `Bearer ${token}`);
    }

    const response = await fetch(`${API_BASE_URL}${url}`, {
        method: 'POST',
        headers,
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        const message = await readErrorMessage(response);
        throw new ApiError(response.status, message);
    }

    return response.json();
}

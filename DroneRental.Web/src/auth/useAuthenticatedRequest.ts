import { ApiError } from '../api/client';
import { useAuth } from './useAuth';

export function useAuthenticatedRequest() {
    const { session, endSession } = useAuth();

    async function runAuthenticated<T>(
        request: (token: string) => Promise<T>
    ): Promise<T> {
        if (session === null) {
            throw new ApiError(401, 'Please log in.');
        }

        try {
            return await request(session.token);
        } catch (error: unknown) {
            if (error instanceof ApiError && error.status === 401) {
                endSession();
            }

            throw error;
        }
    }

    return { runAuthenticated };
}
import { useEffect, useState } from "react";
import type { ReactNode } from "react";
import { AuthContext } from "./AuthContext";
import type { AuthResponse } from "../types/auth";
import { getTokenExpiration } from "./tokenUtils";


type AuthProviderProps = {
    children: ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {

    const [session, setSession] = useState<AuthResponse | null>(null);

    useEffect(() => {
        if (session === null) {
            return;
        }

        const expiration = getTokenExpiration(session.token);

        let timeoutId: number;

        function checkExpiration() {
            window.clearTimeout(timeoutId);

            const remainingTime =
                expiration === null ? 0 : expiration - Date.now();

            if (remainingTime <= 0) {
                setSession(null);
                return;
            }

            const delay = Math.min(remainingTime, 2_147_483_647);

            timeoutId = window.setTimeout(checkExpiration, delay);
        }

        timeoutId = window.setTimeout(checkExpiration, 0);

        window.addEventListener('focus', checkExpiration);

        return () => {
            window.clearTimeout(timeoutId);
            window.removeEventListener('focus', checkExpiration);
        };
    }, [session]);

    function startSession(data: AuthResponse) {
        setSession(data);
    }

    function endSession() {
        setSession(null);
    }

    return (
        <AuthContext.Provider value={{ session, startSession, endSession }}>
            {children}
        </AuthContext.Provider>
    );
}
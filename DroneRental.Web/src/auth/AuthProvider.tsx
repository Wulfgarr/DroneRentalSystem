import { useState } from "react";
import type { ReactNode } from "react";
import { AuthContext } from "./AuthContext";
import type { AuthResponse } from "../types/auth";


type AuthProviderProps = {
    children: ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {

    const [session, setSession] = useState<AuthResponse | null>(null);

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
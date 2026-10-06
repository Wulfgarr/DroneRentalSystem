import { createContext } from "react";
import type { AuthResponse } from "../types/auth";


// Description of data and operations available to app components.
export type AuthContextValue = {
    session: AuthResponse | null;
    startSession: (data: AuthResponse) => void;
    endSession: () => void;
};

// Component did not reviced a context provider.
export const AuthContext = createContext<AuthContextValue | undefined>(
    undefined
)
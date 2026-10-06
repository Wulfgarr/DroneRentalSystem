import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./useAuth";

export function RequiredAuth() {
    const { session } = useAuth();

    if (session === null) {
        return <Navigate to="/login" replace />
    }

    return <Outlet />
}
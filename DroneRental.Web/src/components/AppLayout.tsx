import { Link, NavLink, Outlet } from 'react-router-dom';
import { useAuth } from "../auth/useAuth";

export function AppLayout() {

    const { session, endSession } = useAuth();

    return (
        <div className="app">
            <header className="app-header">
                <Link to="/drones" className="app-brand">
                    DroneRental
                </Link>

                <nav className="app-nav" aria-label="Main navigation">
                    <NavLink
                        to="/drones"
                        className={({ isActive }) =>
                            isActive ? 'nav-link nav-link--active' : 'nav-link'
                        }
                    >
                        Drones
                    </NavLink>
                </nav>

                {/* TODO: Connect with logged in user status*/}
                <div className="app-account">
                    {session ? (
                        <>
                            <Link to="/account">{session.email}</Link>
                            <button type="button" onClick={endSession}>
                                Log out
                            </button>
                        </>
                    ) : (
                        <Link to="/login">Log in</Link>
                    )}
                </div>
            </header>

            <main className="app-content">
                <Outlet />
            </main>

        </div>
    );
}
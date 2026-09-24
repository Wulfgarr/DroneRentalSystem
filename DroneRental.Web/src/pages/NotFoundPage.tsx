import { Link } from 'react-router-dom';

export function NotFoundPage() {
    return (
        <section>
            <h2>404 - Page not found</h2>
            <p>The page you are looking for does not exist.</p>
            <Link to="/drones">Back to drones</Link>
        </section>
    );
}
import { useAuth } from '../auth/useAuth';

export function AccountPage() {
    const { session } = useAuth();

    if (session === null) {
        return null;
    }

    return (
        <section>
            <h1>My account</h1>

            <dl>
                <dt>Email</dt>
                <dd>{session.email}</dd>

                <dt>Role</dt>
                <dd>{session.role}</dd>
            </dl>
        </section>
    );
}
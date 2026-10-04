import { useState } from 'react';
import type { SubmitEvent } from 'react';
import { login } from '../api/authApi';
import { ApiError } from '../api/client';

export function LoginPage() {
    // Stores current state of the form fields.
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        //  Form handled in REACT without a page reload.
        event.preventDefault();

        if (isSubmitting) {
            return;
        }

        // We are starting a new attempt and clearing previous messages.
        setIsSubmitting(true);
        setError(null);
        setSuccessMessage(null);

        try {
            await login({
                email: email.trim(),
                password: password,
            });

            // Testing connection with API
            setSuccessMessage('Credentials verified successfully.');
            setPassword('');
        } catch (error: unknown) {
            if (error instanceof ApiError) {
                setError(error.message);
            } else {
                setError('Could not connect to the server. Please try again.');
            }
        } finally {
            // Unblock form either success or fail attempt.
            setIsSubmitting(false);
        }
        
    }

    return (
        <section>
            <h1>Log in</h1>

            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="login-email">Email</label>
                    <input
                        id="login-email"
                        name="email"
                        typeof="email"
                        autoComplete="username"
                        value={email}
                        onChange={(event) => setEmail(event.target.value)}
                        required
                        disabled={isSubmitting}
                    />
                </div>

                <div>
                    <label htmlFor="login-password">Password</label>
                    <input
                        id="login-password"
                        name="password"
                        type="password"
                        autoComplete="current-password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        required
                        disabled={isSubmitting}

                    />
                </div>

                <button type="submit" disabled={isSubmitting}>
                    {isSubmitting ? 'Logging in...' : 'Log in'}
                </button>
                
                {error && (
                    <p className="page-message error" role="alert">
                        {error}
                    </p>
                )}

                {successMessage && (
                    <p className="page-message" role="status">
                        {successMessage}
                    </p>
                )}
            </form>
        </section>
    );
}
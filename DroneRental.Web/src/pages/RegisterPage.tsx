import { useState } from 'react';
import { Link } from 'react-router-dom';
import type { SubmitEvent } from 'react';
import { register } from '../api/authApi';
import { ApiError } from '../api/client';

export function RegisterPage() {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault();

        // Don't send another request after sent request.
        if (isSubmitting) {
            return;
        }

        setError(null);
        setSuccessMessage(null);

        // firstname and lastname whitespace validation
        if (!firstName.trim() || !lastName.trim()) {
            setError('Please enter your first and last name.');
            return;
        }

        setIsSubmitting(true);

        try {
            await register({
                firstName: firstName.trim(),
                lastName: lastName.trim(),
                email: email.trim(),
                password: password,
            });

            setSuccessMessage('Account created successfully. You can now log in.');
            setPassword('');
        } catch (error: unknown) {
            if (error instanceof ApiError) {
                setError(error.message);
            } else {
                setError('Could not connect to the server. Please try again.');
            }
        } finally {
            // The form is available for edit now
            setIsSubmitting(false);
        }
    }

    return (
        <section>
            <h1>Create an account</h1>

            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="register-first-name">First name</label>
                    <input
                        id="register-first-name"
                        name="firstName"
                        type="text"
                        autoComplete="given-name"
                        value={firstName}
                        onChange={(event) => setFirstName(event.target.value)}
                        maxLength={100}
                        required
                        disabled={isSubmitting}
                    />
                </div>

                <div>
                    <label htmlFor="register-last-name">Last Name</label>
                    <input
                        id="register-last-name"
                        name="lastName"
                        type="text"
                        autoComplete="family-name"
                        value={lastName}
                        onChange={(event) => setLastName(event.target.value)}
                        maxLength={100}
                        required
                        disabled={isSubmitting}
                    />
                </div>

                <div>
                    <label htmlFor="register-email">Email</label>
                    <input
                        id="register-email"
                        name="email"
                        type="email"
                        autoComplete="username"
                        value={email}
                        onChange={(event) => setEmail(event.target.value)}
                        required
                        disabled={isSubmitting}
                    />
                </div>

                <div>
                    <label htmlFor="register-password">Password</label>
                    <input
                        id="register-password"
                        name="password"
                        type="password"
                        autoComplete="new-password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        minLength={6}
                        aria-describedby="register-password-help"
                        required
                        disabled={isSubmitting}
                    />
                    <p id="register-password-help">
                        Use at least 6 characters.
                    </p>
                </div>

                <button type="submit" disabled={isSubmitting}>
                    {isSubmitting ? 'Creating account...' : 'Create account'}
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

            <p>
                Already have an account? <Link to="/login">Log in</Link>
            </p>

        </section >
    );
}

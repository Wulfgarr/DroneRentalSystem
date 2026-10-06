export function getTokenExpiration(token: string): number | null {
    try {
        const parts = token.split('.');

        if (parts.length !== 3) {
            return null;
        }

        const payload = parts[1];

        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');

        const bytes = Uint8Array.from(
            atob(base64),
            (character) => character.charCodeAt(0)
        );

        const json = new TextDecoder().decode(bytes);
        const data: unknown = JSON.parse(json);

        if (typeof data !== 'object' || data === null) {
            return null;
        }

        if (!('exp' in data) || typeof data.exp !== 'number') {
            return null;
        }

        const expiration = data.exp * 1000;

        if (!Number.isFinite(expiration)) {
            return null;
        }

        return expiration;
    } catch {
        return null;
    }
}
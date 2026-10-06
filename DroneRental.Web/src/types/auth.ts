export type LoginRequest = {
    email: string;
    password: string;
};

export type RegisterRequest = {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
};

export type AuthResponse = {
    token: string;
    userId: number;
    email: string;
    role: string;
};

import { apiPost } from './client';
import type {
    AuthResponse,
    LoginRequest,
    RegisterRequest,
} from '../types/auth';

export function login(data: LoginRequest): Promise<AuthResponse> {
    return apiPost<AuthResponse, LoginRequest>('/api/auth/login', data);
}

export function register(data: RegisterRequest): Promise<AuthResponse> {
    return apiPost<AuthResponse, RegisterRequest>('/api/auth/register', data);
}

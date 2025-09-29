import type { User } from '@/types/User';

export interface IAuthenticationService {
  login(username: string, password: string): Promise<User | null>;
}
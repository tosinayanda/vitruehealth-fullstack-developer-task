import type { User } from '@/types/User';
import type { IAuthenticationService } from './IAuthenticationService';
import testUsers from '@/data/users';

export class LocalAuthenticationService implements IAuthenticationService {
  public async login(username: string, password: string): Promise<User | null> {
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 500));

    const user = testUsers.find(
      (u) => (u.username === username || u.email === username) && u.passwordHash === password
    );

    if (user) {
      const { passwordHash, ...userWithoutPassword } = user;
      return userWithoutPassword;
    }
    return null;
  }
}
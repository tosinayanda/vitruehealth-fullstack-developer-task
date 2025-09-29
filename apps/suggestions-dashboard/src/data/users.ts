import type { User } from '@/types/User';

// Mock user database
const testUsers: (User & { passwordHash: string })[] = [
  { id: 1, username: 'hsmanager@company.com', email: 'hsmanager@company.com', passwordHash: 'password123' },
  { id: 2, username: 'test@company.com', email: 'test@company.com', passwordHash: 'password123' },
];

export default testUsers;

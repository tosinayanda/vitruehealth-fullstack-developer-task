import type { BaseResponse, PaginatedResponse } from '@/types/BaseResponse';
import type { Employee } from '@/types/Suggestions';
import type { User } from '@/types/User';

export interface IEmployeeService {
  getAllEmployees(pageNumber: number, pageSize: number): Promise<PaginatedResponse<Employee>>;
  getEmployeeById(id: string): Promise<BaseResponse<Employee | null>>;
}
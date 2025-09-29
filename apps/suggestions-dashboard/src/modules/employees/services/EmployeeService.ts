import type { User } from '@/types/User';
import type {IEmployeeService } from './IEmployeeService';
import testUsers from '@/data/users';
import type { Employee } from '@/types/Suggestions';
import type { PaginatedResponse, BaseResponse } from '@/types/BaseResponse';
import axios from "axios"

export class EmployeeService implements IEmployeeService{
    baseUrl: string;
    constructor() {
        this.baseUrl = import.meta.env.VUE_APP_API_URL || import.meta.env.VITE_API_URL || 'http://localhost:3000';
    }
    async getAllEmployees(pageNumber: number, pageSize: number): Promise<PaginatedResponse<Employee>> {
        return await axios.get<PaginatedResponse<Employee>>(`${this.baseUrl}/employees`, {
            params: { pageNumber, pageSize }
        }).then(response => response.data);
    }
    async getEmployeeById(id: string): Promise<BaseResponse<Employee | null>> {
        return await axios.get<BaseResponse<Employee | null>>(`${this.baseUrl}/employees/${id}`).then(response => response.data);
    }

}
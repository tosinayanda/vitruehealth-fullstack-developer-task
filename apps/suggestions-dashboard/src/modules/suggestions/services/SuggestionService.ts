import type { User } from '@/types/User';
import type {ISuggestionService } from './ISuggestionService';
import testUsers from '@/data/users';
import type { Employee, Suggestion } from '@/types/Suggestions';
import type { PaginatedResponse, BaseResponse } from '@/types/BaseResponse';
import axios from "axios"

export class SuggestionService implements ISuggestionService{
    baseUrl: string;
    constructor() {
        this.baseUrl = import.meta.env.VUE_APP_API_URL || import.meta.env.VITE_API_URL || 'http://localhost:3000';
    }
    async createSuggestion(data: Suggestion): Promise<BaseResponse<Suggestion>> {
        return await axios.post<BaseResponse<Suggestion>>(`${this.baseUrl}/suggestions`, data).then(response => response.data);
    }
    async createSuggestionsBulk(data: Suggestion[]): Promise<BaseResponse<null>> {
        return await axios.post<BaseResponse<null>>(`${this.baseUrl}/suggestions/bulk`, { items: data }).then(response => response.data);
    }
    async updateSuggestion(id: string, data: Suggestion): Promise<BaseResponse<Suggestion | null>> {
        return await axios.post<BaseResponse<Suggestion | null>>(`${this.baseUrl}/suggestions/${id}`, data).then(response => response.data);
    }
    async updateSuggestionsBulk(data: Suggestion[]): Promise<BaseResponse<null>> {
        return await axios.post<BaseResponse<null>>(`${this.baseUrl}/suggestions/bulk`, { items: data }).then(response => response.data);
    }
    async getAllSuggestions(pageNumber: number, pageSize: number, source?: string, priority?:string, type?:string, status?:string): Promise<PaginatedResponse<Suggestion>> {
        return await axios.get<PaginatedResponse<Suggestion>>(`${this.baseUrl}/suggestions`, {
            params: { pageNumber, pageSize , source , priority, type, status}
        }).then(response => response.data);
    }
    async getSuggestionById(id: string): Promise<BaseResponse<Suggestion | null>> {
        return await axios.get<BaseResponse<Suggestion | null>>(`${this.baseUrl}/suggestions/${id}`).then(response => response.data);
    }

}
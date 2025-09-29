import type { BaseResponse, PaginatedResponse } from '@/types/BaseResponse';
import type { Employee, Suggestion } from '@/types/Suggestions';
import type { User } from '@/types/User';

export interface ISuggestionService {
  getAllSuggestions(pageNumber: number, pageSize: number): Promise<PaginatedResponse<Suggestion>>;
  getSuggestionById(id: string): Promise<BaseResponse<Suggestion | null>>;
  createSuggestion(data: Suggestion): Promise<BaseResponse<Suggestion>>;
  createSuggestionsBulk(data: Suggestion[]): Promise<BaseResponse<null>>;
  updateSuggestion(id: string, data: Suggestion): Promise<BaseResponse<Suggestion | null>>;
  updateSuggestionsBulk(data: Suggestion[]): Promise<BaseResponse<null>>;
}
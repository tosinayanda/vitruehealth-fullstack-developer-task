export interface PaginatedResponse<T> {
  pageNumber:   number;
  pageSize:     number;
  firstPage:    null;
  lastPage:     null;
  totalPages?:   number;
  totalRecords?: number;
  data:         T[] | null;
  errors:       null;
  message:      null;
  success:      boolean;
}

export interface BaseResponse<T> {
  data:         T | null;
  errors:       null;
  message:      null;
  success:      boolean;
}
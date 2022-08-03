
// [aznote] exact same name as Response headers Pagination.
export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T>{
    result: T;
    pagination: Pagination;
}
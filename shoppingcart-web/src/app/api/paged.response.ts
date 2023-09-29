export interface PagedResponse<T> {
    totalItems: number;
    pageNumber: number;
    pageSize: number;
    items: T[];
}

export interface PagedModel<T> {
    totalItems: number;
    pageNumber: number;
    pageSize: number;
    items: T[];
}

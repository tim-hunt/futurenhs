import { GetServerSidePropsContext } from 'next';
import { Group } from '@appTypes/group';
import { Content } from '@appTypes/content';
import { Pagination, ApiPagination } from '@appTypes/pagination';

export interface ServiceResponse<T> {
    data?: T;
    errors?: Array<Record<any>>;
    succeeded?: boolean;
    message?: string;
}

export interface ServicePaginatedResponse<T> {
    pagination?: Pagination;
    data?: T;
    errors?: Array<any>;
    succeeded?: boolean;
    message?: string;
}

export type ApiResponse<T> = any; // confirm with Tim

export interface ApiPaginatedResponse<T> extends ApiPagination {
    data?: T;
    errors?: Array<Record<any>>;
    succeeded?: boolean;
    message?: string;
}
 
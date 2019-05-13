import {ErrorType} from './enums/error-type';

export class ApiResponse<T> {
    isSuccess: boolean;
    message: string;
    errorType: ErrorType
    result: T;
}

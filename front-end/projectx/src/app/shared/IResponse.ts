export interface IResponse 
{
    isSuccess: boolean,
    error: IError
}

export interface IDataResponse<T> extends IResponse 
{
    data: T
}

export interface IError
{
    msessage: string,
    type: number,
    errorCode: number
}
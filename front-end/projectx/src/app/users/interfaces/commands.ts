export interface ICreateUserCommand
{
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    country: string,
    city: string,
    street: string
}
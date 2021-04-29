export interface User {
    id: number,
    firstName: string,
    lastName: string,
    emailAlreadyConfirmed: boolean,
    address: Address,
    roles: Role[]
}

export interface Address {
    country: string,
    city: string,
    street: string
}

export interface Role {
    id: number,
    name: string
}
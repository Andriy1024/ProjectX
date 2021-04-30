export class Token 
{
    access_token: string;
    refresh_token: string;
    expires_in: number;
    token_type: number;
    scope: string;

    constructor(token: string, 
                refresh_token: string,
                expires_in: number,
                token_type: number, 
                scope: string)
    {
        this.access_token = token;
        this.refresh_token = refresh_token;
        this.expires_in = expires_in;
        this.token_type = token_type;
        this.scope = scope;
    }
}
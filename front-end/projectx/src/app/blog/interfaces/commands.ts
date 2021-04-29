export interface ICreateArticleCommand {
    title: string,
    body: string
}

export interface IDeleteArticleCommand {
    id: number
}
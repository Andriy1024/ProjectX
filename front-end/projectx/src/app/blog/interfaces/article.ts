import { Guid } from "guid-typescript";

export interface IArticle {
    id: Guid,
    name: string
}

export interface IFullArticle extends IArticle {
    content: string
}

export interface ICreateArticleCommand {
    name: string,
    content: string
}
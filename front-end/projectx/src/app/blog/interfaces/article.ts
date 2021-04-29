export interface IArticle {
    id: number,
    title: string,
    body: string,
    createdAt: number,
    updatedAt: number,
    author: IAuthor
}

export interface IAuthor {
    id: number,
    firstName: string,
    lastName: string,
    email: string
}

export interface IFullArticle extends IArticle {
    comments: IComment[]
}

export interface IComment {
    articleId: number,
    text: string,
    createdAt: number,
    updatedAt: number,
    author: IAuthor
}
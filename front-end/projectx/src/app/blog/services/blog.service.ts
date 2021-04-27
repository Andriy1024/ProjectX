import { Injectable } from "@angular/core";
import { Guid } from "guid-typescript";
import { IArticle, IFullArticle, ICreateArticleCommand } from "../interfaces/article";

@Injectable({providedIn: 'root'})
export class BlogService {
    
    private _articles: IFullArticle[] = [
        {
            id: Guid.parse('4860c63b-4cbf-35f0-6ec4-68038347bf8a'),
            name: 'First Article',
            content: 'First Article content'
        },
        {
            id: Guid.parse('54e20cfb-a1bc-79ec-1ea3-c486dfe6b29d'),
            name: 'Second Article',
            content: 'First Article content'
        },
        {
            id: Guid.parse('9c46482f-7eff-c562-e515-603ee83c1f2c'),
            name: 'Third Article',
            content: 'First Article content'
        }
    ];

    public getArticles(): IArticle[] {
        return this._articles;
    }

    public getFullArticle(id: Guid): IFullArticle {
        const article = this._articles.find(a => a.id == id);
        
        if(!article) throw new Error(`Article with id: ${id} was not found.`);

        return article;
    }

    public deleteArticle(article: IArticle): void {
        this._articles = this._articles.filter(a => a.id !== article.id);
    }

    public createArticle(article: ICreateArticleCommand): void {
        this._articles.push(
            {
                id: Guid.create(),
                name: article.name,
                content: article.content
            });
    }
}
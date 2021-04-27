import { Injectable } from "@angular/core";
import { Guid } from "guid-typescript";
import { Article } from "../interfaces/article";

@Injectable({providedIn: 'root'})
export class BlogService {
    
    private _articles: Article[] = [
        {
        id: Guid.parse('4860c63b-4cbf-35f0-6ec4-68038347bf8a'),
        name: 'First Article'
        },
        {
        id: Guid.parse('54e20cfb-a1bc-79ec-1ea3-c486dfe6b29d'),
        name: 'Second Article'
        },
        {
        id: Guid.parse('9c46482f-7eff-c562-e515-603ee83c1f2c'),
        name: 'Third Article'
        }
    ];

    public getArticles(): Article[] {
        return this._articles;
    }

    public getArticle(id: Guid): Article {
        const article = this._articles.find(a => a.id == id);
        
        if(!article) throw new Error(`Article with id: ${id} was not found.`);

        return article;
    }

    public deleteArticle(article: Article): void {
        this._articles = this._articles.filter(a => a.id !== article.id);
    }

    public addArticle(article: Article): void {
        this._articles.push(article);
    }
}
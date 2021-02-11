﻿using ProjectX.Core;
using System;

namespace ProjectX.Blog.Domain
{
    public sealed partial class ArticleEntity
    {
        public sealed class Builder : EntityBuilder<ArticleEntity>
        {
            public Builder CreateArticle(string tittle, string body)
            {
                Entity = new ArticleEntity()
                {
                    Tittle = tittle,
                    Body = body
                };

                return this;
            }

            public Builder Author(AuthorEntity author)
            {
                EnsureCreated();
                Entity.Author = author;
                return this;
            }

            public override ArticleEntity Build()
            {
                if (Entity == null || Entity.Author == null || Entity.Tittle == null || Entity.Body == null)
                    throw new InvalidOperationException("Article not fully initialized.");

                Entity.CreatedAt = Entity.UpdatedAt = DateTime.UtcNow;
                Entity.AddDomainEvent(new ArticleCreated(Entity));
                return base.Build();
            }
        }
    }
}
using ProjectX.Core;
using System.Collections.Generic;

namespace ProjectX.Blog.Domain
{
    public sealed class AuthorEntity : Entity<long>
    {
        protected AuthorEntity() { }
        
        public AuthorEntity(long id, string firstName, string lastName, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }

        public ICollection<ArticleEntity> Articles = new List<ArticleEntity>();
        public ICollection<CommentEntity> Comments = new List<CommentEntity>();
    }
}

using System.Collections.Generic;

namespace ProjectX.Blog.Application
{
    public class ArticleDto
    {
        public AuthorDto Author { get; set; }
        public string Tittle { get; private set; }
        public string Body { get; private set; }
        public long CreatedAt { get; private set; }
        public long UpdatedAt { get; private set; }
        public IEnumerable<CommentDto> Comments { get; set; }
    }
}

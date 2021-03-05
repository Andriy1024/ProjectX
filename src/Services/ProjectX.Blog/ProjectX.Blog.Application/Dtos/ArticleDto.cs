using System.Collections.Generic;

namespace ProjectX.Blog.Application
{
    public class ArticleDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public AuthorDto Author { get; set; }
    }

    public class FullArticleDto : ArticleDto
    {
        public IEnumerable<CommentDto> Comments { get; set; }
    }
}

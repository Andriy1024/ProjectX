namespace ProjectX.Blog.Application
{
    public class CommentDto
    {
        public AuthorDto Author { get; set; }
        public long ArticleId { get; private set; }
        public string Text { get; private set; }
        public long CreatedAt { get; private set; }
        public long UpdatedAt { get; private set; }
    }
}

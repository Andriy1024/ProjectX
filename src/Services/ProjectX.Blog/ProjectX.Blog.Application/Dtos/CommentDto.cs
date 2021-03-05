namespace ProjectX.Blog.Application
{
    public class CommentDto
    {
        public long ArticleId { get; set; }
        public string Text { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public AuthorDto Author { get; set; }
    }
}

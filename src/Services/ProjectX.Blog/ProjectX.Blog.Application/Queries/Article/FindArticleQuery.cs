using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class FindArticleQuery : IQuery<FullArticleDto>
    {
        public FindArticleQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}

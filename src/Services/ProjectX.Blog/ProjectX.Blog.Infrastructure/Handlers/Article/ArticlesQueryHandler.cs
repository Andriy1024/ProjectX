using AutoMapper;
using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class ArticlesQueryHandler : IQueryHandler<ArticlesQuery, ArticleDto[]>
    {
        private readonly IMapper _mapper;
        private readonly IArticleRepository _repository;

        public ArticlesQueryHandler(IMapper mapper, IArticleRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IResponse<ArticleDto[]>> Handle(ArticlesQuery query, CancellationToken cancellationToken)
        {
            var response = await _repository.GetAsync(ArticleEntity.SpeceficationFactory
                                            .Search(query.Search)
                                            .WhereAuthor(query.AuthorId)
                                            .PublishedAt(query.PublishedAt)
                                            .GetSpecification(),
                                             pagination: query,
                                             ordering: query,
                                             cancellationToken);

            return ResponseFactory.Success(_mapper.Map<ArticleDto[]>(response.Data), response.Total);
        }
    }
}

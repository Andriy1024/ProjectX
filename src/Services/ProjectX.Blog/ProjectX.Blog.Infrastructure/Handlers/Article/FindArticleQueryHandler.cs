﻿using AutoMapper;
using ProjectX.Blog.Application;
using ProjectX.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class FindArticleQueryHandler : IQueryHandler<FindArticleQuery, ArticleDto>
    {
        readonly IMapper _mapper;
        readonly IArticleRepository _repository;

        public FindArticleQueryHandler(IMapper mapper, IArticleRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IResponse<ArticleDto>> Handle(FindArticleQuery query, CancellationToken cancellationToken)
        {
            var maybeArticle = await _repository.GetFullArticleAsync(a => a.Id == query.Id, cancellationToken);

            if (maybeArticle.IsFailed)
                return ResponseFactory.Failed<ArticleDto>(maybeArticle.Error);

            return ResponseFactory.Success(_mapper.Map<ArticleDto>(maybeArticle.Result));
        }
    }
}

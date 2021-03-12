using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Blog.Application;
using ProjectX.Core;
using ProjectX.Infrastructure.Controllers;

namespace ProjectX.Blog.API.Controllers
{
    [Route("api/articles")]
    public sealed class ArticlesController : BaseApiController
    {
        [HttpGet("{id:long:min(1)}")]
        [ProducesResponseType(typeof(IResponse<FullArticleDto>), 200)]
        public Task<IActionResult> FindArticleAsync([FromRoute] long id, CancellationToken cancellationToken)
            => Send(new FindArticleQuery(id), cancellationToken);

        [HttpGet]
        [ProducesResponseType(typeof(IPaginatedResponse<ArticleDto[]>), 200)]
        public Task<IActionResult> GetArticlesAsync([FromQuery] ArticlesQuery query, CancellationToken cancellationToken) 
            => Send(query, cancellationToken);

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(IResponse<ArticleDto>), 200)]
        public Task<IActionResult> CreateArticleAsync([FromBody] CreateArticleCommand command) 
            => Send(command);

        [HttpPut("titles")]
        [ProducesResponseType(typeof(IResponse<ArticleDto>), 200)]
        public Task<IActionResult> UpdateArticleTitleAsync([FromBody] UpdateArticleTitleCommand command) 
            => Send(command);

        [Authorize]
        [HttpPut("bodies")]
        [ProducesResponseType(typeof(IResponse<ArticleDto>), 200)]
        public Task<IActionResult> UpdateArticleBodyAsync([FromBody] UpdateArticleBodyCommand command) 
            => Send(command);

        [Authorize]
        [HttpDelete("{id:long:min(1)}")]
        [ProducesResponseType(typeof(IResponse), 200)]
        public Task<IActionResult> DeleteArticleAsync([FromRoute] long id)
            => Send(new DeleteArticleCommand(id));
    }
}
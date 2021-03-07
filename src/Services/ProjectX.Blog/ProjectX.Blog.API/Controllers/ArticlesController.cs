using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Blog.Application;
using ProjectX.Infrastructure.Controllers;

namespace ProjectX.Blog.API.Controllers
{
    [Route("api/articles")]
    public sealed class ArticlesController : BaseApiController
    {
        [HttpGet("{id:long:min(1)}")]
        public Task<IActionResult> FindArticleAsync([FromRoute] long id, CancellationToken cancellationToken)
            => Send(new FindArticleQuery(id), cancellationToken);

        [HttpGet]
        public Task<IActionResult> GetArticlesAsync([FromQuery] ArticlesQuery query, CancellationToken cancellationToken) 
            => Send(query, cancellationToken);

        [HttpPost]
        public Task<IActionResult> CreateArticleAsync([FromBody] CreateArticleCommand command) 
            => Send(command);

        [HttpPut("titles")]
        public Task<IActionResult> UpdateArticleTitleAsync([FromBody] UpdateArticleTitleCommand command) 
            => Send(command);

        [HttpPut("bodies")]
        public Task<IActionResult> UpdateArticleBodyAsync([FromBody] UpdateArticleBodyCommand command) 
            => Send(command);

        [HttpDelete("{id:long:min(1)}")]
        public Task<IActionResult> DeleteArticleAsync([FromRoute] long id)
           => Send(new DeleteArticleCommand(id));
    }
}
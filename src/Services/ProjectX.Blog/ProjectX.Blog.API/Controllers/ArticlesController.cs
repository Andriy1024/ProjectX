using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Blog.Application;
using ProjectX.Infrastructure.Controllers;

namespace ProjectX.Blog.API.Controllers
{
    [Route("api/articles")]
    public class ArticlesController : BaseApiController
    {
        [HttpGet("{id:long:min(1)}")]
        public async Task<IActionResult> FindArticleAsync([FromRoute] long id, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(new FindArticleQuery(id), cancellationToken));

        [HttpGet]
        public async Task<IActionResult> GetArticlesAsync([FromQuery] ArticlesQuery query, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(query, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> CreateArticleAsync([FromBody] CreateArticleCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut("titles")]
        public async Task<IActionResult> UpdateArticleTitleAsync([FromBody] UpdateArticleTitleCommand command)
           => MapResponse(await Mediator.Send(command));

        [HttpPut("bodies")]
        public async Task<IActionResult> UpdateArticleBodyAsync([FromBody] UpdateArticleBodyCommand command)
           => MapResponse(await Mediator.Send(command));

        [HttpDelete("{id:long:min(1)}")]
        public async Task<IActionResult> DeleteArticleAsync([FromRoute] long id)
           => MapResponse(await Mediator.Send(new DeleteArticleCommand(id)));
    }
}
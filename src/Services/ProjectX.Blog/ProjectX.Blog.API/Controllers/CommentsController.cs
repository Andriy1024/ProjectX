using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Blog.Application;
using ProjectX.Core;
using ProjectX.Infrastructure.Controllers;
using System.Threading.Tasks;
 
namespace ProjectX.Blog.API.Controllers
{
    [Route("api/comments")]
    [Authorize]   
    public sealed class CommentsController : BaseApiController
    {
        [HttpPost]
        [ProducesResponseType(typeof(IResponse<CommentDto>), 200)]
        public Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentCommand command) => Send(command);

        [HttpPut]
        [ProducesResponseType(typeof(IResponse<CommentDto>), 200)]

        public Task<IActionResult> CreateCommentAsync([FromBody] UpdateCommentCommand command) => Send(command);

        [HttpDelete("{id:long:min(1)}")]
        [ProducesResponseType(typeof(IResponse), 200)]
        public Task<IActionResult> DeleteCommentAsync([FromRoute] long id) => Send(new DeleteCommentCommand(id));
    }
}
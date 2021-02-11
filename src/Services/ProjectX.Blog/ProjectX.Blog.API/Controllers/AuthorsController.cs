using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Blog.Application;
using ProjectX.Infrastructure.Controllers;

namespace ProjectX.Blog.API.Controllers
{
    /// <summary>
    /// I don't use CQRS here, because this controller created for debugging.
    /// Author entity it is users from identity server, and  updates via rabbitmq.
    /// </summary>
    [Route("api/authors")]
    public class AuthorsController : ApiController
    {
        readonly IMapper _mapper;
        readonly IArticleRepository _repository;

        public AuthorsController(IMapper mapper, IArticleRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IActionResult> GetAuthorsAsync(CancellationToken cancellationToken)
        {
            return Ok(await _repository.GetAsync<AuthorDto>(_mapper, cancellationToken: cancellationToken));
        }
    }
}
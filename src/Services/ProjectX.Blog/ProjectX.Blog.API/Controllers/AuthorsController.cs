﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Blog.Application;
using ProjectX.Core;
using ProjectX.Infrastructure.Controllers;

namespace ProjectX.Blog.API.Controllers
{
    /// <summary>
    /// I don't use CQRS here, because this controller created for debugging.
    /// Author entity it is users from identity server, and  updates via rabbitmq.
    /// </summary>
    [Route("api/authors")]
    [Authorize]
    public sealed class AuthorsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IAuthorRepository _repository;

        public AuthorsController(IMapper mapper, IAuthorRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Response<AuthorDto[]>), 200)]
        public async Task<IActionResult> GetAuthorsAsync(CancellationToken cancellationToken)
        {
            return MapResponse(await _repository.GetAsync<AuthorDto>(_mapper, cancellationToken: cancellationToken));
        }
    }
}
using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [ApiController]
    [Produces("application/json", "application/xml")]
    [Route("api/v{version:apiVersion}/authors")]
    //[Route("api/v2.0/authors")]
    [ApiVersion("2.0")]
    public class AuthorsControllerV2 : ControllerBase
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IMapper mapper;

        public AuthorsControllerV2(IAuthorRepository authorRepository, IMapper mapper)
        {
            this.authorRepository = authorRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get the authors (V2)
        /// </summary>
        /// <returns>An ActionResult of type IEnumerable of Author</returns>
        /// <response code="200">Returns the list of authors</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            var authorsFromRepo = await authorRepository.GetAuthorsAsync();
            return Ok(mapper.Map<IEnumerable<Author>>(authorsFromRepo));
        }
    }
}

using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository authorsRepository;
        private readonly IMapper mapper;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper)
        {
            this.authorsRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            var authorsFromRepo = await authorsRepository.GetAuthorsAsync();
            return Ok(mapper.Map<IEnumerable<Author>>(authorsFromRepo));
        }

        [HttpGet("{authorId}")]
        public async Task<ActionResult<Author>> GetAuthor(Guid authorId)
        {
            var authorFromRepo = await authorsRepository.GetAuthorAsync(authorId);
            if(authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<Author>(authorFromRepo));
        }

        [HttpPut("{authorId}")]
        public async Task<ActionResult<Author>> UpdateAuthor(Guid authorId, AuthorForUpdate authorForUpdate)
        {
            var authorFromRepo = await authorsRepository.GetAuthorAsync(authorId);
            if(authorFromRepo == null)
            {
                return NotFound();
            }

            mapper.Map(authorForUpdate, authorFromRepo);

            //// update & save
            authorsRepository.UpdateAuthor(authorFromRepo);
            await authorsRepository.SaveChangesAsync();

            // return the author
            return Ok(mapper.Map<Author>(authorFromRepo));
        }

        [HttpPatch("{authorId}")]
        public async Task<ActionResult<Author>> UpdateAuthor(Guid authorId, [FromBody]JsonPatchDocument<AuthorForUpdate> patchDocument)
        {
            var authorFromRepo = await authorsRepository.GetAuthorAsync(authorId);
            if(authorFromRepo == null)
            {
                return NotFound();
            }

            // map to DTO to apply the patch to
            var author = mapper.Map<AuthorForUpdate>(authorFromRepo);
            patchDocument.ApplyTo(author, ModelState);

            // if there are errors when applying the patch the patch doc 
            // was badly formed  These aren't caught via the ApiController
            // validation, so we must manually check the modelstate and
            // potentially return these errors.
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            // map the applied changes on the DTO back into the entity
            mapper.Map(author, authorFromRepo);

            // update & save
            authorsRepository.UpdateAuthor(authorFromRepo);
            await authorsRepository.SaveChangesAsync();

            // return the author
            return Ok(mapper.Map<Author>(authorFromRepo));
        }
    }
}

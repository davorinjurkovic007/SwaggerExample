using AutoMapper;
using Library.API.Attributes;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Produces("application/json", "application/xml")]
    //[Route("api/authors/{authorId}/books")]
    //[Route("api/v{version:apiVersion}/authors/{authorId}/books")]
    [Route("api/v{version:apiVersion}/authors/{authorId}/books")]
    //[Route("api/v1.0/authors/{authorId}/books")]
    //[ApiExplorerSettings(GroupName = "LibraryOpenAPISpecificationBooks")]
    [ApiController]
    public class BooksController : ControllerBase
    { 
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public BooksController(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }
       
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(
        Guid authorId )
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var booksFromRepo = await _bookRepository.GetBooksAsync(authorId); 
            return Ok(_mapper.Map<IEnumerable<Book>>(booksFromRepo));
        }

        /// <summary>
        /// Get a book by id for a specific author
        /// </summary>
        /// <param name="authorId">The id of the book author</param>
        /// <param name="bookId">The id of the book</param>
        /// <returns>An ActionResult of type Book</returns>
        /// <response code="200">Returns the requested book</response>
        [HttpGet("{bookId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.marvin.book+json")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept, "application/json", "application/vnd.marvin.book+json")]
        public async Task<ActionResult<Book>> GetBook(
            Guid authorId,
            Guid bookId)
        {
            if (! await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _bookRepository.GetBookAsync(authorId, bookId);
            if (bookFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Book>(bookFromRepo));
        }

        /// <summary>
        /// Get a book by id for a specific author
        /// </summary>
        /// <param name="authorId">The id of the book author</param>
        /// <param name="bookId">The id of the book</param>
        /// <returns>An ActionResult of type BookWithConcatenatedAuthorName</returns>
        [HttpGet("{bookId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.marvin.bookwithconcatenatedauthorname+json")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept, "application/vnd.marvin.bookwithconcatenatedauthorname+json")]
        public async Task<ActionResult<BookWithConcatenatedAuthorName>> GetBookWithConcatenatedAuthorName(
            Guid authorId,
            Guid bookId)
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _bookRepository.GetBookAsync(authorId, bookId);
            if (bookFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookWithConcatenatedAuthorName>(bookFromRepo));
        }

        /// <summary>
        /// Create a book for a specific author
        /// </summary>
        /// <param name="authorId">The id of the book author</param>
        /// <param name="bookForCreation">The book to create</param>
        /// <returns>An ActionResult of type Book</returns>
        /// <response code="422">Validation error</response>
        [HttpPost()]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<Book>> CreateBook(
            Guid authorId,
            [FromBody] BookForCreation bookForCreation)
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookToAdd = _mapper.Map<Entities.Book>(bookForCreation);
            _bookRepository.AddBook(bookToAdd);
            await _bookRepository.SaveChangesAsync();

            return CreatedAtRoute(
                "GetBook",
                new { authorId, bookId = bookToAdd.Id },
                _mapper.Map<Book>(bookToAdd));
        }
    }
}

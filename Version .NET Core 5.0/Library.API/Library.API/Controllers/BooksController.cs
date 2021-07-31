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
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository bookRepository;
        private readonly IAuthorRepository authorRepository;
        private readonly IMapper mapper;

        public BooksController(IBookRepository bookRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            this.authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(Guid authorId)
        {
            if(!await authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var booksFromRepo = await bookRepository.GetBooksAsync(authorId);
            return Ok(mapper.Map<IEnumerable<Book>>(booksFromRepo));
        }

        [HttpGet("{bookId}")]
        public async Task<ActionResult<Book>> GetBook(Guid authorId, Guid bookId)
        {
            if(! await authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await bookRepository.GetBookAsync(authorId, bookId);
            if(bookFromRepo == null)
            { 
                return NotFound();
            }

            return Ok(mapper.Map<Book>(bookFromRepo));
        }

        [HttpPost()]
        public async Task<ActionResult<Book>> CreateBook( Guid authorId, [FromBody] BookForCreation bookForCreation)
        {
            if(!await authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookToAdd = mapper.Map<Entities.Book>(bookForCreation);
            bookRepository.AddBook(bookToAdd);
            await bookRepository.SaveChangesAsync();

            return CreatedAtRoute("GetBook", new { authorId, bookId = bookToAdd.Id }, mapper.Map<Book>(bookToAdd));
        }
    }
}

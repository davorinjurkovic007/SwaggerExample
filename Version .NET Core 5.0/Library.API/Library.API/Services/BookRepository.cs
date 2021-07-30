using Library.API.Contexts;
using Library.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public class BookRepository : IBookRepository, IDisposable
    {
        private LibraryContext context;

        public BookRepository(LibraryContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddBook(Book bookToAdd)
        {
            if(bookToAdd == null)
            {
                throw new ArgumentNullException(nameof(bookToAdd));
            }

            context.Add(bookToAdd);
        }

        public async Task<Book> GetBookAsync(Guid authorId, Guid bookId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if(bookId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(bookId));
            }

            return await context.Books.Include(b => b.Author).Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(Guid authorId)
        {
           if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await context.Books.Include(b => b.Author).Where(b => b.AuthorId == authorId).ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            // return true if 1 or more entities were changed
            return (await context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }
    }
}

using Library.API.Contexts;
using Library.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public class AuthorRepository : IAuthorRepository, IDisposable
    {
        private LibraryContext context;

        public AuthorRepository(LibraryContext context)
        {
            this.context = context;
        }

        public async Task<bool> AuthorExistsAsync(Guid authorId)
        {
            return await context.Authors.AnyAsync(a => a.Id == authorId);
        }

        public async Task<Author> GetAuthorAsync(Guid authorId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            return await context.Authors.ToListAsync();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
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

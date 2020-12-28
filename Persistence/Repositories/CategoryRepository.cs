using Microsoft.EntityFrameworkCore;
using Supermarket.API.Domain.Model;
using Supermarket.API.Domain.Repositories;
using Supermarket.API.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Category>> ListAsync()
        {
            return await context.Categories.ToListAsync();
        }
    }
}
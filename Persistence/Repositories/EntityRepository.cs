using Microsoft.EntityFrameworkCore;
using Supermarket.API.Domain.Repositories;
using Supermarket.API.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppAPI.Services
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext context;
        private DbSet<TEntity> _dbSet;

        public EntityRepository(AppDbContext context)
        {
            this.context = context;
        }

        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_dbSet == null)
                {
                    _dbSet = context.Set<TEntity>();
                }

                return _dbSet;
            }
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Entities.ToListAsync();
        }

        public TEntity GetOne(Func<TEntity, bool> where)
        {
            return Entities.FirstOrDefault(where);
        }
    }
}
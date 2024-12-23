using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Api.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbContext dbContext;

        protected DbSet<T> DbSet => dbContext.Set<T>();

        public GenericRepository(DbContext dbContext)
        {
            this.dbContext = dbContext?? throw new ArgumentNullException(nameof(dbContext));
        }

        #region Add Methods
        public virtual int Add(T entity)
        {
            DbSet.Add(entity);
            return dbContext.SaveChanges();
        }

        public virtual int Add(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
            return dbContext.SaveChanges();
        }

        public virtual async Task<int> AddASync(T entity)
        {
            await DbSet.AddAsync(entity);
            return await dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> AddAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
            return await dbContext.SaveChangesAsync();
        }

        #endregion

        #region Update Methods
        public virtual int Update(T entity)
        {
            DbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return dbContext.SaveChanges();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            DbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return await dbContext.SaveChangesAsync();
        }

        #endregion

        #region Delete Methods
        public int Delete(T entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
            return dbContext.SaveChanges();
        }

        public virtual int Delete(Guid id)
        {
            var entity = DbSet.Find(id);
            return Delete(entity);
        }

        public virtual Task<int> DeleteAsync(Guid id)
        {
            var entity = DbSet.Find(id);
            return DeleteAsync(entity);
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
            return await dbContext.SaveChangesAsync();
        }

        public bool DeleteRange(Expression<Func<T, bool>> predicate)
        {
            dbContext.RemoveRange(DbSet.Where(predicate));
            return dbContext.SaveChanges() > 0;
        }

        public Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate)
        {
            dbContext.RemoveRange(DbSet.Where(predicate));
            return dbContext.SaveChangesAsync().ContinueWith(t => t.Result > 0);
        }
        #endregion

        #region AddOrUpdate Methods
        public virtual int AddOrUpdate(T entity)
        {
            if (!DbSet.Local.Any(x => EqualityComparer<Guid>.Default.Equals(x.Id, entity.Id)))
                dbContext.Update(entity);
            
            return dbContext.SaveChanges();
        }

        public virtual Task<int> AddOrUpdateAsync(T entity)
        {
            if (!DbSet.Local.Any(x => EqualityComparer<Guid>.Default.Equals(x.Id, entity.Id)))
                dbContext.Update(entity);

            return dbContext.SaveChangesAsync();
        }
        #endregion

        #region Get Methods

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate, bool noTracking = true, params Expression<Func<T, object>>[] includes)
        {
            var query = DbSet.AsQueryable();
            if(predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includes);

            if (noTracking)
                query = query.AsNoTracking();
            return query;
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool noTracking = true, params Expression<Func<T, object>>[] includes)
        {
            T foundEntity = await DbSet.FirstOrDefaultAsync(predicate);

            if (foundEntity == null)
                return null;
            if (noTracking)
                dbContext.Entry(foundEntity).State = EntityState.Detached;

            foreach (Expression<Func<T, object>> include in includes)
            {
                dbContext.Entry(foundEntity).Reference(include).Load();
            }
            return foundEntity;
        }

        public virtual async Task<List<T>> GetAll(bool noTracking = true)
        {
            IQueryable<T> query = DbSet.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<T, object>>[] includes)
        {
            T foundEntity = await DbSet.FindAsync(id);
            if (foundEntity == null)
                return null;
            if(noTracking)
                dbContext.Entry(foundEntity).State = EntityState.Detached;

            foreach(Expression<Func<T, object>> include in includes)
            {
                dbContext.Entry(foundEntity).Reference(include).Load();
            }
            return foundEntity;
        }

        public virtual async Task<List<T>> GetList(Expression<Func<T, bool>> predicate, bool noTracking = true, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includes);

            if (orderBy != null)
                query = orderBy(query);

            if (noTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool noTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet;

            if (predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includes);

            if (noTracking)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync();

        }
        #endregion

        #region Bulk Methods
        public virtual async Task BulkAdd(IEnumerable<T> entities)
        {
            if (entities==null || !entities.Any())
                await Task.CompletedTask;

            await DbSet.AddRangeAsync(entities);

            await dbContext.SaveChangesAsync();
        }

        public virtual async Task BulkDelete(Expression<Func<T, bool>> predicate)
        {

            dbContext.RemoveRange(DbSet.Where(predicate));

            await dbContext.SaveChangesAsync();
        }

        public virtual async Task BulkDelete(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                await Task.CompletedTask;

             dbContext.RemoveRange(entities);

            await dbContext.SaveChangesAsync();
        }

        public virtual async Task BulkDeleteById(IEnumerable<Guid> ids)
        {
            if(ids==null || !ids.Any())
              await Task.CompletedTask;

            dbContext.RemoveRange(DbSet.Where(x => ids.Contains(x.Id)));
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task BulkUpdate(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                await Task.CompletedTask;

            entities.ToList().ForEach(entity =>
            {
                DbSet.Attach(entity);
                dbContext.Entry(entity).State = EntityState.Modified;
            });
            await dbContext.SaveChangesAsync();
        }
        #endregion


        public IQueryable<T> AsQueryable()
        {
            return DbSet.AsQueryable();
        }


        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            if(includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }



    }
}

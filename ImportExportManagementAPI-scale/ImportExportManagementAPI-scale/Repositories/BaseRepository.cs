using ImportExportManagementAPI_scale.DBContext;
using ImportExportManagementAPI_scale.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected IEManagementContext _dbContext;
        protected DbSet<TEntity> _dbSet;

        public BaseRepository()
        {
            _dbContext = new IEManagementContext();
            _dbSet = _dbContext.Set<TEntity>();
        }
        public void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public List<TEntity> GetAll()
        {
            List<TEntity> entities = _dbSet.ToList();
            return entities;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            List<TEntity> entities = await _dbSet.ToListAsync();
            return entities;
        }

        public TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public async Task<TEntity> GetByIDAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}

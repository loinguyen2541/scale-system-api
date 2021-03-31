using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    interface IRepository<TEntity> where TEntity : class
    {
        List<TEntity> GetAll();
        TEntity GetByID(object id);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> GetByIDAsync(object id);

        void Delete(TEntity entityToDelete);
        void Delete(object id);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
        void Save();
        Task SaveAsync();
    }
}

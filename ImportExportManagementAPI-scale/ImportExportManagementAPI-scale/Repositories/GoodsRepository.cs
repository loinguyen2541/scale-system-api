using ImportExportManagement_API.Models;
using ImportExportManagementAPI_scale.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    public class GoodsRepository : BaseRepository<Goods>
    {
        public float GetGoodCapacity()
        {
            return _dbSet.SingleOrDefault().QuantityOfInventory;
        }

        public async void UpdateQuantityOfGood(int id, float weight, TransactionType type)
        {
            Goods goods = _dbSet.Find(id);
            if (weight < 0) weight = weight * -1;
            if (type.Equals(TransactionType.Import)) goods.QuantityOfInventory = goods.QuantityOfInventory + weight;
            if (type.Equals(TransactionType.Export)) goods.QuantityOfInventory = goods.QuantityOfInventory - weight;
            _dbContext.Entry(goods).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await SaveAsync();
        }
    }
}

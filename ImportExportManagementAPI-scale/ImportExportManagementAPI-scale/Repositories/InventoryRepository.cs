using ImportExportManagementAPI_scale.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>
    {
        public async Task<Inventory> CheckExistDateRecord(DateTime dateRecord)
    {
        var getDate = dateRecord.Date;
        Task<Inventory> inventory = _dbSet.Where(i => i.RecordedDate.Equals(getDate)).FirstOrDefaultAsync();
        if (inventory.Result == null)
        {
            //chua co thi tao moi

            GoodsRepository goodsRepository = new GoodsRepository();
            float goodsQuantity = goodsRepository.GetGoodCapacity();
            Inventory newInventory = new Inventory { RecordedDate = dateRecord, OpeningStock = goodsQuantity };
            Insert(newInventory);
            await SaveAsync();
            return newInventory;
        }
        //co roi thi tra ve
        return inventory.Result;
    }
}
}

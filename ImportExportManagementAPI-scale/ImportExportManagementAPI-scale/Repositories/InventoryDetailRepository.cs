using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI_scale.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    public class InventoryDetailRepository : BaseRepository<InventoryDetail>
    {
        public async Task<bool> UpdateInventoryDetail(DateTime recordDate, Transaction trans)
        {
            InventoryRepository inventRepo = new InventoryRepository();
            var date = recordDate.Date;
            int transType = (int)trans.TransactionType;

            //check da co phieu nhao kho vao ngay chua va detail cua type nay da co chua
            Inventory checkInventoryExisted = await inventRepo.CheckExistDateRecord(date);
            //check type nay partner co chua
            Task<InventoryDetail> checkTypeExisted = CheckExistedDetailType(trans.PartnerId, transType, checkInventoryExisted.InventoryId);
            //neu inven da co && type chua co => tao moi
            if (checkTypeExisted.Result == null)
            {
                //nếu chưa có => tạo mới
                AddNewInventoryDetail(trans, checkInventoryExisted);
                return true;
            }
            else if (checkTypeExisted.Result != null)
            {
                //neu inventory da co && type da co roi => update weight
                UpdateInventoryDetailByType(trans, checkTypeExisted.Result);
                return true;
            }
            return false;
        }

        private async Task<InventoryDetail> CheckExistedDetailType(int partnerId, int type, int inventoryId)
        {
            //get list detail of partner
            List<InventoryDetail> listDetailOfPartner = await GetPartnerInventoryDetail(partnerId, inventoryId);
            if (listDetailOfPartner != null && listDetailOfPartner.Count > 0)
            {
                foreach (var item in listDetailOfPartner)
                {
                    if ((int)item.Type == type)
                    {
                        //co roi
                        return item;
                    }
                }
            }
            return null;
        }

        //get list detail by partner
        private async Task<List<InventoryDetail>> GetPartnerInventoryDetail(int partnerId, int inventoryId)
        {
            List<InventoryDetail> details = new List<InventoryDetail>();
            details = await _dbSet.Where(d => d.PartnerId == partnerId && d.InventoryId == inventoryId).ToListAsync();
            return details;
        }

        private async void AddNewInventoryDetail(Transaction trans, Inventory inventory)
        {
            InventoryDetail detail = new InventoryDetail { GoodsId = trans.GoodsId, InventoryId = inventory.InventoryId, PartnerId = trans.PartnerId };
            if (trans.TransactionType.Equals(TransactionType.Import))
            {
                detail.Type = (int)InventoryDetailType.Import;
            }
            else
            {
                detail.Type = (int)InventoryDetailType.Export;
            }

            float totalWeight = trans.WeightIn - trans.WeightOut;
            if (totalWeight < 0) totalWeight = totalWeight * -1;
            detail.Weight = totalWeight;
            Insert(detail);
            try
            {
                await SaveAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void UpdateInventoryDetailByType(Transaction trans, InventoryDetail detail)
        {
            float totalWeight = trans.WeightIn - trans.WeightOut;
            if (totalWeight < 0) totalWeight = totalWeight * -1;
            detail.Weight = detail.Weight + totalWeight;
            Update(detail);
            try
            {
                await SaveAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

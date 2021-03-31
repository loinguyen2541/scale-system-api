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
    public class TimeTemplateItemRepository : BaseRepository<TimeTemplateItem>
    {
        public async Task<List<TimeTemplateItem>> GetAppliedItem()
        {
            return await _dbSet.Include(i => i.Schedules.Where(s => s.ScheduleStatus == (int)ScheduleStatus.Approved)).Where(i => i.TimeTemplate.TimeTemplateStatus == (int)TimeTemplateStatus.Applied).ToListAsync();
        }

        public void UpdateCurrent(TransactionType type, float registeredWeight, int id)
        {
            TimeTemplateItem timeTemplateItem = _dbSet.Find(id);
            float targetItemCapacity = 0;
            List<TimeTemplateItem> timeTemplateItems = null;
            if (timeTemplateItem != null)
            {
                timeTemplateItems = _dbSet
                .Where(i => i.TimeTemplateId == timeTemplateItem.TimeTemplateId)
                .OrderBy(p => p.ScheduleTime).ToList();
            }
            if (timeTemplateItems != null || timeTemplateItems.Count > 0)
            {
                if (type == TransactionType.Export)
                {
                    targetItemCapacity = timeTemplateItem.Inventory - registeredWeight;
                    UpdateCapacityExport(timeTemplateItems, timeTemplateItem.ScheduleTime, targetItemCapacity, registeredWeight);
                }
                else
                {
                    targetItemCapacity = timeTemplateItem.Inventory + registeredWeight;
                    UpdateCapacityImport(timeTemplateItems, timeTemplateItem.ScheduleTime, targetItemCapacity, registeredWeight);
                }
                _dbContext.SaveChanges();
            }
        }
        public void UpdateCapacityExport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float targetInventory, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime < targetTime && item.Inventory < targetInventory)
                {
                    item.Inventory = targetInventory;
                }
                else if (item.ScheduleTime == targetTime)
                {
                    item.Inventory = targetInventory;
                }
                else
                {
                    item.Inventory -= registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        public void UpdateCapacityImport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float targetInventory, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime == targetTime)
                {
                    item.Inventory = targetInventory;
                }
                else if (item.ScheduleTime > targetTime)
                {
                    item.Inventory += registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
    }
}

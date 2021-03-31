using ImportExportManagementAPI.Models;
using ImportExportManagementAPI_scale.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI_scale.Repositories
{
    public class ScheduleRepository : BaseRepository<Schedule> 
    {
        public async Task<List<Schedule>> GetBookedScheduleInDate(int partnerId)
        {
            var current = DateTime.Now.Date;
            List<Schedule> listSchedule = new List<Schedule>();
            listSchedule = await _dbSet.OrderBy(s => s.RegisteredWeight).Where(s => s.PartnerId == partnerId && s.ScheduleDate.Date == current && !s.UpdatedBy.Equals("Update action")).ToListAsync();
            return listSchedule;
        }

        public async Task<bool> UpdateActualWeight(int partnerId, float weight)
        {
            List<Schedule> listScheduled = await GetBookedScheduleInDate(partnerId);
            if (listScheduled != null && listScheduled.Count > 0)
            {
                //approximate
                if (weight < 0) weight = weight * -1;
                float deviation = (float)(weight * 0.1);
                float max = weight + deviation;
                float min = weight - deviation;

                foreach (var item in listScheduled)
                {
                    if (min < item.RegisteredWeight && item.RegisteredWeight < max)
                    {
                        item.ActualWeight = weight;
                        item.ScheduleStatus = (int)ScheduleStatus.Success;
                        Update(item);
                        await SaveAsync();
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}

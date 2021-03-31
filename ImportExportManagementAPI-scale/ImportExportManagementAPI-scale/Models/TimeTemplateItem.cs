using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class TimeTemplateItem
    {
        public TimeTemplateItem()
        {
            Schedules = new HashSet<Schedule>();
        }

        public int TimeTemplateItemId { get; set; }
        public float Inventory { get; set; }
        public TimeSpan ScheduleTime { get; set; }
        public int TimeTemplateId { get; set; }

        public virtual TimeTemplate TimeTemplate { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public float RegisteredWeight { get; set; }
        public float? ActualWeight { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TransactionType { get; set; }
        public int ScheduleStatus { get; set; }
        public string UpdatedBy { get; set; }
        public int PartnerId { get; set; }
        public int GoodsId { get; set; }
        public int TimeTemplateItemId { get; set; }

        public virtual Good Goods { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual TimeTemplateItem TimeTemplateItem { get; set; }
    }
}

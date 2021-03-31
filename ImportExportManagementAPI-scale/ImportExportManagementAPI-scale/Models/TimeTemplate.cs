using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class TimeTemplate
    {
        public TimeTemplate()
        {
            TimeTemplateItems = new HashSet<TimeTemplateItem>();
        }

        public int TimeTemplateId { get; set; }
        public string TimeTemplateName { get; set; }
        public DateTime ApplyingDate { get; set; }
        public int TimeTemplateStatus { get; set; }

        public virtual ICollection<TimeTemplateItem> TimeTemplateItems { get; set; }
    }
}

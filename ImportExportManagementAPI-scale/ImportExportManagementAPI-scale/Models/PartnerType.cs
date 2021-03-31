using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class PartnerType
    {
        public PartnerType()
        {
            Partners = new HashSet<Partner>();
        }

        public int PartnerTypeId { get; set; }
        public string PartnerTypeName { get; set; }

        public virtual ICollection<Partner> Partners { get; set; }
    }
}

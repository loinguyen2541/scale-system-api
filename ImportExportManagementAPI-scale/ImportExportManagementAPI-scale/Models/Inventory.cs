using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class Inventory
    {
        public Inventory()
        {
            InventoryDetails = new HashSet<InventoryDetail>();
        }

        public int InventoryId { get; set; }
        public float OpeningStock { get; set; }
        public DateTime RecordedDate { get; set; }

        public virtual ICollection<InventoryDetail> InventoryDetails { get; set; }
    }
}

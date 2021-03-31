using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class InventoryDetail
    {
        public int InventoryDetailId { get; set; }
        public float Weight { get; set; }
        public int? Type { get; set; }
        public int InventoryId { get; set; }
        public int GoodsId { get; set; }
        public int PartnerId { get; set; }

        public virtual Goods Goods { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual Partner Partner { get; set; }
    }
}

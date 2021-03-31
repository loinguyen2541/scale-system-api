using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class Good
    {
        public Good()
        {
            InventoryDetails = new HashSet<InventoryDetail>();
            Schedules = new HashSet<Schedule>();
            Transactions = new HashSet<Transaction>();
        }

        public int GoodsId { get; set; }
        public string GoodName { get; set; }
        public float QuantityOfInventory { get; set; }
        public int GoodsStatus { get; set; }

        public virtual ICollection<InventoryDetail> InventoryDetails { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}

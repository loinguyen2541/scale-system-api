using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public float WeightIn { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public float WeightOut { get; set; }
        public bool? IsScheduled { get; set; }
        public int TransactionType { get; set; }
        public int TransactionStatus { get; set; }
        public int PartnerId { get; set; }
        public string IdentityCardId { get; set; }
        public int GoodsId { get; set; }

        public virtual Goods Goods { get; set; }
        public virtual IdentityCard IdentityCard { get; set; }
        public virtual Partner Partner { get; set; }
    }
}

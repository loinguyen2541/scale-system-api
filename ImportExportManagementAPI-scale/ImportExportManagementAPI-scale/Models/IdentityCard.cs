using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class IdentityCard
    {
        public IdentityCard()
        {
            Transactions = new HashSet<Transaction>();
        }

        public string IdentityCardId { get; set; }
        public int IdentityCardStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PartnerId { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}

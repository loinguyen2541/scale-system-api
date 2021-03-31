using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class Partner
    {
        public Partner()
        {
            IdentityCards = new HashSet<IdentityCard>();
            InventoryDetails = new HashSet<InventoryDetail>();
            Schedules = new HashSet<Schedule>();
            Transactions = new HashSet<Transaction>();
        }

        public int PartnerId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int PartnerStatus { get; set; }
        public string Username { get; set; }
        public int PartnerTypeId { get; set; }

        public virtual PartnerType PartnerType { get; set; }
        public virtual Account UsernameNavigation { get; set; }
        public virtual ICollection<IdentityCard> IdentityCards { get; set; }
        public virtual ICollection<InventoryDetail> InventoryDetails { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}

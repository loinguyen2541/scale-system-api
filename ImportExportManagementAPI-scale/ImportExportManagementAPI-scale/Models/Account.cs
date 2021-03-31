using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExportManagementAPI_scale.Models
{
    public partial class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public int RoleId { get; set; }
        public string Token { get; set; }

        public virtual Role Role { get; set; }
        public virtual Partner Partner { get; set; }
    }
}

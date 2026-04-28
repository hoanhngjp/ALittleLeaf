using System;
using System.Collections.Generic;

namespace ALittleLeaf.Api.Models
{
    public partial class User
    {
        public long UserId { get; set; }

        public string UserEmail { get; set; } = null!;

        public string? UserPassword { get; set; }

        public string UserFullname { get; set; } = null!;

        public bool UserSex { get; set; }

        public DateOnly UserBirthday { get; set; }

        public bool UserIsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string UserRole { get; set; } = null!;

        // "local" for password-based accounts, "google" for SSO accounts
        public string? AuthProvider { get; set; }

        public string? GoogleId { get; set; }

        public virtual ICollection<AddressList> AddressLists { get; set; } = new List<AddressList>();

        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }

}
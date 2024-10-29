using System;
using System.Collections.Generic;

namespace ALittleLeaf.Models;

public partial class User
{
    public long UserId { get; set; }

    public string UserEmail { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string UserFullname { get; set; } = null!;

    public bool UserSex { get; set; }

    public DateOnly UserBirthday { get; set; }

    public bool UserIsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UserRole { get; set; } = null!;

    public virtual ICollection<AddressList> AddressLists { get; set; } = new List<AddressList>();

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}

using System;
using System.Collections.Generic;

namespace onlineStore.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsApproved { get; set; }

    public bool IsBlocked { get; set; }

    public int RoleId { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Role Role { get; set; } = null!;
}

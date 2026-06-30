using System;
using System.Collections.Generic;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    

    public string? Role { get; set; }
   
    public bool IsTemporaryPassword { get; set; } = false;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}


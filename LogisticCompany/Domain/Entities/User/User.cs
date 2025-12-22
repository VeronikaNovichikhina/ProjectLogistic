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
  
}


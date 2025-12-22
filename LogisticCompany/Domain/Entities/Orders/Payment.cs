using System;
using System.Collections.Generic;

namespace LogisticCompany.Domain.Entities.Orders;

public partial class Payment
{
    public int PaymentsId { get; set; }

    public int OrdersId { get; set; }
    public int PaymentMethodId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string Amount { get; set; }

    public virtual Order Orders { get; set; } = null!;
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
  
}

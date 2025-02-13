using System;
using System.Collections.Generic;

namespace Exam.Entities;

public partial class BookTicket
{
    public int BookTicketId { get; set; }

    public string TicketCode { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string TicketName { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public int Price { get; set; }

    public int Qty { get; set; }

    public virtual Ticket TicketCodeNavigation { get; set; } = null!;
}

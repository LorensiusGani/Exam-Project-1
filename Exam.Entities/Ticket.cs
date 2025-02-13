using System;
using System.Collections.Generic;

namespace Exam.Entities;

public partial class Ticket
{
    public string TicketCode { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string TicketName { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public int Price { get; set; }

    public int SeatNumber { get; set; }

    public int Quota { get; set; } 

    public virtual ICollection<BookTicket> BookTickets { get; set; } = new List<BookTicket>();
}

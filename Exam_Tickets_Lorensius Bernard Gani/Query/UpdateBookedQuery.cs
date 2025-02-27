﻿using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using MediatR;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Query
{
    public class UpdateBookedQuery : IRequest<List<BookedTicketDetails>>
    {
        public int BookTicketID { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }

    }
}

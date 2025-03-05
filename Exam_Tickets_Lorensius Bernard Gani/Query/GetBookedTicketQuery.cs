using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using MediatR;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Query
{
    public class GetBookedTicketQuery : IRequest<List<BookedTicket>>
    {
            public int BookTicketId { get; set; }
            public string TicketCode { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public string TicketName { get; set; } = string.Empty;
            public DateTime EventDate { get; set; }
            public int Price { get; set; }
            public int Qty { get; set; }

    }
}

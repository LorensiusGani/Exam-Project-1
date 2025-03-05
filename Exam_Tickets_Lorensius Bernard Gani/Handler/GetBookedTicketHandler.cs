using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Handler
{
    public class GetBookedTicketHandler : IRequestHandler<GetBookedTicketQuery, List<BookedTicket>>
    {
        private readonly AccelokaContext _context;

        public GetBookedTicketHandler(AccelokaContext context)
        {
            _context = context;
        }

        public async Task<List<BookedTicket>> Handle(GetBookedTicketQuery request, CancellationToken cancellationToken)
        {
            var dataBookedTicket = await _context.BookTickets
                .Select(Q => new BookedTicket
                {
                    BookTicketId = Q.BookTicketId,
                    TicketCode = Q.TicketCode,
                    CategoryName = Q.CategoryName,
                    TicketName = Q.TicketName,
                    EventDate = Q.EventDate,
                    Price = Q.Price,
                    Qty = Q.Qty
                })
                .ToListAsync(cancellationToken);

            return dataBookedTicket;
        }

    }
}

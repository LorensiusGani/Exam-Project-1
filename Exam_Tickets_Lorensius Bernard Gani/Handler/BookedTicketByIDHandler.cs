using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Handler
{
    public class BookedTicketByIDHandler : IRequestHandler<BookedTicketByIDQuery, List<BookedCategoryModel>>
    {
        private readonly AccelokaContext _context;

        public BookedTicketByIDHandler(AccelokaContext context)
        {
            _context = context;
        }

        public async Task<List<BookedCategoryModel>> Handle(BookedTicketByIDQuery request, CancellationToken cancellationToken)
        {
            var bookedTickets = await _context.BookTickets
               .Where(ticket => ticket.BookTicketId == request.bookedTicketID)
               .GroupBy(ticket => ticket.CategoryName)
               .Select(group => new BookedCategoryModel
               {
                   QtyProperty = group.Sum(x => x.Qty),
                   CategoryName = group.Key,
                   Tickets = group.Select(ticket => new DetailsBookedModel
                   {
                       TicketCode = ticket.TicketCode,
                       TicketName = ticket.TicketName,
                       EventDate = ticket.EventDate.ToString("dd-MM-yyyy HH:mm:ss")
                   }).ToList()
               }).ToListAsync();

            if (bookedTickets == null || !bookedTickets.Any())
            {
                return null;
            }

            return bookedTickets;
        }
    }
}

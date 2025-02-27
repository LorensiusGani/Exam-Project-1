using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Handler
{
    public class DeleteBookedHandler : IRequestHandler<DeleteBookedTicketQuery, List<BookedTicketDetails>>
    {

        private readonly AccelokaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteBookedHandler(AccelokaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<BookedTicketDetails>> Handle(DeleteBookedTicketQuery request, CancellationToken cancellationToken)
        {
            var bookedTicketDB = await _context.BookTickets
                .FirstOrDefaultAsync(x => x.BookTicketId == request.BookTicketID && x.TicketCode == request.TicketCode);

            if (bookedTicketDB == null)
            {
                var error = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Booked Ticket Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"BookedTicketId {request.BookTicketID} or TicketCode {request.TicketCode} not found.",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                throw new ProblemDetailsException(error);
            }

            if (bookedTicketDB.Qty < request.Qty)
            {
                var error = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid Quantity",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = $"The quantity {request.Qty} is too high. Available quantity: {bookedTicketDB.Qty}.",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                throw new ProblemDetailsException(error);
            }

            bookedTicketDB.Qty -= request.Qty;

            if (bookedTicketDB.Qty == 0)
            {
                _context.BookTickets.Remove(bookedTicketDB);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var remainingBookedTicket = await _context.BookTickets
                .Where(y => y.BookTicketId == request.BookTicketID)
                .Select(y => new BookedTicketDetails
                {
                    TicketCode = y.TicketCode,
                    TicketName = y.TicketName,
                    Quantity = y.Qty,
                    CategoryName = y.CategoryName
                }).ToListAsync(cancellationToken);

            return remainingBookedTicket;
        }




    }
}

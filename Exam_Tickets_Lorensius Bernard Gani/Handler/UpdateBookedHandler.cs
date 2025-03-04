using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Handler
{
    public class UpdateBookedHandler : IRequestHandler<UpdateBookedQuery, List<BookedTicketDetails>>
    {
        private readonly  AccelokaContext _context;

        public UpdateBookedHandler( AccelokaContext context)
        {
            _context = context;
        }

        public async Task<List<BookedTicketDetails>> Handle(UpdateBookedQuery request, CancellationToken cancellationToken)
        {
            var bookedTicketDB = await _context.BookTickets
                .FirstOrDefaultAsync(x => x.BookTicketId == request.BookTicketID, cancellationToken);

            if (bookedTicketDB == null)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Booked Ticket Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"BookedTicketId {request.BookTicketID} not found."
                });
            }

            var bookticket = await _context.BookTickets
                .FirstOrDefaultAsync(y => y.TicketCode == request.TicketCode, cancellationToken);

            if(bookticket == null)
        {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Ticket Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Ticket with Code {request.TicketCode} Not Found."
                });
            }

            if (request.Quantity < 1)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid Quantity",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Quantity must be greater than 0."
                });
            }

            int oldQuota = bookedTicketDB.Qty;
            int newQuota = request.Quantity;
            int remainingQuota = oldQuota - newQuota;


            var ticket = await _context.Tickets
                .Where(Q => Q.TicketCode == request.TicketCode)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Ticket Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Ticket with Code {request.TicketCode} Not Found in Tickets."
                });
            }

            if (newQuota > oldQuota)
            {
                ticket.Quota -= Math.Abs(remainingQuota);
            }

            if (newQuota < oldQuota)
            {
                ticket.Quota += Math.Abs(remainingQuota);
            }

            bookedTicketDB.TicketCode = request.TicketCode;
            bookedTicketDB.Qty = request.Quantity;

            await _context.SaveChangesAsync();

            var remainingBookedTickets = await _context.BookTickets
                .Where(y => y.BookTicketId == request.BookTicketID)
                .Select(y => new BookedTicketDetails
                {
                    TicketCode = y.TicketCode,
                    TicketName = y.TicketName,
                    Quantity = y.Qty,
                    CategoryName = y.CategoryName
                }).ToListAsync(cancellationToken);

            return remainingBookedTickets;
        }

    }
}

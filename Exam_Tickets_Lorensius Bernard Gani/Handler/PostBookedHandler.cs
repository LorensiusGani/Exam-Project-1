using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Handler
{
    public class PostBookedHandler : IRequestHandler<PostBookedTicketQuery, BookedTicketResponse>
    {
        private readonly AccelokaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostBookedHandler(AccelokaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BookedTicketResponse> Handle(PostBookedTicketQuery request, CancellationToken cancellationToken)
        {
            if (request.Tickets == null || !request.Tickets.Any())
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Request Body can't be null or empty",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                });
            }

            var ticketCodes = request.Tickets.Select(t => t.TicketCode).ToList();
            var dataTickets = await _context.Tickets
                .Where(t => ticketCodes.Contains(t.TicketCode))
                .ToListAsync(cancellationToken);

            foreach (var ticketReq in request.Tickets)
            {
                var ticket = dataTickets.FirstOrDefault(t => t.TicketCode == ticketReq.TicketCode);

                if (ticket == null)
                {
                    throw new ProblemDetailsException(new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/404",
                        Title = "Ticket Not Found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"Ticket with Code {ticketReq.TicketCode} Not Found",
                        Instance = _httpContextAccessor.HttpContext?.Request.Path
                    });
                }

                if (ticket.Quota < ticketReq.Quantity)
                {
                    throw new ProblemDetailsException(new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/400",
                        Title = "Insufficient Quota",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = $"Not enough quota for ticket {ticketReq.TicketCode}",
                        Instance = _httpContextAccessor.HttpContext?.Request.Path
                    });
                }

                if (ticket.EventDate <= DateTime.UtcNow)
                {
                    throw new ProblemDetailsException(new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/400",
                        Title = "Event Date Passed",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = $"Event date for ticket {ticketReq.TicketCode} has already passed.",
                        Instance = _httpContextAccessor.HttpContext?.Request.Path
                    });
                }

                ticket.Quota -= ticketReq.Quantity;

                var bookedTicket = new BookTicket
                {
                    TicketCode = ticket.TicketCode,
                    Qty = ticketReq.Quantity,
                    CategoryName = ticket.CategoryName,
                    TicketName = ticket.TicketName,
                    Price = ticket.Price,
                    EventDate = DateTime.UtcNow
                };

                _context.BookTickets.Add(bookedTicket);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var groupedTickets = dataTickets
                .Where(t => request.Tickets.Any(rt => rt.TicketCode == t.TicketCode))
                .GroupBy(t => t.CategoryName)
                .Select(g => new BookedTicketCategoryResponse
                {
                    CategoryName = g.Key,
                    SummaryPrice = g.Sum(t => t.Price * request.Tickets.First(rt => rt.TicketCode == t.TicketCode).Quantity),
                    Tickets = g.Select(t => new BookedTicketDetailResponse
                    {
                        TicketCode = t.TicketCode,
                        TicketName = t.TicketName,
                        Price = t.Price
                    }).ToList()
                }).ToList();

            return new BookedTicketResponse
            {
                PriceSummary = groupedTickets.Sum(g => g.SummaryPrice),
                TicketsPerCategories = groupedTickets
            };

        }
    }
}

using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Services
{
    public class BookedTicketServices
    {
        private readonly AccelokaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookedTicketServices(AccelokaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<List<BookedTicket>> Get()
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
                    
                }).ToListAsync();

            return dataBookedTicket;
        }

        public async Task<List<BookedCategoryModel>> GetBookedTicked(int bookedTicketId)
        {
            var bookedTickets = await _context.BookTickets
                .Where(ticket => ticket.BookTicketId == bookedTicketId)
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
            
            if(!bookedTickets.Any())
            {
                return null;
            }

            return bookedTickets;
        }

        public async Task<BookedTicketResponse> PostBookedTicket(BookedTicketInputModel request)
        {
            if (request == null || request.Tickets == null || !request.Tickets.Any())
            {
                var error = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Request Body can't be null",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Request Body can't be null",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path

                };
                throw new ProblemDetailsException(error);
            }

            var dataTicket = await _context.Tickets
                .Where(x => request.Tickets.Select(y => y.TicketCode).Contains(x.TicketCode))
                .ToListAsync();

            foreach (var ticketReq in request.Tickets)
            {

                var ticket = dataTicket.FirstOrDefault(y => y.TicketCode == ticketReq.TicketCode);

                if (ticket == null)
                {
                    var error = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/404",
                        Title = "Ticket Not Found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"Ticket with Code {ticketReq.TicketCode} Not Found",
                        Instance = _httpContextAccessor.HttpContext?.Request.Path

                    };
                    throw new ProblemDetailsException(error);
                }

                if (ticket.Quota < ticketReq.Quantity)
                {
                    var error = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/404",
                        Title = "Not Quota Available",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"No Quota Available for ticket {ticketReq.TicketCode}",
                        Instance = _httpContextAccessor.HttpContext?.Request.Path

                    };
                    throw new ProblemDetailsException(error);
                }

                if (ticket.EventDate <= DateTime.UtcNow)
                {
                    var error = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/404",
                        Title = "Event date already passed",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"Event date for ticket {ticketReq.TicketCode} has already passed.",
                        Instance = _httpContextAccessor.HttpContext?.Request.Path

                    };
                    throw new ProblemDetailsException(error);
                }
                
                ticket.Quota -= ticketReq.Quantity;

                var bookedTicket = new BookedTicket
                {
                    TicketCode = ticket.TicketCode,
                    Qty = ticketReq.Quantity,
                    CategoryName = ticket.CategoryName,
                    TicketName = ticket.TicketName,
                    Price = ticket.Price,
                    EventDate = DateTime.UtcNow
                };

                var bookTicket = new BookTicket
                {
                    TicketCode = bookedTicket.TicketCode,
                    Qty = bookedTicket.Qty,
                    CategoryName = bookedTicket.CategoryName,
                    TicketName= bookedTicket.TicketName,
                    Price = bookedTicket.Price,
                    EventDate = bookedTicket.EventDate
                };

                _context.BookTickets.Add(bookTicket);
            }
            await _context.SaveChangesAsync();

            var groupTicket = dataTicket
            .Where(y => request.Tickets.Any(a => a.TicketCode == y.TicketCode))
            .GroupBy(x => x.CategoryName)
            .Select(g => new BookedTicketCategoryResponse
                {
                 CategoryName = g.Key,
                 SummaryPrice = g.Sum(x => x.Price * request.Tickets.First(y => y.TicketCode == x.TicketCode).Quantity),
                 Tickets = g.Select(t => new BookedTicketDetailResponse
                {
                 TicketCode = t.TicketCode,
                 TicketName = t.TicketName,
                 Price = t.Price
                }).ToList()
            }).ToList();

            return new BookedTicketResponse
            {
                PriceSummary = groupTicket.Sum(x => x.SummaryPrice),
                TicketsPerCategories = groupTicket
            };

        }

        public async Task<List<BookedTicketDetails>> Delete(int bookTicketID, string ticketCode,int Qty)
        {
            var bookedTicketDB = await _context.BookTickets
            .FirstOrDefaultAsync(x => x.BookTicketId == bookTicketID && x.TicketCode == ticketCode);


            if (bookedTicketDB == null)
            {
                var error = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Booked Ticket Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"BookedTicketId {bookTicketID} or TicketCode {ticketCode} not found.",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                throw new ProblemDetailsException(error);
            }

           if(bookedTicketDB.Qty < Qty)
            {
                var error = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid Quantity",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = $"The quantity {Qty} is too high. Available quantity: {bookedTicketDB.Qty}.",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                throw new ProblemDetailsException(error);
            }

            bookedTicketDB.Qty -= Qty;

            if(bookedTicketDB.Qty == 0)
            {
                _context.BookTickets.Remove(bookedTicketDB);
            }

            await _context.SaveChangesAsync();

            var remainingBookedTicket = await _context.BookTickets
                 .Where(y => y.BookTicketId == bookTicketID)
                .Select(y => new BookedTicketDetails
                {
                    TicketCode = y.TicketCode,
                    TicketName = y.TicketName,
                    Quantity = y.Qty,
                    CategoryName = y.CategoryName
                }).ToListAsync();

            return remainingBookedTicket;

        }

        public async Task<List<BookedTicketDetails>> Update(int bookTicketID, BookedTicketInputItemModel ticketUpdates)
        {
                var bookedTicketDB = await _context.BookTickets
                .FirstOrDefaultAsync(x => x.BookTicketId == bookTicketID);

                if (bookedTicketDB == null)
                {
                var error = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Booked Ticket Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"BookedTicketId {bookTicketID} not found.",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                throw new ProblemDetailsException(error);
            }

                var bookticket = await _context.BookTickets
                    .FirstOrDefaultAsync(y => y.TicketCode ==  ticketUpdates.TicketCode);

                if(bookticket == null)
                {
                    var error = new ProblemDetails
                     {
                    Type = "https://httpstatuses.com/404",
                    Title = "Ticket Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Ticket with Code {bookedTicketDB.TicketCode} Not Found",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path

                    };
                    throw new ProblemDetailsException(error);
                 }

                if(ticketUpdates.Quantity < 1)
                {
                var error = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/404",
                    Title = "Quantity must be higher than 1",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"No Quantity is available",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path

                };
                throw new ProblemDetailsException(error);
                }

                int oldQuota = bookticket.Qty;
                int newQuota = ticketUpdates.Quantity;
                int RemainingQuota = oldQuota - newQuota;

                var ticket = await _context.Tickets
                .FirstOrDefaultAsync(x => x.TicketCode == ticketUpdates.TicketCode);

                if(newQuota > oldQuota)
                {
                    ticket.Quota -= Math.Abs(RemainingQuota);
                }

                if (newQuota < oldQuota)
                {
                    ticket.Quota += Math.Abs(RemainingQuota);
                }

                bookedTicketDB.TicketCode = ticketUpdates.TicketCode;
                bookedTicketDB.Qty = ticketUpdates.Quantity;


                await _context.SaveChangesAsync();

                var remainingBookedTicket = await _context.BookTickets
                .Where(y => y.BookTicketId == bookTicketID)
               .Select(y => new BookedTicketDetails
               {
                   TicketCode = y.TicketCode,
                   TicketName = y.TicketName,
                   Quantity =  y.Qty,
                   CategoryName = y.CategoryName
               }).ToListAsync();

               return remainingBookedTicket;   


        }

    }
}

using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Services
{
    public class TicketsServices
    {
        private readonly AccelokaContext _context;
        public TicketsServices(AccelokaContext context)
        {
            _context = context;
        }

        public async Task<TicketsResponseModel> GetTickets(
            string? categoryName, string? ticketCode, string? ticketName, int? maxPrice,
            DateTime? minEventDate, DateTime? maxEventDate, string? orderBy, string? orderState,
            int page = 1, int pageSize = 10)
        {
            var queryGetTickets = _context.Tickets.AsQueryable();

            if(!string.IsNullOrEmpty(categoryName) ) 
            {
                queryGetTickets = queryGetTickets.Where(x => x.CategoryName.Contains(categoryName));
            }
            if (!string.IsNullOrEmpty(ticketCode))
            {
                queryGetTickets = queryGetTickets.Where(y => y.TicketCode.Contains(ticketCode));    
            }
            if (!string.IsNullOrEmpty(ticketName))
            {
                queryGetTickets = queryGetTickets.Where(z => z.TicketName.Contains(ticketName));
            }
            if (maxPrice.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(a => a.Price <= maxPrice.Value);
            }

            if(minEventDate.HasValue && maxEventDate.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(a => a.EventDate >= minEventDate.Value && a.EventDate <= maxEventDate.Value);
            }
            else if (minEventDate.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(q => q.EventDate >= minEventDate.Value);
            }
            else if (maxEventDate.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(q => q.EventDate <= maxEventDate.Value);
            }

            //Order State
            bool isDescending = orderState?.ToLower() == "desc";
            orderBy = orderBy?.ToLower() ?? "ticketcode";

            // Kalau mau descending
            queryGetTickets = orderBy switch
            {
                "categoryname" => isDescending ? queryGetTickets.OrderByDescending(q => q.CategoryName) : queryGetTickets.OrderBy(q => q.CategoryName),
                "ticketcode" => isDescending ? queryGetTickets.OrderByDescending(q => q.TicketCode) : queryGetTickets.OrderBy(q => q.TicketCode),
                "ticketname" => isDescending ? queryGetTickets.OrderByDescending(q => q.TicketName) : queryGetTickets.OrderBy(q => q.TicketName),
                "price" => isDescending ? queryGetTickets.OrderByDescending(q => q.Price) : queryGetTickets.OrderBy(q => q.Price),
                "eventdate" => isDescending ? queryGetTickets.OrderByDescending(q => q.EventDate) : queryGetTickets.OrderBy(q => q.EventDate),
                _ => queryGetTickets.OrderBy(q => q.TicketCode) 
            };

            int totalTickets = await queryGetTickets.CountAsync();
            var pagination = queryGetTickets.Skip((page - 1) * pageSize).Take(pageSize);

            var dataTicket = await pagination.Select(Q => new TicketsModel
            {
                EventDate = Q.EventDate.ToString("dd-MM-yyyy HH:mm:ss"),
                Quota = Q.Quota,
                TicketCode = Q.TicketCode,
                TicketName = Q.TicketName,
                CategoryName = Q.CategoryName,
                 Price = Q.Price
            }).ToListAsync();

            return new TicketsResponseModel
            {
                Tickets = dataTicket,
                TotalTickets = totalTickets
            };
        }

    }
}

using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Handler
{

    public class GetTicketHandler : IRequestHandler<TicketQuery,TicketsResponseModel<TicketsModel>>
    {
        private readonly AccelokaContext _context;

        public GetTicketHandler(AccelokaContext context)
        {
            _context = context;
        }

        public async Task<TicketsResponseModel<TicketsModel>> Handle(TicketQuery request, CancellationToken cancellationToken)
        {
            var queryGetTickets = _context.Tickets.AsQueryable();

            if (!string.IsNullOrEmpty(request.categoryName))
            {
                queryGetTickets = queryGetTickets.Where(x => x.CategoryName.Contains(request.categoryName));
            }
            if (!string.IsNullOrEmpty(request.ticketCode))
            {
                queryGetTickets = queryGetTickets.Where(y => y.TicketCode.Contains(request.ticketCode));
            }
            if (!string.IsNullOrEmpty(request.ticketName))
            {
                queryGetTickets = queryGetTickets.Where(z => z.TicketName.Contains(request.ticketName));
            }
            if (request.maxPrice.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(a => a.Price <= request.maxPrice.Value);
            }
            if (request.minEventDate.HasValue && request.maxEventDate.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(a => a.EventDate >= request.minEventDate.Value && a.EventDate <= request.maxEventDate.Value);
            }
            else if (request.minEventDate.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(q => q.EventDate >= request.minEventDate.Value);
            }
            else if (request.maxEventDate.HasValue)
            {
                queryGetTickets = queryGetTickets.Where(q => q.EventDate <= request.maxEventDate.Value);
            }

            // Order Data
            bool isDescending = request.orderState?.ToLower() == "desc";
            string orderBy = request.orderBy?.ToLower() ?? "ticketcode";

            queryGetTickets = orderBy switch
            {
                "categoryname" => isDescending ? queryGetTickets.OrderByDescending(q => q.CategoryName) : queryGetTickets.OrderBy(q => q.CategoryName),
                "ticketcode" => isDescending ? queryGetTickets.OrderByDescending(q => q.TicketCode) : queryGetTickets.OrderBy(q => q.TicketCode),
                "ticketname" => isDescending ? queryGetTickets.OrderByDescending(q => q.TicketName) : queryGetTickets.OrderBy(q => q.TicketName),
                "price" => isDescending ? queryGetTickets.OrderByDescending(q => q.Price) : queryGetTickets.OrderBy(q => q.Price),
                "eventdate" => isDescending ? queryGetTickets.OrderByDescending(q => q.EventDate) : queryGetTickets.OrderBy(q => q.EventDate),
                _ => queryGetTickets.OrderBy(q => q.TicketCode)
            };

            int totalTickets = await queryGetTickets.CountAsync(cancellationToken);
            var pagination = queryGetTickets.Skip((request.page - 1) * request.pageSize).Take(request.pageSize);

            var dataTicket = await pagination.Select(Q => new TicketsModel
            {
                EventDate = Q.EventDate.ToString("dd-MM-yyyy HH:mm:ss"),
                Quota = Q.Quota,
                TicketCode = Q.TicketCode,
                TicketName = Q.TicketName,
                CategoryName = Q.CategoryName,
                Price = Q.Price
            }).ToListAsync();

            return new TicketsResponseModel<TicketsModel>
            {
                Tickets = dataTicket,
                TotalTickets = totalTickets
            };
        }


    }
}

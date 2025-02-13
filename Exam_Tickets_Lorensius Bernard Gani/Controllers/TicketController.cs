using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam_Tickets_Lorensius_Bernard_Gani.Controllers
{

    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketsServices _context;
        public TicketController(TicketsServices context)
        {
            _context = context;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> Get(string? categoryName, string? ticketCode, string? ticketName, int? maxPrice,
            DateTime? minEventDate, DateTime? maxEventDate, string? orderBy, string? orderState,
            int page = 1, int pageSize = 10)
        {
            if (page <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid page parameter",
                    Detail = "Page number must be greater than zero.",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }

            if (pageSize <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid pageSize parameter",
                    Detail = "Page size must be greater than zero.",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }

            if (maxPrice.HasValue && maxPrice <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid maxPrice parameter",
                    Detail = "Max price must be greater than zero.",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }

            if (!string.IsNullOrEmpty(orderState) && !orderState.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
                !orderState.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Invalid orderState parameter",
                    Detail = "Order state must be either 'asc' or 'desc'",
                    Status = 400,
                    Instance = HttpContext.Request.Path
                });
            }
            var data = await _context.GetTickets(categoryName, ticketCode, ticketName, maxPrice,
                minEventDate, maxEventDate, orderBy, orderState, page, pageSize);
            return Ok(data);
        }

        [HttpGet("download-pdf-ticket")]
        public async Task<IActionResult> DownloadReport(string? categoryName, string? ticketCode, string? ticketName, int? maxPrice,
            DateTime? minEventDate, DateTime? maxEventDate, string? orderBy, string? orderState)
        {
            var reports = await _context.GetTickets(categoryName, ticketCode, ticketName, maxPrice,
                minEventDate, maxEventDate, orderBy, orderState, 1, int.MaxValue);
            if (reports == null || reports.Tickets.Count == 0)
            {
                return BadRequest("No data to generate this report");
            }

            string directory = @"D:\DOWNLOAD DARI CHROME";

            var pdfFile = GeneratePDFTicketsReport.GenerateTicketReport(reports.Tickets, directory);
            return Ok($"File successfully saved at: {pdfFile}");

        }

    }

}

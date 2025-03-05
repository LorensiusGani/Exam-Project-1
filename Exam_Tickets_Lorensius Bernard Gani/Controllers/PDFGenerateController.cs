using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam_Tickets_Lorensius_Bernard_Gani.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class PDFGenerateController : ControllerBase
    {
        private readonly TicketsServices _context;
        private readonly BookedTicketServices _services;

        public PDFGenerateController(TicketsServices context, BookedTicketServices services)
        {
            _context = context;
            _services = services;

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
            if (!System.IO.File.Exists(pdfFile))
            {
                return NotFound("File not found.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfFile);

            return File(fileBytes, "application/pdf", "TicketReport.pdf");

        }

        [HttpGet("download-pdf-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> DownloadReportBooked(int bookedTicketId)
        {
            var report = await _services.GetBookedTickedID(bookedTicketId);

            if (report == null || report.Count == 0)
            {
                return BadRequest($"No data found for BookedTicketID: {bookedTicketId}");
            }
            string directory = @"D:\DOWNLOAD DARI CHROME";

            var pdfFILE = GeneratePDFBookedTicketReport.GenerateBookedTicketReport(report, directory);

            return Ok($"File successfully saved at: {pdfFILE}");
        }

    }
}

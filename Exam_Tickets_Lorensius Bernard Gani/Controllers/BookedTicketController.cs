using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam_Tickets_Lorensius_Bernard_Gani.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly BookedTicketServices _services;

        public BookedTicketController(BookedTicketServices services)
        {
            _services = services;
        }

        [HttpGet("get-booked-ticked/{BookedTicketId}")]
        public async Task<IActionResult> GetBookedTicked(int BookedTicketId)
        {
            var booked = await _services.GetBookedTicked();
            return Ok(booked);
        }

        [HttpGet("download-pdf-booked-ticket")]
        public async Task<IActionResult> DownloadReportBooked()
        {
            var report = await _services.Get();
            if (report == null || report.Count == 0)
            {
                return BadRequest("No data to generate this report");
            }
            string directory = @"D:\DOWNLOAD DARI CHROME";
            var pdfFILE = GeneratePDFBookedTicketReport.GenerateBookedTicketReport(report, directory);
            return Ok($"File successfully saved at: {pdfFILE}");
        }

        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTicket([FromBody] BookedTicketInputModel request)
        {
            if(ModelState.IsValid == false)
            {
                var error = new ProblemDetails
                {
                    Title = "Data Not Valid",
                    Type = "https://httpstatuses.com/404",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Data is not valid",
                    Instance = HttpContext.Request.Path

                };
                throw new ProblemDetailsException(error);
            }

            var response = await _services.PostBookedTicket(request);
            return Ok(response);
        }

        [HttpDelete("revoke-ticket/{BookedTicketId}/{KodeTicket}/{Qty}.")]
        public async Task<IActionResult> Delete(int BookedTicketId, string KodeTicket, int Qty)
        {
            var response = await _services.Delete(BookedTicketId, KodeTicket,  Qty);
            return Ok(response);
        }

        [HttpPut("edit-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Put(int BookedTicketId, [FromBody] BookedTicketInputItemModel ticketUpdates)
        {
            var response = await _services.Update(BookedTicketId, ticketUpdates);
            return Ok(response);
        }
    }
}

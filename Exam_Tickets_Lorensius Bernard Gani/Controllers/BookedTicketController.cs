using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using Exam_Tickets_Lorensius_Bernard_Gani.Validator;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam_Tickets_Lorensius_Bernard_Gani.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        //private readonly BookedTicketServices _services;
        private readonly IMediator _mediator;
        private readonly IValidator<BookedTicketByIDQuery> _getvalidator;
        private readonly IValidator<PostBookedTicketQuery> _postvalidator;
        private readonly IValidator<DeleteBookedTicketQuery> _delvalidator;
        private readonly IValidator<UpdateBookedQuery> _updatevalidator;

        public BookedTicketController(IMediator mediator, IValidator<BookedTicketByIDQuery> getvalidator,IValidator<PostBookedTicketQuery> postvalidator, IValidator<DeleteBookedTicketQuery> delvalidator , IValidator<UpdateBookedQuery> updatevalidator)
        {
            //_services = services;
            _mediator = mediator;
            _getvalidator = getvalidator;
            _postvalidator = postvalidator;
            _delvalidator = delvalidator;
            _updatevalidator = updatevalidator;
        }

        [HttpGet("get-booked-ticked/{BookedTicketId}")]
        public async Task<IActionResult> GetBookedTicked(int BookedTicketId)
        {
           
            var query = new BookedTicketByIDQuery(BookedTicketId);
            var validation = await _getvalidator.ValidateAsync(query);

            if (!validation.IsValid)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 404,
                    Type = "https://httpstatuses.com/404",
                    Title = "BookedTicketID must be higher than 0",
                    Detail = "Your BookedTicketID must be higher than 0",
                    Instance = HttpContext.Request.Path
                });
            }

            var bookeddata = await _mediator.Send(query);

            if(bookeddata == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = 404,
                    Type = "https://httpstatuses.com/404",
                    Title = "No Data available",
                    Detail = $"No data in {BookedTicketId}",
                    Instance = HttpContext.Request.Path
                };
                return NotFound(problemDetails);
            }
            return Ok(bookeddata);  
        }

        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTicket([FromBody] PostBookedTicketQuery request)
        {
            var validation = await _postvalidator.ValidateAsync(request);

            if(!validation.IsValid)
            {

                var errors = validation.Errors.Select(err => err.ErrorMessage).ToList();

                var error = new ProblemDetails
                {
                    Title = "Data Not Valid",
                    Type = "https://httpstatuses.com/404",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Data is not valid",
                    Instance = HttpContext.Request.Path

                };
                return BadRequest(error);
            }

            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("revoke-ticket/{BookedTicketId}/{KodeTicket}/{Qty}")]
        public async Task<IActionResult> Delete(int BookedTicketId, string KodeTicket, int Qty)
        {
            var request = new DeleteBookedTicketQuery
            {
                BookTicketID = BookedTicketId,
                TicketCode = KodeTicket,
                Qty = Qty
            };

            var validation = await _delvalidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(err => err.ErrorMessage).ToList();

                var error = new ProblemDetails
                {
                    Title = "Data Not Valid",
                    Type = "https://httpstatuses.com/400",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred.",
                    Instance = HttpContext.Request.Path,
                    Extensions = { { "errors", errors } }
                };

                return BadRequest(error);
            }

            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut("edit-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Put(int BookedTicketId, [FromBody] BookedTicketInputItemModel ticketUpdates)
        {
            var request = new UpdateBookedQuery
            {
                BookTicketID = BookedTicketId,
                TicketCode = ticketUpdates.TicketCode,
                Quantity = ticketUpdates.Quantity
            };

            var validation = await _updatevalidator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(err => err.ErrorMessage).ToList();

                var error = new ProblemDetails
                {
                    Title = "Data Not Valid",
                    Type = "https://httpstatuses.com/400",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred.",
                    Instance = HttpContext.Request.Path,
                    Extensions = { { "errors", errors } }
                };

                return BadRequest(error);
            }

            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}

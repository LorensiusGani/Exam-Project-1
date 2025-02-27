using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using FluentValidation;
using iText.Kernel.Geom;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exam_Tickets_Lorensius_Bernard_Gani.Controllers
{

    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<TicketQuery> _validator;


        public TicketController(IMediator mediator , IValidator<TicketQuery> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> Get(string? categoryName, string? ticketCode, string? ticketName, int? maxPrice,
            DateTime? minEventDate, DateTime? maxEventDate, string? orderBy, string? orderState,
            int page = 1, int pageSize = 10)
        {
            var query = new TicketQuery(categoryName, ticketCode, ticketName, maxPrice, minEventDate, maxEventDate, orderBy, orderState, page, pageSize);

            var validation = await _validator.ValidateAsync(query);
            if (!validation.IsValid)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 404,
                    Type = "https://httpstatuses.com/404",
                    Title = "No Data Available",
                    Instance = HttpContext.Request.Path,
                });
            }


            var data = await _mediator.Send(query);
            return Ok(data);
        }

    }

}

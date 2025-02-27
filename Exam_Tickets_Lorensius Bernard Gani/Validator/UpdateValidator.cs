using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using FluentValidation;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Validator
{
    public class UpdateValidator : AbstractValidator<UpdateBookedQuery>
    {
        public UpdateValidator()
        {
            RuleFor(x => x.BookTicketID)
            .GreaterThan(0).WithMessage("Booked Ticket ID must be greater than 0.");

            RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("Ticket Code is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}

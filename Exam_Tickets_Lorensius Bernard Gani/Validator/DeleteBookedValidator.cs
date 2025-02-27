using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using FluentValidation;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Validator
{
    public class DeleteBookedValidator : AbstractValidator<DeleteBookedTicketQuery>
    {
        public DeleteBookedValidator()
        {
            RuleFor(x => x.BookTicketID)
                .GreaterThan(0).WithMessage("BookTicketID must be greater than 0");

            RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("TicketCode is required");

            RuleFor(x => x.Qty)
                .GreaterThan(0).WithMessage("Qty must be greater than 0");
        }


    }
}

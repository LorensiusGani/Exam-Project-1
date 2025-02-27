using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using FluentValidation;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Validator
{
    public class BookedTicketByIDValidator : AbstractValidator<BookedTicketByIDQuery>
    {
        public BookedTicketByIDValidator()
        {
            RuleFor(x => x.bookedTicketID).GreaterThan(0)
                .WithMessage("BookedTicketID must be greater than 0");
                ;
        }
    }
}

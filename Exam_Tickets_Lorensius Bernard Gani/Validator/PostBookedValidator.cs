using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using FluentValidation;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Validator
{
    public class PostBookedValidator : AbstractValidator<PostBookedTicketQuery>
    {
        public PostBookedValidator()
        {
            RuleFor(q => q.Tickets)
           .NotEmpty().WithMessage("Tickets list cannot be empty");

            RuleForEach(q => q.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(t => t.TicketCode)
                    .NotEmpty().WithMessage("TicketCode cannot be empty");

                ticket.RuleFor(t => t.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            });
        }

    }
}

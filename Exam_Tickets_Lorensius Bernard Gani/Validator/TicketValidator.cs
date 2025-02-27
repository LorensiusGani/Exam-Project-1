using Exam_Tickets_Lorensius_Bernard_Gani.Query;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Validator
{
    public class TicketValidator : AbstractValidator<TicketQuery>
    {
        public TicketValidator()
        {
            RuleFor(q => q.page)
            .GreaterThan(0).WithMessage("Page harus lebih dari 0");

            RuleFor(q => q.pageSize)
                .GreaterThan(0).WithMessage("PageSize harus lebih dari 0");

            RuleFor(q => q.maxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("MaxPrice harus lebih dari atau sama dengan 0");

            RuleFor(q => q.orderBy)
                .Must(orderBy => string.IsNullOrEmpty(orderBy) || new[] { "categoryname", "ticketcode", "ticketname", "price", "eventdate" }.Contains(orderBy.ToLower()))
                .WithMessage("OrderBy tidak valid");

            RuleFor(q => q.orderState)
                .Must(orderState => string.IsNullOrEmpty(orderState) || orderState.ToLower() == "asc" || orderState.ToLower() == "desc")
                .WithMessage("OrderState hanya boleh 'asc' atau 'desc'");

            RuleFor(q => q.maxEventDate)
                .GreaterThanOrEqualTo(q => q.minEventDate)
                .When(q => q.minEventDate.HasValue && q.maxEventDate.HasValue)
                .WithMessage("MaxEventDate harus lebih besar atau sama dengan MinEventDate");
        }
    }
}

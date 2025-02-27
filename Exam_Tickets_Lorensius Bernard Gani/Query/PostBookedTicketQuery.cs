using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using MediatR;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Query
{
    public class PostBookedTicketQuery : IRequest<BookedTicketResponse>
    {
        public List<BookedTicketInputItemModel> Tickets { get; set; } = new();
    }
}

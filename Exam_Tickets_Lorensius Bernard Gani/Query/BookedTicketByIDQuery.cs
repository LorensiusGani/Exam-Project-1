using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using MediatR;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Query
{
    public class BookedTicketByIDQuery : IRequest<List<BookedCategoryModel>>
    {
        public int bookedTicketID { get; set; }
        public int qtyProperty { get; set; }
        public string categoryName { get; set; } = string.Empty;
        public List<DetailsBookedModel> Tickets { get; set; } = new List<DetailsBookedModel>();

        public BookedTicketByIDQuery(int bookedTicketID) 
        {
            this.bookedTicketID = bookedTicketID;
        }

    }
}

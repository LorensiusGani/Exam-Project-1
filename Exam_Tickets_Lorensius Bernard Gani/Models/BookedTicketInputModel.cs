namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class BookedTicketInputModel
    {
        public List<BookedTicketInputItemModel> Tickets { get; set; } = new List<BookedTicketInputItemModel>();
    }
}

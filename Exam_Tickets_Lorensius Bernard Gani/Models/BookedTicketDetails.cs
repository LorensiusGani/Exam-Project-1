namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class BookedTicketDetails
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set;} = string.Empty;
        public int Quantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}

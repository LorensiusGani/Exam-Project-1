namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class TicketsResponseModel
    {
        public List<TicketsModel> Tickets { get; set; } = new List<TicketsModel>();
        public int TotalTickets { get; set; }
    }
}

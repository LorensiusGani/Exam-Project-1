namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class BookedTicketResponse
    {
        public int PriceSummary { get; set; }
        public List<BookedTicketCategoryResponse> TicketsPerCategories { get; set; } = new List<BookedTicketCategoryResponse>();
       
    }
}

namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class BookedTicketCategoryResponse
    {
        public string CategoryName { get; set; } = string.Empty;

        public int SummaryPrice { get; set; }

        public List<BookedTicketDetailResponse> Tickets { get; set; } = new List<BookedTicketDetailResponse>();

    }
}

using System.ComponentModel.DataAnnotations;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class BookedTicketInputItemModel
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}

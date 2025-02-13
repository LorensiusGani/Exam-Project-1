using System.ComponentModel.DataAnnotations;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class BookedTicket
    {
        [Required]
        public int Qty { get; set; }
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        [Required]
        public int BookTicketId { get; set; }
        [Required]
        public string TicketCode { get; set; } = string.Empty;
        [Required]
        public string TicketName { get; set; } = string.Empty;
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public int Price { get; set; }
    }
}

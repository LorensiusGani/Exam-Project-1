using System.ComponentModel.DataAnnotations;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class TicketsModel
    {

        [Required]
        public string EventDate { get; set; } = string.Empty;
        [Required]
        public int Quota { get; set; }
        [Required]
        public string TicketCode { get; set; } = string.Empty;
        [Required]
        public string TicketName { get; set;} = string.Empty;
        [Required]
        public string CategoryName { get; set;} = string.Empty;
        [Required]
        public int Price { get; set; }
    }
}

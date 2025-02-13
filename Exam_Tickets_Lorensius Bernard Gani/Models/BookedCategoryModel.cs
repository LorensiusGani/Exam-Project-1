namespace Exam_Tickets_Lorensius_Bernard_Gani.Models
{
    public class BookedCategoryModel
    {
        public int QtyProperty { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public List<DetailsBookedModel> Tickets { get; set; } = new List<DetailsBookedModel>();
    }
}

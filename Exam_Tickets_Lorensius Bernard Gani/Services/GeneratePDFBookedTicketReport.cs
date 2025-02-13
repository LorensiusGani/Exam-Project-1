using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Services
{
    public class GeneratePDFBookedTicketReport
    {
        public static string GenerateBookedTicketReport(List<BookedTicket> booked, string ComputerDirectory)
        {
            if(!Directory.Exists(ComputerDirectory))
            {
                Directory.CreateDirectory(ComputerDirectory);
            }

            string path = Path.Combine(ComputerDirectory, "BookedTicket.pdf");

            using(var writer = new PdfWriter(path))
            {
                using(var pdf = new PdfDocument(writer))
                {
                    var doc = new Document(pdf);
                    doc.Add(new Paragraph("Booked Ticket Report")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20));

                    var table = new Table(new float[] { 2, 3, 3, 3, 2, 2, 2 });
                    table.SetWidth(UnitValue.CreatePercentValue(100));

                    table.AddHeaderCell("BookTicketID");
                    table.AddHeaderCell("TicketCode");
                    table.AddHeaderCell("CategoryName");
                    table.AddHeaderCell("TicketName");
                    table.AddHeaderCell("EventDate");
                    table.AddHeaderCell("Price");
                    table.AddHeaderCell("Quantity");

                    foreach(var book in booked)
                    {
                        table.AddCell(book.BookTicketId.ToString()).SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(book.TicketCode).SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(book.CategoryName).SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(book.TicketName).SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(book.EventDate.ToString("dd-MM-yyyy HH:mm:ss")).SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(book.Price.ToString
                                ("C", new System.Globalization.CultureInfo("id-ID"))).SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(book.Qty.ToString()).SetTextAlignment(TextAlignment.CENTER);
                    }
                    doc.Add(table);
                    doc.Close();
                }
            }

            return path;

        }
    }
}

using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Services
{
    public class GeneratePDFBookedTicketReport
    {
        public static string GenerateBookedTicketReport(List<BookedTicket> booked, string ComputerDirectory)
        {
            if (!Directory.Exists(ComputerDirectory))
            {
                Directory.CreateDirectory(ComputerDirectory);
            }

            string path = Path.Combine(ComputerDirectory, "BookedTicket.pdf");

            using (var writer = new PdfWriter(path))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var doc = new Document(pdf);

                    // Judul Dokumen
                    doc.Add(new Paragraph("Booked Ticket Report")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20)
                        .SetFontColor(ColorConstants.BLUE));

                    // Loop untuk setiap tiket
                    foreach (var book in booked)
                    {
                        doc.Add(new Paragraph($"BookTicketID: {book.BookTicketId}"));
                        doc.Add(new Paragraph($"Ticket Code: {book.TicketCode}"));
                        doc.Add(new Paragraph($"Category: {book.CategoryName}"));
                        doc.Add(new Paragraph($"Ticket Name: {book.TicketName}"));
                        doc.Add(new Paragraph($"Event Date: {book.EventDate:dd-MM-yyyy HH:mm:ss}"));
                        doc.Add(new Paragraph($"Price: {book.Price.ToString("C", new System.Globalization.CultureInfo("id-ID"))}"));
                        doc.Add(new Paragraph($"Quantity: {book.Qty}"));
                    }

                    doc.Close();
                }
            }

            return path;
        }
    }
}

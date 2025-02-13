using Exam_Tickets_Lorensius_Bernard_Gani.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;

namespace Exam_Tickets_Lorensius_Bernard_Gani.Services
{
    public class GeneratePDFTicketsReport
    {
        public static string GenerateTicketReport(List<TicketsModel> reports, string ComputerDirectory)
        {
            if (!Directory.Exists(ComputerDirectory))
            {
                Directory.CreateDirectory(ComputerDirectory);
            }

            string path = Path.Combine(ComputerDirectory, "TicketReports.pdf");

                using(var writer = new PdfWriter(path))
                {
                    using (var pdf = new PdfDocument(writer)) 
                    {
                        var doc = new Document(pdf);
                        doc.Add(new Paragraph("Ticket Report") //Judul
                          .SetTextAlignment(TextAlignment.CENTER)
                          .SetFontSize(20));

                         var table = new Table(new float[] { 3, 3, 3, 3, 3, 3 });
                        table.SetWidth(UnitValue.CreatePercentValue(100));

                        //Table
                        table.AddHeaderCell("Event Date");
                        table.AddHeaderCell("Quota");
                        table.AddHeaderCell("TicketCode"); 
                        table.AddHeaderCell("TicketName");
                        table.AddHeaderCell("CategoryName");
                        table.AddHeaderCell("Price");

                        foreach(var ticket in reports)
                        {
                            table.AddCell(ticket.EventDate).SetTextAlignment(TextAlignment.CENTER);
                            table.AddCell(ticket.Quota.ToString()).SetTextAlignment(TextAlignment.CENTER);
                            table.AddCell(ticket.TicketCode).SetTextAlignment(TextAlignment.CENTER);
                            table.AddCell(ticket.TicketName).SetTextAlignment(TextAlignment.CENTER);
                            table.AddCell(ticket.CategoryName).SetTextAlignment(TextAlignment.CENTER);
                            table.AddCell(ticket.Price.ToString
                                ("C", new System.Globalization.CultureInfo("id-ID"))).SetTextAlignment(TextAlignment.CENTER);

                        }

                    doc.Add(table);
                        doc.Close();

                    }
                }
                return path;
        }
    }
}

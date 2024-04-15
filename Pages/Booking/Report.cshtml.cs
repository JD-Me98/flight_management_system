using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using iText.Layout.Borders;
using System.Security.Cryptography.Xml;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using System.Web;


namespace flight_management_system.Pages.Booking
{
    public class ReportModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string successMessage = "";
        public string errorMessage = "";
        public List<Booking> listBooking = new List<Booking>();
        public ReportModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet(string fullname, string email, string flight, string flightClass, string trip)
        {
            
            string outputPath = GetDownloadsFolderPath() + "/report.pdf";
            string ticketPath = GetDownloadsFolderPath() + "/ticket-"+fullname+".pdf";
            if (!string.IsNullOrEmpty(fullname))
            {
                printTicket(ticketPath, fullname, email, flight, flightClass, trip);
            }
            if (Request.Query["report"] != "" && User.Identity.IsAuthenticated)
            {
                GeneratePdfReport(outputPath);
            }
            Console.WriteLine($"PDF report generated at: {outputPath}");
        }

        public void GeneratePdfReport(string Path)
        {
            using (PdfWriter writer = new PdfWriter(Path))
            using (PdfDocument pdfDoc = new PdfDocument(writer))
            using (Document document = new Document(pdfDoc))
            {

                string agency = string.IsNullOrEmpty(User.FindFirst("agent").Value) ? "No Agency" : User.FindFirst("agent").Value.Trim() + " Report";
                Paragraph heading = new Paragraph(agency).SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(25);
                document.Add(heading);

                float col = 3000f;
                float[] colWidth = { col, col };

                Table table = new Table(colWidth);

                DateTime currentDateAndTime = DateTime.Now;

                Cell cell1 = new Cell(1, 1)
                    .SetBorder(Border.NO_BORDER)
                    .Add(new Paragraph("Date: " + currentDateAndTime));
                table.AddCell(cell1);
                document.Add(table);

                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "";
                    if (User.IsInRole("agent"))
                    {
                        query = "select * from booking B WHERE B.agent_id = @id " +
                        "AND created_at >= DATEADD(day, -30, GETDATE())";
                    }
                    else
                    {
                        query = "select * from booking B WHERE created_at >= DATEADD(day, -30, GETDATE())";//iyo ushaka kureba abantu bari hasi ya 30 days
                    }
                    
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (User.IsInRole("agent"))
                        {
                            cmd.Parameters.AddWithValue("@id", User.FindFirst("agent").Value);

                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            listBooking.Clear();
                            while (reader.Read())
                            {
                                Booking booking = new Booking();
                                booking.Id = reader.GetInt32(reader.GetOrdinal("id"));
                                booking.Fullname = reader.GetString(reader.GetOrdinal("fullname"));
                                booking.Email = reader.GetString(reader.GetOrdinal("email"));
                                booking.Flight = reader.GetString(reader.GetOrdinal("flight_id"));
                                booking.FlightClass = reader.GetString(reader.GetOrdinal("class"));
                                booking.Trip = reader.GetString(reader.GetOrdinal("trip"));
                                if (!reader.IsDBNull(reader.GetOrdinal("agent_id")))
                                {
                                    booking.Agency = reader.GetString(reader.GetOrdinal("agent_id"));
                                }
                                else
                                {
                                    booking.Agency = "No Agency";
                                }
                                booking.Created_at = reader.GetDateTime(reader.GetOrdinal("created_at"));

                                listBooking.Add(booking);
                            }
                        }
                    }
                }

                Table table1 = new Table(7); // Assuming 7 columns in your Booking class
                table1.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                table1.AddHeaderCell("ID");
                table1.AddHeaderCell("Fullname");
                table1.AddHeaderCell("Email");
                table1.AddHeaderCell("Flight");
                table1.AddHeaderCell("Flight Class");
                table1.AddHeaderCell("Trip");
                table1.AddHeaderCell("Agency");

                foreach (Booking booking in listBooking)
                {
                    table1.AddCell(booking.Id.ToString());
                    table1.AddCell(booking.Fullname);
                    table1.AddCell(booking.Email);
                    table1.AddCell(booking.Flight);
                    table1.AddCell(booking.FlightClass);
                    table1.AddCell(booking.Trip);
                    table1.AddCell(booking.Agency);
                }

                document.Add(table1);

                document.Close();

                successMessage = "Report Generated Successfully!";
            }

        }

        public void printTicket(string Path, string fullname, string email, string flight, string flightClass, string trip)
        {
            
            using (PdfWriter writer = new PdfWriter(Path))
            using (PdfDocument pdfDoc = new PdfDocument(writer))
            using (Document document = new Document(pdfDoc))
            {
                Paragraph heading = new Paragraph("Flight Ticket").SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(25);
                document.Add(heading);

                float col = 3000f;
                float[] colWidth = { col, col };

                float totalWidth = 600f;

                Table table = new Table(new float[] { totalWidth / 2f, totalWidth / 2f });

                // Create Paragraph instances for each piece of information
                Paragraph name = new Paragraph("Name: " + fullname).SetFontSize(14);
                Paragraph Email = new Paragraph("Email: " + email).SetFontSize(14);
                Paragraph Flight = new Paragraph("Flight: " + flight).SetFontSize(14);
                Paragraph FlightClass = new Paragraph("Class: " + flightClass).SetFontSize(14);
                Paragraph Trip = new Paragraph("Trip: " + trip).SetFontSize(14);

                // Create cells and add them to the table
                Cell cell1 = new Cell().Add(name).SetBorder(Border.NO_BORDER);
                Cell cell2 = new Cell(1, 2).Add(Email).SetBorder(Border.NO_BORDER);
                Cell cell3 = new Cell().Add(Flight).SetBorder(Border.NO_BORDER);
                Cell cell4 = new Cell().Add(FlightClass).SetBorder(Border.NO_BORDER);
                Cell cell5 = new Cell(1, 2).Add(Trip).SetBorder(Border.NO_BORDER);

                table.AddCell(cell1);
                table.AddCell(cell2);
                table.AddCell(cell3);
                table.AddCell(cell4);
                table.AddCell(cell5);

                // Add the table to the document
                document.Add(table);

                // Close the document
                document.Close();

                // Dispose of the PdfDocument
                pdfDoc.Close();
                successMessage = "Ticket Downloaded Successfully!";
            }
            return;
        }
            static string GetDownloadsFolderPath()
            {
                // Get the path to the Downloads folder using Environment.SpecialFolder
                string downloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                downloadsFolderPath = Path.Combine(downloadsFolderPath, "Downloads");

                return downloadsFolderPath;
            }

        public class Booking
        {
            public int Id { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Flight { get; set; }
            public string FlightClass { get; set; }
            public string Trip { get; set; }
            public string Agency { get; set; }
            public DateTime Created_at { get; set; }
        }
        public class BookingData
        {
            public int Id { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Flight { get; set; }
            public string FlightClass { get; set; }
            public string trip { get; set; }
            public string Agency { get; set; }
        }
    }
}

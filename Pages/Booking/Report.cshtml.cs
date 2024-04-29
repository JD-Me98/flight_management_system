using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Colors;


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
                table.UseAllAvailableWidth();
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
            Ticket ticket = new Ticket();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "select ao.location as 'from', ad.location as 'to', " +
                    "CONVERT(varchar(10), f.departure, 101) as 'date', " +
                    "CONVERT(varchar(8), DATEADD(minute, -30, f.departure), 108) as 'boarding' from flight f " +
                    "JOIN airport ao on ao.id = f.departure_airport_id " +
                    "JOIN airport ad on ad.id = f.destination_airport_id " +
                    "where f.id=@id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    
                        cmd.Parameters.AddWithValue("@id", flight);
                    

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        listBooking.Clear();
                        if (reader.Read())
                        {                            
                            ticket.from = reader.GetString(reader.GetOrdinal("from"));
                            ticket.to = reader.GetString(reader.GetOrdinal("to"));
                            ticket.Boarding = reader.GetString(reader.GetOrdinal("boarding"));
                            ticket.FlightDate = reader.GetString(reader.GetOrdinal("date"));
                        }
                    }
                }
            }

            using (PdfWriter writer = new PdfWriter(Path))
            using (PdfDocument pdfDoc = new PdfDocument(writer))
            using (Document document = new Document(pdfDoc))
            {
                Paragraph heading = new Paragraph("Flight Ticket").SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(25);
                
                string logoPath = "wwwroot/images/logo-no-background.png";

                Image logo = new Image(ImageDataFactory.Create(logoPath));
                logo.SetWidth(70);
                

                Table header = new Table(2);
                header.UseAllAvailableWidth();
                DeviceRgb customColor = new DeviceRgb(255, 203, 0);
                header.SetBackgroundColor(customColor);

                Cell cellLogo = new Cell().Add(logo).SetBorder(Border.NO_BORDER);
                Cell title = new Cell().Add(heading).SetBorder(Border.NO_BORDER); 

                header.AddCell(cellLogo);
                header.AddCell(title);

                document.Add(header);

                float col = 150f;
                float[] colWidth = { col, col , col };

                float totalWidth = 550f;

                Table table = new Table(3);
                table.UseAllAvailableWidth();
                Table FromTo = new Table(3);
                Table last = new Table(3);
                last.UseAllAvailableWidth();

                // Create Paragraph instances for each piece of information
                Paragraph name = new Paragraph(fullname).SetFontSize(11).SetBold();
                Paragraph Flight = new Paragraph(flight).SetFontSize(11).SetBold();
                Paragraph FlightClass = new Paragraph(flightClass).SetFontSize(11).SetBold();
                Paragraph origin = new Paragraph(ticket.from).SetFontSize(24).SetBold();
                Paragraph destination = new Paragraph(ticket.to).SetFontSize(24).SetBold();
                Paragraph boarding = new Paragraph(ticket.Boarding.ToString()).SetFontSize(11).SetBold();
                Paragraph flightDate = new Paragraph(ticket.FlightDate.ToString()).SetFontSize(11).SetBold();

                // Create cells and add them to the table
                Cell cellName = new Cell().Add(new Paragraph("PASSENGER").SetFontSize(11)).SetBorder(Border.NO_BORDER); ;
                Cell cellFlight = new Cell().Add(new Paragraph("FLIGHT").SetFontSize(11)).SetBorder(Border.NO_BORDER); ;
                Cell cellDate = new Cell().Add(new Paragraph("DATE").SetFontSize(11)).SetBorder(Border.NO_BORDER); ;

                table.AddCell(cellName);
                table.AddCell(cellFlight);
                table.AddCell(cellDate);

                Cell cell1 = new Cell().Add(name).SetBorder(Border.NO_BORDER); ;                
                Cell cell2 = new Cell().Add(Flight).SetBorder(Border.NO_BORDER); ;                
                Cell cell3 = new Cell().Add(flightDate).SetBorder(Border.NO_BORDER); ;
                
                table.AddCell(cell1);
                table.AddCell(cell2);                                                
                table.AddCell(cell3);

                // Add the table to the document
                document.Add(table);

                //From To
                Cell emptyleft = new Cell().Add(new Paragraph("            ")).SetBorder(Border.NO_BORDER); ;
                string fPath = ticket.from.Trim() + " to " + ticket.to.Trim();
                Cell cellFrom = new Cell().Add(new Paragraph(fPath)).SetFontSize(32).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                Cell emptyRight = new Cell().Add(new Paragraph("           ")).SetBorder(Border.NO_BORDER); ;

                FromTo.AddCell(emptyleft);
                FromTo.AddCell(cellFrom);
                FromTo.AddCell(emptyRight);

                document.Add(FromTo);

                //footer
                Cell cellClass = new Cell().Add(new Paragraph("CLASS")).SetFontSize(11).SetBorder(Border.NO_BORDER);
                Cell cellBoarding = new Cell().Add(new Paragraph("BOARDING")).SetFontSize(11).SetBorder(Border.NO_BORDER);
                Cell cellNull = new Cell().Add(new Paragraph("")).SetBorder(Border.NO_BORDER);

                last.AddCell(cellClass);
                last.AddCell(cellBoarding);
                last.AddCell(cellNull);

                Cell cell5 = new Cell().Add(FlightClass).SetBorder(Border.NO_BORDER);                
                Cell cell6 = new Cell().Add(boarding).SetBorder(Border.NO_BORDER);
                
                string imagePath = "wwwroot/images/barcode.gif";

                Image barcode = new Image(ImageDataFactory.Create(imagePath));
                barcode.SetWidth(150);

                Cell cell7 = new Cell().Add(barcode).SetBorder(Border.NO_BORDER);
                                
                last.AddCell(cell5);                
                last.AddCell(cell6);
                last.AddCell(cell7);

                document.Add(last);

                Table footer = new Table(1);
                footer.UseAllAvailableWidth();

                //DeviceRgb footerColor = new DeviceRgb(255, 203, 0);
                footer.SetBackgroundColor(customColor);

                Cell copyright = new Cell().Add(new Paragraph("Copyright 2024 Intore Flights.").SetFontSize(11)).SetBorder(Border.NO_BORDER);

                footer.AddCell(copyright);

                document.Add(footer);

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

        public class Ticket
        {
            public string from { get; set; }
            public string to { get; set; }
            public string Boarding { get; set; }

            public string FlightDate { get; set; }
        }
    }
}

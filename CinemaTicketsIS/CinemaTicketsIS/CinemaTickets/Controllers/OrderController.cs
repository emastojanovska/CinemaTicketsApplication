using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CinemaTickets.Repository;
using CinemaTickets.Service.Interface;
using ClosedXML.Excel;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTickets.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;
        public OrderController(IOrderService orderService, ApplicationDbContext context)
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            _orderService = orderService;
            _context = context;
        }
        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = _orderService.getAllOrdersForUser(userId);
            return View(orders);
        }


        public FileContentResult CreateInvoice(Guid id)
        {
            var result = _orderService.GetOrderDetails(id);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{Username}}", result.User.Email);

            StringBuilder sb = new StringBuilder();

            var totalPrice = 0.0;

            foreach (var item in result.TicketsInOrders)
            {

                totalPrice += item.SelectedTicket.Price * item.Quantity;
                var name = "";
                foreach(var movie in _context.Movies)
                {
                    if(item.SelectedTicket.MovieId.Equals(movie.Id))
                    {
                        name = movie.MovieName;
                    }
                }
                sb.AppendLine(item.Quantity + " tickets for "+name + " with price of: " + item.SelectedTicket.Price + "$");
            }


            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "$");


            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "OrderExportInvoice.pdf");
        }
    }

}

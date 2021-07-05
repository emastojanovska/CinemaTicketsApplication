using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CinemaTickets.Repository;
using CinemaTickets.Domain.DTO;
using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Service.Interface;
using ClosedXML.Excel;
using System.IO;

namespace CinemaTickets.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IMovieService _movieService;
        private readonly Repository.ApplicationDbContext _context;

        public TicketsController(ITicketService ticketService, IMovieService movieService, ApplicationDbContext context)
        {
            _ticketService = ticketService;
            _movieService = movieService;
            _context = context;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            var allTickets = _ticketService.GetAllTickets();
            return View(allTickets);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketService.GetDetailsForTicket(id);
            
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public IActionResult AddTicketToCart(Guid? id)
        {
            var model = _ticketService.GetShoppingCartInfo(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCart([Bind("TicketId", "Quantity")] AddToShoppingCartDto item)
        {          
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _ticketService.AddToShoppingCart(item, userId);
               
            if(result)
            {
                return RedirectToAction("Index", "Tickets");
            }           
           
            return View(item);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_movieService.GetAllMovies(), "Id", "MovieName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,DateTime,Available,MovieId,Price")] Ticket ticket)
        {
           if (ModelState.IsValid)
           {
                _ticketService.CreateNewTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(this._movieService.GetAllMovies(), "Id", "MovieName", ticket.MovieId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_movieService.GetAllMovies(), "Id", "MovieName", ticket.MovieId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,DateTime,Available,MovieId,Price")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ticketService.UpdateExistingTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_movieService.GetAllMovies(), "Id", "MovieName", ticket.MovieId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost]
        public FileContentResult ExportAllTickets(Domain.DomainModels.Enumerations.Genre movieGenre)
        {
            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Tickets");

                worksheet.Cell(1, 1).Value = "Ticket Id";
                worksheet.Cell(1, 2).Value = "Movie";
                worksheet.Cell(1, 3).Value = "Date and Time";
                worksheet.Cell(1, 4).Value = "Price";
                worksheet.Cell(1, 5).Value = "Movie Genre";


                var result = _ticketService.GetAllTicketsByGenre(movieGenre);

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];
                  
                        worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                        foreach (var m in _context.Movies)
                        {
                            if (item.MovieId.Equals(m.Id))
                            {
                                worksheet.Cell(i + 1, 2).Value = m.MovieName;
                            }
                        }
                        worksheet.Cell(i + 1, 3).Value = item.DateTime.ToString();
                        worksheet.Cell(i + 1, 4).Value = item.Price + "$";
                        worksheet.Cell(i + 1, 5).Value = movieGenre;
                    


                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }

            }
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(Guid id)
        {
            return _ticketService.GetDetailsForTicket(id) != null;
        }
    }
}

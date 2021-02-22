using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookShop.WebApplication.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookShopContext bookShopContext;

        public BooksController(BookShopContext bookShopContext)
        {
            this.bookShopContext = bookShopContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return this.RedirectToRoute("/Books/All");
        }

        [HttpGet]
        public IActionResult All()
        {
            List<Book> books = bookShopContext
               .Books
               .ToList();

            return this.View(books);
        }


    }
}

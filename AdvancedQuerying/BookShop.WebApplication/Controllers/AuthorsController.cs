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
    public class AuthorsController : Controller
    {
        private readonly BookShopContext bookShopContext;

        public AuthorsController(BookShopContext bookShopContext)
        {
            this.bookShopContext = bookShopContext;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return this.RedirectToRoute("/Authors/All1");
        }

        [HttpGet]
        public IActionResult All1()
        {
            List<Author> authors = bookShopContext
               .Authors
               .ToList();

            return this.View(authors);
        }

    }
}

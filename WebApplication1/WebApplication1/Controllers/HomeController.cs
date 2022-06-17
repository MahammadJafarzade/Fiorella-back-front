using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _context { get; }
        public HomeController(AppDbContext context)
        {
            _context=context;
        }
        public IActionResult Index()
        {
            HttpContext.Session.SetString("name","Maqa");
            HttpContext.Response.Cookies.Append("surname", "Ceferzade");
            HomeViewModel home = new HomeViewModel
            {
                Slides = _context.Slides.ToList(),
                SliderSummary =_context.SliderSummary.FirstOrDefault(),
                Categories = _context.Categories.Where(c=>!c.IsDeleted).ToList(),
                Products = _context.Products.Where(c => !c.IsDeleted).Include(p => p.Images).Include(p => p.Category).Take(8).ToList()
            };
            return View(home);
        }

        public IActionResult LoadProducts()
        {
            List<Product> products = _context.Products.Where(c => !c.IsDeleted).Include(p => p.Images).Include(p => p.Category).Skip(8).Take(8).ToList();
            //return Json(products);
            return PartialView("", products);
        }
        public IActionResult GetSession()
        {
            string session = HttpContext.Session.GetString("name");
            string cookies = HttpContext.Request.Cookies["surame"];
            return Json(session+"-"+cookies);
        }
    }
}

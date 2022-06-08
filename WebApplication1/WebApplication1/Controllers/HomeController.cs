using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            HomeViewModel home = new HomeViewModel
            {
                Slides = _context.Slides.ToList(),
                SliderSummary =_context.SliderSummary.FirstOrDefault(),
                Categories = _context.Categories.Where(c=>!c.IsDeleted).ToList(),
                Products = _context.Products.Where(c => !c.IsDeleted).Include(p=>p.Images).Include(p=>p.Category).ToList()
            };
            return View(home);
        }
    }
}

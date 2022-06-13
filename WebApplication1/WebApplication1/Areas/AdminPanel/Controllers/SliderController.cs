using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Categories;

namespace WebApplication1.Areas.AdminPanel.Controllers
{
    [Area("Adminpanel")]
    public class SliderController : Controller
    {
        private AppDbContext _context { get; }
        private IWebHostEnvironment _env { get; }
        private int countSlider { get; }
        private int sliderMaxCount { get; }


        private IEnumerable<SliderController> sliders;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            countSlider = _context.Slides.Count();
            sliderMaxCount=int.Parse(_context.Settings.ToDictionary(s => s.Key, s => s.Value)["slider_maxcount"]);
        }

        public IActionResult Index()
        {
            ViewBag.slideCount = countSlider;
            ViewBag.sliderMaxCount = sliderMaxCount;
            return View(_context.Slides);
        }
        public IActionResult Create()
        {
            if (countSlider >= ViewBag.sliderMaxCount)
            {
                return Content("beledi");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public  async Task<IActionResult> Create(Slide slide)
        {
            if (!ModelState.IsValid) return View();
            if (slide.Photos.Count + countSlider > ViewBag.sliderMaxCount)
            {
                ModelState.AddModelError("", $"Maximium count of slider must be { ViewBag.sliderMaxCount}. You can create.{ ViewBag.sliderMaxCount - countSlider}photos");
                return View();
            }
            string errorMessage = string.Empty;
            foreach(var photo in slide.Photos)
            {
                if (!photo.CheckFileSize(200))
                {
                    errorMessage += $"Max image size({photo.FileName})must be 200kb.\n";
                }
                if (!photo.CheckFileType("image/"))
                {
                    errorMessage+=$"Type of file({photo.FileName}) must be image.\n";
                    return View();
                }
            }
            if (errorMessage != String.Empty)
            {
                ModelState.AddModelError("Photo", errorMessage);
                return View(); 
            }
            foreach(var photo in slide.Photos)
            {
                Slide newSlide = new Slide();
               // newSlide= await photo.SaveFileAsync(_env.WebRootPath, "img");
                await _context.Slides.AddAsync(slide);
            }
                await _context.SaveChangesAsync();

            #region
            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}
            //if (!slide.Photo.CheckFileSize(200))
            //{
            //    ModelState.AddModelError("Photo", "max size must be less than 200kb");
            //    return View();
            //}
            //if(!slide.Photo.CheckFileType("image/"))
            //{
            //    ModelState.AddModelError("Photo", "Type of file must be image");
            //    return View();
            //}


            #endregion
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var slider = _context.Slides.Find(id);
            if (slider == null) return NotFound();
            var path = Helper.GetPath(_env.WebRootPath, "img", slider.Url);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Slides.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #region
        //public IActionResult Update(int? id)
        //{
        //    if (id == null) return BadRequest();
        //    var slider = _context.Slides.Find(id);
        //    if (slider == null) return NotFound();
        //    return View(slider);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Update(int? id, Slide slider)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }
        //    var sliderDb = _context.Slides.Find(id);
        //    if (sliderDb == null)
        //    {
        //        return NotFound();
        //    }
        //    var removePath = Helper.GetPath(_env.WebRootPath, "img", sliderDb.Url);
        //    if (System.IO.File.Exists(removePath))
        //    {
        //        System.IO.File.Delete(removePath);
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    if (!slider.Photo.CheckFileSize(200))
        //    {
        //        ModelState.AddModelError("Photo", "Maximum file size is 200 Kb!");
        //        return View();
        //    }
        //    if (!slider.Photo.CheckFileType("image/"))
        //    {
        //        ModelState.AddModelError("Photo", "File type must be image");
        //        return View();
        //    }
        //    slider.Url = await slider.Photo.SaveFileAsync(_env.WebRootPath, "img");
        //    sliderDb.Url = slider.Url;
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion

    }
}

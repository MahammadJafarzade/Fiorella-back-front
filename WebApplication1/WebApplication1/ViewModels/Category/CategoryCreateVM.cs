﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.ViewModels.Categories
{
    public class CategoryCreateVM
    {
        [Required(ErrorMessage ="Must filled!")]
        public string Name { get; set; }
    }
}

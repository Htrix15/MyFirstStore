using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;

namespace MyFirstStore.ViewModels
{
    public class ProductAndProductTypes
    {
        public Product Product { get; set; }
        public List<SelectListItem> ProductTypes { get; set; }
    }
}

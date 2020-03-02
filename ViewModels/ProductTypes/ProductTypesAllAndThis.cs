using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyFirstStore.ViewModels
{
    public class ProductsTypesAllAndThis
    {
        public List<SelectListItem> ProductTypes { get; set; }
        public ProductTypeMini ProductType { get; set; }
        public void SetProductTypes(List<ProductTypeMini> allNoParentProductTypes, int idThisType)
        {
            ProductTypes = new List<SelectListItem>
                {
                    new SelectListItem() { Value = "1", Text = "Это родительский тип" }
                };
            foreach (var item in allNoParentProductTypes)
            {
                if (item.Id != idThisType)
                {
                    ProductTypes.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
                }
            }
        }
        public void SetProductTypes(List<ProductTypeMini> allNoParentProductTypes)
        {
            ProductTypes = new List<SelectListItem>
                {
                    new SelectListItem() { Value = "1", Text = "Это родительский тип" }
                };
            foreach (var item in allNoParentProductTypes)
            {
                ProductTypes.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
            }
        }

    }
}

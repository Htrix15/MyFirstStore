using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyFirstStore.Models;
using MyFirstStore.Services;
using MyFirstStore.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using MyFirstStore.ViewModels.Storekeeper;

namespace MyFirstStore.Controllers
{
    [Authorize(Roles = "Admin, ContentManager")]
    public class ContentMakingController : Controller
    {
        private readonly DataProcessingConveyor _dataProcessingConveyor;
        public ContentMakingController(DataProcessingConveyor dataProcessingConveyor)
        {
            _dataProcessingConveyor = dataProcessingConveyor;
        }
        //------
        public IActionResult Index()
        {
            return View();
        }
        //------------------------------------
        public async Task<IActionResult> ProductAttributesView(int? currentPosition, string sortType)
        {
            FilterBase filters = new FilterBase(countVisablePozitins: 3,
                                                currentPosition: currentPosition,
                                                sortType: sortType);
            var result = await _dataProcessingConveyor.GetProductAttributesAndFilterAsync(filters);
            return View(result);
        }
        [HttpGet]
        public IActionResult NewProductsAttribute()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewProductsAttribute(ProductsAttribute productsAttribute)
        {
            if (ModelState.IsValid)
            {
                if (await _dataProcessingConveyor.AddProductsAttributeAsync(productsAttribute))
                {
                    return RedirectToAction("ProductAttributesView");
                }
                else
                {
                    return NotFound();
                }
            }
            return View(productsAttribute);
        }
        [HttpGet]
        public async Task<IActionResult> EditProductsAttribute(int? id)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.GetProductsAttributeAsync((int)id);
                if (result != null)
                {
                    return View(result);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditProductsAttribute(ProductsAttribute productsAttribute)
        {
            if (ModelState.IsValid)
            {
                if (await _dataProcessingConveyor.EditProductsAttributeAsync(productsAttribute))
                {
                    return RedirectToAction("ProductAttributesView");
                }
                else
                {
                    return View(productsAttribute);
                }
            }
            return View(productsAttribute);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProductsAttribute(int? id)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.GetProductsAttributeAsync((int)id);
                if (result != null)
                {
                    return View(result);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProductsAttribute(ProductsAttribute productsAttribute)
        {
            if (await _dataProcessingConveyor.DeleteProductsAttributeAsync(productsAttribute))
            {
                return RedirectToAction("ProductAttributesView");
            }
            else
            {
                return NotFound();
            }
        }
        //------------------------------
        public async Task<IActionResult> ProductsTypesView(string sortType, int? currentPosition, string select)
        {
            FilterBase filters = new FilterBase(countVisablePozitins: 3,
                                                currentPosition: currentPosition,
                                                selectFromSelectList: select,
                                                sortType: sortType);
            ProductTypeMiniAndFilters result = await _dataProcessingConveyor.GetProductTypeMinisAsync(filters);
            return View(result);
        }    
        [HttpGet]
        public async Task<IActionResult> NewProductsTypes()
        {
            ProductsTypesAllAndThis productsTypesAllAndThis = await _dataProcessingConveyor.GetPropductParentTypesAsync();
            return View(productsTypesAllAndThis);
        }
        [HttpPost]
        public async Task<IActionResult> NewProductsTypes(ProductsType productsType)
        {
            if (ModelState.IsValid)
            {
                if (await _dataProcessingConveyor.AddProductsTypeAsync(productsType))
                {
                    return RedirectToAction("ProductsTypesView");
                }
            }
            else
            {
                View(productsType);
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> EditProductsTypes(int? id)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.GetProductTypeThisAndParentsAsync((int)id);
                if (result != null)
                {
                    return View(result);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditProductsTypes(ProductsType productsType, string oldHEXColor, int olpParentProductTypeId)
        {
            if (ModelState.IsValid)
            {
                if (await _dataProcessingConveyor.EditProdyctTypeAsync(productsType:productsType,
                                                                       oldHEXColor:oldHEXColor,
                                                                       olpParentProductTypeId:olpParentProductTypeId))
                {
                   
                    return RedirectToAction("ProductsTypesView");
                }
                else
                {
                    return NotFound();
                }
            }
            return View(productsType);

        }
        [HttpGet]
        public async Task<IActionResult> DeleteProductsTypes(int? id)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.GetProductTypeThisAndParentsAsync((int)id);
                if (result != null)
                {
                    return View(result);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProductsTypes(ProductsType productsType)
        {
            if (await _dataProcessingConveyor.DeleteProductTypeAsync(productsType))
            {
                return RedirectToAction("ProductsTypesView");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditAttributesToProductTypes(int? productTypeId, int? parentTypeId)
        {
            if (productTypeId != null)
            {
                var result = await _dataProcessingConveyor.GetAttributesToProductsTypesChecksAsync((int)productTypeId, parentTypeId);
                ViewData["productTypeId"] = productTypeId;
                ViewData["parentTypeId"] = parentTypeId;
                return View(result);
            }
            else
                return RedirectToAction("ProductsTypesView");
        }
        [HttpPost]
        public async Task<IActionResult> EditAttributesToProductTypes(int? productTypeId, int? parentTypeId, List<int> attributesToProductsTypesIdCheck)
        {
            if (productTypeId != null)
            {

                bool result = await _dataProcessingConveyor.SetAttributesToProductsTypesChecksAsync((int)productTypeId, parentTypeId, attributesToProductsTypesIdCheck);
                if (!result)
                {
                    return NotFound();
                }
            }
            return RedirectToAction("ProductsTypesView");
        }
        //------------------------------
        public async Task<IActionResult> ProductsView(string whereSearch,
                                                      string desired,
                                                      string sortType,
                                                      int? currentPosition,
                                                      string select)
        {
            FilterBaseAndSearch filters = new FilterBaseAndSearch(desired: desired,
                                                                  whereSearch: whereSearch,
                                                                  countVisablePositions: 3,
                                                                  currentPosition: currentPosition,
                                                                  selectFromSelectList: select,
                                                                  sortType: sortType);
            ProductCardForEditAndFilters result = await _dataProcessingConveyor.GetProductCardForEditAndFiltersAsync(filters);
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> NewProduct()
        {
            var allType = await _dataProcessingConveyor.GetNoParentPTtoSelectListAsync();
            return View(new ProductAndProductTypes() { Product = null, ProductTypes = allType });
        }
        [HttpPost]
        public async Task<IActionResult> NewProduct(Product product, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                if (await _dataProcessingConveyor.AddProductsAsync(product, uploadedFile))
                {
                    return RedirectToAction("ProductsView");
                }
                else return NotFound();
            }
            var allType = await _dataProcessingConveyor.GetNoParentPTtoSelectListAsync();
            return View(new ProductAndProductTypes() { Product = null, ProductTypes = allType });
        }
        [HttpGet]
        public async Task<IActionResult> EditProducts(int? id)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.GetProductAndProductTypesAsync((int)id);
                if (result != null)
                {
                    return View(result);
                }
            }
            return RedirectToAction("ProductsView");
        }
        [HttpPost]
        public async Task<IActionResult> EditProducts(Product product, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                if (await _dataProcessingConveyor.EditProductsAsync(product, uploadedFile))
                {
                    return RedirectToAction("ProductsView");
                }
            }
            var allType = await _dataProcessingConveyor.GetNoParentPTtoSelectListAsync();
            return View(new ProductAndProductTypes() { Product = product, ProductTypes = allType });
        }
        [HttpGet]
        public async Task<IActionResult> EditAttributesToProduct(int? id, int productTypeId, int parentTypeId)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.GetAttributesToProductAsync((int)id, productTypeId, parentTypeId);
                return View(result);
            }
            else
                return RedirectToAction("ProductsView");
        }
        [HttpPost]
        public async Task<IActionResult> EditAttributesToProduct(int id, List<AttributesToProduct> values)
        {
            if (ModelState.IsValid)
            {
                if (await _dataProcessingConveyor.SetAttributesToProductAsync(id, values))
                {
                    return RedirectToAction("ProductsView");
                }
                return NotFound();
            }
            return View(values);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.ProductAndOrdersAsync((int)id);
                return View(result);
            }
            return NotFound();
        }
        [HttpPost]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteProductP(int? id)
        {
            if (id != null)
            {
                if (await _dataProcessingConveyor.DeleteProductAsync((int)id))
                {
                    return RedirectToAction("ProductsView");
                }
            }
            return NotFound();

        }
        [HttpGet]
        public async Task<IActionResult> EditPicturesToProduct(int? id)
        {
            if (id != null)
                return View(await _dataProcessingConveyor.GetProductsPicturesPathsAsync((int)id));
            else
                return RedirectToAction("ProductsView");
        }
        [HttpPost]
        public async Task<IActionResult> EditPicturesToProduct(int id, IFormFile uploadedFiles1, IFormFile uploadedFiles2, IFormFile uploadedFiles3)
        {
            if (uploadedFiles1 != null || uploadedFiles2 != null || uploadedFiles3 != null)
            {
                if(await _dataProcessingConveyor.AddProductsPicturesPathsAsync(id, uploadedFiles1, uploadedFiles2, uploadedFiles3))
                {
                    return RedirectToAction("ProductsView");
                }
                else
                {
                    return NotFound();
                }

            }
            return RedirectToAction("ProductsView");
        }
    }

}
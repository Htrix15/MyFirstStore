using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyFirstStore.Models;
using MyFirstStore.ViewModels;

namespace MyFirstStore.TagHelpers
{
    public class ProductsTypesBoxsTagHelper : TagHelper
    {

        public List<ProductTypeMini> LProductsTypes {get;set;}
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string htmlText="";
            string backColor= "";
            List<ProductTypeMini> lheadLineText = LProductsTypes.Where(table => table.ParentProductTypeId == 1 &&table.Id!=2).ToList();//1-родители 2 - без родительнеы позиции

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "allpanel");
            output.TagMode = TagMode.StartTagAndEndTag;

            for (int i = 0; i < lheadLineText.Count; i++)
            {

                htmlText += "\n<div class=\"productTypeBox\"\n>";
                var LLine = LProductsTypes.Where(table => (table.ParentProductTypeId == lheadLineText[i].Id));
                backColor = $"bc{(lheadLineText[i].HEXColor) ?? "DefaultColor"}";
                if (LLine.Count() != 0)
                {
                    htmlText += $"<a class=\"mainProductType {backColor} fontPdosuctTypeStyle\" href=\"/Home/ProductsList?select={lheadLineText[i].Id}\"> {lheadLineText[i].Name}</a>\n";
                    foreach (var itemLine in LLine)
                    {
                        backColor = $"bc{(itemLine.HEXColor) ?? "DefaultColor"}";
                        htmlText += $"<a class=\"subProductType {backColor} fontPdosuctTypeStyle\" href=\"/Home/ProductsList?select={itemLine.Id}\"> {itemLine.Name}</a>\n";
                    }
                }
                else
                {
                    htmlText += $"<a class=\"mainSingleProductType {backColor}\">{lheadLineText[i].Name }</a>\n";
                }
                htmlText += "\n</div>";
            }
            output.PostContent.SetHtmlContent(htmlText);
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyFirstStore.ViewModels;

namespace MyFirstStore.TagHelpers
{
    public class ProductListBasket : TagHelper
    {
        public List<ProductInBasket> ProductInBaskets { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ProductInBaskets != null)
            {
                output.TagName = "div";
                output.Attributes.SetAttribute("class", "basket visibilityOFF");
                output.TagMode = TagMode.StartTagAndEndTag;
                StringBuilder stringBuilder = new StringBuilder();
                int productId;
                foreach (var item in ProductInBaskets)
                {
                    productId = item.Id;
                    stringBuilder.Append($"<div class=\"basketBox borderProductBox{item.HEXcolor}\"  idBasket=\"{productId}\">");
                    stringBuilder.Append($"<img src =\"{item.MainPicturePath}\" width=100px />");
                    stringBuilder.Append($"<span class=\"basketMiniText\"> {item.Name}</span>");
                    stringBuilder.Append($"<div><span class=\"price basketMiniText\"> {item.Price}</span><span class=\"basketMiniText\"> р.</span></div>");
                    stringBuilder.Append("<div class=\"basketMiniButtons\">");
                    stringBuilder.Append($"<button value =\"{productId}\" class=\"removeProductForBasket minButton bc{item.HEXcolor}\"> - </button>");
                    stringBuilder.Append($"<div><span class=\"countProduct basketMiniText\"> {item.Count}</span><span class=\"basketMiniText\"> шт.</span></div>");
                    stringBuilder.Append($"<button value =\"{productId}\" class=\"addProductToBasket minButton bc{item.HEXcolor} \"> + </button>");
                    stringBuilder.Append("</div>");
                    stringBuilder.Append($"<button value =\"{productId}\" class=\"removeAllProductForBasket big1Button bc{item.HEXcolor}\"> Удалить </button></div>");
                }
                stringBuilder.Append("<div class=\"newBasket\"></div>");
                output.Content.SetHtmlContent(stringBuilder.ToString());
            }
        }
    }
}

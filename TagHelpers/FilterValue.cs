using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyFirstStore.ViewModels;
using MyFirstStore.ViewModels.Filters;

namespace MyFirstStore.TagHelpers
{
    public class FilterValue : TagHelper
    {
        public IGetFilterInfoToHtml Filters { get; set; }
        public string FilterDesc { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "filterBox");
            StringBuilder stringBuilder = new StringBuilder();
            string selected = Filters.GetSelect();
            string checkSelected;

            stringBuilder.Append($"<form action = \"/{Controller}/{Action}\" method = \"get\">");
            if (Filters.GetSelectListItems() != null)//&& Filters.GetSelectListItems().Count > 2
            {
                stringBuilder.Append($"<span class=\"filterText\">{FilterDesc}: </span> <select name=\"select\" class=\"inputStyle\">");
                foreach (var item in Filters.GetSelectListItems())
                {
                    checkSelected = item.Value == selected ? "selected" : "";
                    stringBuilder.Append($"<option class=\"inputStyle\" value=\"{item.Value}\" {checkSelected}>{item.Text} </option>");
                }
                stringBuilder.Append($"</select>");
            }
            stringBuilder.Append(Filters.GetFilterInfoForFilterTagHelper());
            stringBuilder.Append($"<input type = \"submit\" class=\"filterButton\" value=\"Применить\" />");
            stringBuilder.Append($"</form>");
            stringBuilder.Append($"<div><form action = \"/{Controller}/{Action}\"  method = \"get\">");
            if (Controller == "Home") 
            {
                if (Action == "ProductsList")
                {
                    stringBuilder.Append($"<input type=\"hidden\" name=\"select\" value=\"{Filters.GetSelect()}\" />");
                }
                if(Action == "Search")
                {
                    stringBuilder.Append($"<input type =\"hidden\" name = \"whereSearch\" value = \"{Filters.GetWhereSearch()}\" />");
                    stringBuilder.Append($"<input type = \"hidden\" name = \"desired\" value = \"{Filters.GetDesired()}\" />");
                }
            }
            stringBuilder.Append("<input type = \"submit\" class=\"filterButton\" value = \"Сбросить фильтр\"/></form></div>");
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}

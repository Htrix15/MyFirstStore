using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstStore.ViewModels;
using MyFirstStore.ViewModels.Filters;

namespace MyFirstStore.TagHelpers
{
    public class Search : TagHelper
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public IGetFilterInfoToHtml Filters { get; set; }
        public Array SearchEnumArray { get; set; }
        public bool Reset { get; set; } = false;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "form";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("method", "get");
            output.Attributes.SetAttribute("action", $"/{Controller}/{Action}");
            output.Attributes.SetAttribute("class", "search");
            StringBuilder stringBuilder = new StringBuilder();
            string selected = "";
            string checkSelected;
            string desired = "";
            if (Filters != null)
            {
                selected = Filters.GetWhereSearch();
                desired = Filters.GetDesired();
            }
            string item;
            stringBuilder.Append("<div class=\"searchOptions\">");
            stringBuilder.Append("<span class=\"searchText\">Поиск по:</span>");
            stringBuilder.Append("<select class=\"searchInputStyle\" name =\"whereSearch\">"); 
            for (int i = 0; i < SearchEnumArray.Length; i++)
            {
                item = SearchEnumArray.GetValue(i).ToString();
                checkSelected = item.ToString() == selected ? "selected" : "";
                stringBuilder.Append($"<option class=\"searchInputStyle\" value=\"{item.ToString()}\" {checkSelected}>{item.ToString().Replace("_", " ")} </option>");
            }
            stringBuilder.Append("</select></div>");
            stringBuilder.Append($"<input type=\"text\" name=\"desired\" class=\"inputDesired\" required minlength=\"3\" placeholder=\"минимум 3 символа\" value=\"{desired}\"/>");
            stringBuilder.Append("<input type = \"submit\"class=\"linkText\" value=\"Искать\" />");
            if(Reset)
            {
                stringBuilder.Append($"<a href=\"/{Controller}/{Action}\" class=\"linkText\">Сбросить</a>");
            }
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}

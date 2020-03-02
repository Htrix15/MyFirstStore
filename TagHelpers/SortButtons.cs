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
    public class SortButtons : TagHelper
    {
        public IGetFilterInfoToHtml Filters { get; set; }
        public Array SortEnumArray { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "sortButtonsBox");
            string disabled;
            string buttonClass ;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<span class=\"sortButtonsText\">Сортировать по: </span>");
            for (int i=0; i< SortEnumArray.Length; i++)
            {
                string item = SortEnumArray.GetValue(i).ToString();
                stringBuilder.Append($"<form>");
                stringBuilder.Append($"<input type=\"hidden\" name=\"currentPozition\" value=\"1\" />");
                stringBuilder.Append(Filters.GetFilterInfoForTagHelper(true));
                if (Filters.GetSortType() != null && item == Filters.GetSortType().ToString())
                {
                    buttonClass = "sortButtonDisable";
                    disabled = "disabled";
                }
                else
                {
                    buttonClass = "sortButton";
                    disabled = "";
                }
                stringBuilder.Append($"<input type = \"submit\" {disabled} class=\"{buttonClass}\"  name=\"sortType\" value=\"{item}\" />");
                stringBuilder.Append($"</form>");
            }
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}

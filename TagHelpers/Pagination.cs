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
    public class Pagination:TagHelper
    {
        public IGetFilterInfoToHtml Filters { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Filters.GetCountPage() > 1)
            {
                output.TagName = "div";
                output.Attributes.SetAttribute("class", "paginationBox");
                output.TagMode = TagMode.StartTagAndEndTag;
                string disabled, disableClass;
                int сurrentPosition = Filters.GetCurrentPosition();
                StringBuilder stringBuilder = new StringBuilder();
                //----------------------------------
                for (int i = 1; i <= Filters.GetCountPage(); i++)
                {
                    stringBuilder.Append($"<form id=\"pag{i}\">");
                    stringBuilder.Append($"<input type=\"hidden\" name=\"currentPosition\" value=\"{i}\" />");

                    stringBuilder.Append(Filters.GetFilterInfoForTagHelper(false));
                    if (i == сurrentPosition)
                    {
                        disabled = "disabled";
                        disableClass = "paginationButtonDisable";
                    }
                    else
                    {
                        disabled = "";
                        disableClass = "paginationButton";
                    }

                    stringBuilder.Append($"<input type = \"submit\"  {disabled} class=\"{disableClass}\" value=\"{i}\" />");
                    stringBuilder.Append($"</form>");
                }
                output.Content.SetHtmlContent(stringBuilder.ToString());
            }
        }
    }
}

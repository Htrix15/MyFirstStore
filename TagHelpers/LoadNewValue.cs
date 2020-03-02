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
    public class LoadNewValue : TagHelper
    {
        public IGetFilterInfoToHtml Filters { get; set; }
        public string ClassName { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Filters.GetCountPage() != Filters.GetCurrentPosition())
            {
                output.TagName = "form";
                output.Attributes.SetAttribute("class", "paginationBox");
                output.TagMode = TagMode.StartTagAndEndTag;

                int сurrentPosition = Filters.GetCurrentPosition();

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"<input type=\"hidden\" name=\"currentPosition\" value=\"{сurrentPosition + 1}\" />");
                stringBuilder.Append($"<input type=\"hidden\" name=\"maxPage\" value=\"{Filters.GetCountPage()}\" />");
                stringBuilder.Append(Filters.GetFilterInfoForTagHelper(false));
                stringBuilder.Append($"<input type=\"button\" class=\"{ClassName} loadNewValue\" value=\"Загрузить еще\"/>");

                output.Content.SetHtmlContent(stringBuilder.ToString());
            }
        }
    }
}

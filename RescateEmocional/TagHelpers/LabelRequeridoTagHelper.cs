using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RescateEmocional.TagHelpers
{
    [HtmlTargetElement("label", Attributes = "asp-for")]
    public class LabelRequeridoTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Obtiene el nombre del campo
            var labelText = AspFor.Metadata.DisplayName ?? AspFor.Name;

            // Verifica si tiene el atributo Required
            var isRequired = AspFor.Metadata.ValidatorMetadata
                .OfType<RequiredAttribute>()
                .Any();

            // Genera el contenido del label
            if (isRequired)
            {
                output.Content.SetHtmlContent($"{labelText} <span style='color: red;'>*</span>");
            }
            else
            {
                output.Content.SetHtmlContent(labelText);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Utilities;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Html.Models;
using OrchardCore.Html.ViewModels;

namespace OrchardCore.Html.Settings
{
    public class HtmlBodyPartMonacoEditorSettingsDriver : ContentTypePartDefinitionDisplayDriver<HtmlBodyPart>
    {
        protected readonly IStringLocalizer S;

        public HtmlBodyPartMonacoEditorSettingsDriver(IStringLocalizer<HtmlBodyPartMonacoEditorSettingsDriver> localizer)
        {
            S = localizer;
        }

        public override IDisplayResult Edit(ContentTypePartDefinition contentTypePartDefinition, IUpdateModel updater)
        {
            return Initialize<MonacoSettingsViewModel>("HtmlBodyPartMonacoSettings_Edit", model =>
            {
                var settings = contentTypePartDefinition.GetSettings<HtmlBodyPartMonacoEditorSettings>();
                if (String.IsNullOrWhiteSpace(settings.Options))
                {
                    settings.Options = JsonConvert.SerializeObject(new { automaticLayout = true, language = "html" }, Formatting.Indented);
                }
                model.Options = settings.Options;
            })
            .Location("Editor");
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentTypePartDefinition contentTypePartDefinition, UpdateTypePartEditorContext context)
        {
            if (contentTypePartDefinition.Editor() == "Monaco")
            {
                var model = new MonacoSettingsViewModel();
                var settings = new HtmlBodyPartMonacoEditorSettings();

                await context.Updater.TryUpdateModelAsync(model, Prefix);

                if (!model.Options.IsJson())
                {
                    context.Updater.ModelState.AddModelError(Prefix + "." + nameof(MonacoSettingsViewModel.Options), S["The options are written in an incorrect format."]);
                }
                else
                {
                    var jsonSettings = JObject.Parse(model.Options);
                    jsonSettings["language"] = "html";
                    settings.Options = jsonSettings.ToString();
                    context.Builder.WithSettings(settings);
                }
            }

            return Edit(contentTypePartDefinition, context.Updater);
        }
    }
}

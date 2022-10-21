using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.Translate;
using Amazon.Translate.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace AIServicesDemo.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Text { get; set; } = String.Empty; 
        public string Result { get; set; } = String.Empty;

        private readonly IAmazonComprehend _comprehendClient;
        private readonly IAmazonTranslate _translateClient;

        public IndexModel(IAmazonComprehend comprehendClient, IAmazonTranslate translateClient)
        {
            _comprehendClient = comprehendClient;
            _translateClient = translateClient;
        }

        public void OnGet()
        {

        }

        public async Task OnPostLanguageAsync()
        {
            var request = new DetectDominantLanguageRequest()
            {
                Text = Text
            };

            var response = await _comprehendClient.DetectDominantLanguageAsync(request);
            var languageCode = response.Languages.First().LanguageCode;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Dominant Language: <b>{0}</b><br>", languageCode);
            stringBuilder.AppendLine("==========================<br>");

            if (languageCode != "en")
            {
                stringBuilder.AppendFormat("Translating from <b>{0}</b>:<br>", languageCode);
                stringBuilder.AppendLine("==========================<br>");

                var translatRequest = new TranslateTextRequest
                {
                    Text = Text,
                    SourceLanguageCode = languageCode,
                    TargetLanguageCode = "en"
                };

                var translatResponse = await _translateClient.TranslateTextAsync(translatRequest);

                stringBuilder.Append(translatResponse?.TranslatedText);
            }

            Result = stringBuilder.ToString();
        }

        public async Task OnPostEntitiesAsync()
        {
            var request = new DetectEntitiesRequest()
            {
                Text = Text,
                LanguageCode = "en"
            };

            var response = await _comprehendClient.DetectEntitiesAsync(request);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Entities:<br>");
            stringBuilder.AppendLine("==========================<br>");

            foreach (var entity in response.Entities)
            {
                stringBuilder.AppendFormat(
                    "Text: <b>{0}</b>, Type: <b>{1}</b>, Score: <b>{2}</b>, Offset: {3}-{4}<br>",
                    entity.Text,
                    entity.Type,
                    entity.Score,
                    entity.BeginOffset,
                    entity.EndOffset);
            }

            Result = stringBuilder.ToString();
        }

        public async Task OnPostPIIAsync()
        {
            var request = new DetectPiiEntitiesRequest()
            {
                Text = Text,
                LanguageCode = "en"
            };

            var response = await _comprehendClient.DetectPiiEntitiesAsync(request);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("PII:<br>");
            stringBuilder.AppendLine("==========================<br>");

            foreach (var entity in response.Entities)
            {
                stringBuilder.AppendFormat(
                    "Text: <b>{0}</b>, Type: <b>{1}</b>, Score: <b>{2}</b>, Offset: {3}-{4}<br>",
                    Text.Substring(entity.BeginOffset, entity.EndOffset - entity.BeginOffset),
                    entity.Type,
                    entity.Score,
                    entity.BeginOffset,
                    entity.EndOffset);
            }

            Result = stringBuilder.ToString();

        }
    }
}
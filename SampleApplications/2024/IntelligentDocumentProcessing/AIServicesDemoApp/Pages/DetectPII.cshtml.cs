using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.Textract;
using Amazon.Textract.Model;

namespace AIServicesDemoApp.Pages
{
    public class DetectPIIModel(IAmazonTextract textractClient, IAmazonComprehend comprehendClient,
            IWebHostEnvironment hostEnvironment)
        : PageModel
    {
        [BindProperty]
        public IFormFile? FormFile { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;

        public void OnGet()
        {
        }
        
        
        public async Task OnPostDetectPIIAsync()
        {
            if (FormFile == null)
            {
                return;
            }
            // save document image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var detectDocumentTextRequest = new DetectDocumentTextRequest()
            {
                Document = new Document { Bytes = formDocument.MemoryStream }
            };

            var detectDocumentTextResponse = await textractClient.DetectDocumentTextAsync(detectDocumentTextRequest);

            var stringBuilder = new StringBuilder();


            foreach (var block in detectDocumentTextResponse.Blocks)
            {
                if (block.BlockType.Value == "LINE")
                {
                    stringBuilder.AppendFormat("{0}\n", block.Text);
                }
            }

            var text = stringBuilder.ToString();
            
            var request = new DetectPiiEntitiesRequest()
            {
                Text = text,
                LanguageCode = "en"
            };

            var response = await comprehendClient.DetectPiiEntitiesAsync(request);

            stringBuilder.Clear();
            stringBuilder.AppendLine("PII:<br>");
            stringBuilder.AppendLine("==========================<br>");

            foreach (var entity in response.Entities)
            {
                stringBuilder.AppendFormat(
                    "Text: <b>{0}</b>, Type: <b>{1}</b>, Score: <b>{2}</b><br>",
                    text.Substring(entity.BeginOffset, entity.EndOffset - entity.BeginOffset),
                    entity.Type,
                    entity.Score);
            }
            
            stringBuilder.AppendLine("==========================<br>");
            stringBuilder.AppendLine("Raw text: <br>");
            stringBuilder.AppendLine(text);
            
            Result = stringBuilder.ToString();
        }
        
        public async Task OnPostDetectPIIBedrockAsync()
        {
            if (FormFile == null)
            {
                return;
            }
            // save document image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;
            
            var runtime = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);

            var request = new ConverseRequest
            {
                ModelId = "anthropic.claude-3-sonnet-20240229-v1:0",
                Messages =
                [
                    new Message
                    {
                        Role = ConversationRole.User,
                        Content =
                        [
                            new ContentBlock
                            {
                                Text = $"Detect all PII information, output list of found entities as JSON with value and type of PII"
                            },
                            new ContentBlock
                            {
                                Image = new ImageBlock
                                {
                                    Format = ImageFormat.Jpeg,
                                    Source = new ImageSource { Bytes = formDocument.MemoryStream }
                                }
                            }
                        ]
                    }
                ]
            };

            var response = await runtime.ConverseAsync(request);
            
            Result = response?.Output?.Message?.Content?[0].Text ?? "";
            
        }
       
    }
}

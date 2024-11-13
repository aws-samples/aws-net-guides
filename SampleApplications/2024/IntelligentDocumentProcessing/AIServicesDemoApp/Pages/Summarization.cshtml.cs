using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Amazon.BedrockRuntime;
using Amazon;
using Amazon.BedrockRuntime.Model;
using Amazon.Textract;
using Amazon.Textract.Model;
using Amazon.Util;

namespace AIServicesDemoApp.Pages
{
    public class TextSummarizationModel(IAmazonTextract textractClient, IWebHostEnvironment hostEnvironment)
        : PageModel
    {
        [BindProperty]
        public IFormFile? FormFile { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;

        public void OnGet()
        {
        }
        
        public async Task OnPostGenerateSummaryWithTextractAsync()
        {
            if (FormFile == null)
            {
                return;
            }
            // save document image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;
            
            // detect document text
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
                    stringBuilder.AppendFormat("{0} ", block.Text);
                }
            }
            
            var text = stringBuilder.ToString();
            stringBuilder.Clear();
            
            var runtime = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);
            
            var bedrockRequest = new MyBedrockRequest
            {
                messages = [new MyBedrockMessage { content = [new MyBedrockContent { text = "Summarize in 5 bullet points, format as HTML list: " + text}]}]
            };
            
            var request = new InvokeModelRequest
            {
                ModelId = "anthropic.claude-3-sonnet-20240229-v1:0",
                Accept = "*/*",
                ContentType = "application/json",
                Body = AWSSDKUtils.GenerateMemoryStreamFromString(JsonSerializer.Serialize(bedrockRequest))
                
            };

            var response = await runtime.InvokeModelAsync(request);
            
            var bedrockResponse = JsonSerializer.Deserialize<MyBedrockResponse>(response.Body);
            var summary = bedrockResponse?.content.First().text;
            
            stringBuilder.AppendLine("Summary:<br>");
            stringBuilder.AppendLine("==========================<br>");

            stringBuilder.AppendLine(summary);
            
            stringBuilder.AppendLine("<br>==========================<br>");
            stringBuilder.AppendLine("Raw text: <br>");
            stringBuilder.AppendLine(text);
            
            Result = stringBuilder.ToString();
        }
        
        public async Task OnPostGenerateSummaryAsync()
        {
            if (FormFile == null)
            {
                return;
            }
            // save document image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var runtime = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);
            
            // Interpolated raw string literals 
            string json = 
                $$"""
                  {
                  "anthropic_version": "bedrock-2023-05-31",
                  "max_tokens": 500,
                  "messages": [
                      {
                        "role": "user",
                        "content": [
                          {
                            "type": "image",
                            "source": {
                              "type": "base64",
                              "media_type": "image/jpeg",
                              "data": "{{formDocument.MemoryStream.AsBase64String()}}"
                             }
                          },
                          {
                            "type": "text",
                            "text": "Summarize in 5 bullet points, format as HTML list"
                          }
                        ]
                      }
                    ]
                  }
                  """;
            
            var request = new InvokeModelRequest
            {
                ModelId = "anthropic.claude-3-sonnet-20240229-v1:0",
                Accept = "*/*",
                ContentType = "application/json",
                Body = AWSSDKUtils.GenerateMemoryStreamFromString(json)
            };

            var response = await runtime.InvokeModelAsync(request);
            
            var bedrockResponse = JsonSerializer.Deserialize<MyBedrockResponse>(response.Body);
            var summary = bedrockResponse?.content.First().text;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Summary:<br>");
            stringBuilder.AppendLine("==========================<br>");
            stringBuilder.AppendLine(summary);
            
            Result = stringBuilder.ToString();
        }
       
        public async Task OnPostGenerateSummaryConverseAsync()
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
                                Text = "Summarize in 5 bullet points, format as HTML list"
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
                ],
                InferenceConfig = new InferenceConfiguration()
                {
                    MaxTokens = 512,
                    Temperature = 1,
                    TopP = 0.999F
                }
            };

            var response = await runtime.ConverseAsync(request);
            var summary = response?.Output?.Message?.Content?[0]?.Text ?? "";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Summary:<br>");
            stringBuilder.AppendLine("==========================<br>");
            stringBuilder.AppendLine(summary);
            
            Result = stringBuilder.ToString();
        }
    }
}

using System.Diagnostics;
using System.Text.Json;
using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon.BedrockRuntime;
using Amazon;
using Amazon.BedrockRuntime.Model;
using Amazon.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace AIServicesDemoApp.Pages
{
    public class QueryModel(IAmazonTextract textractClient, IWebHostEnvironment hostEnvironment)
        : PageModel
    {
        [BindProperty]
        public string Query { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? FormFile { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        

        public void OnGet()
        {
        }

        public async Task OnPostTextractAsync()
        {
            if (FormFile == null)
                return;
            
            // save document image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var queryRequest = new AnalyzeDocumentRequest()
            {
                Document = new Document { Bytes = formDocument.MemoryStream },
                FeatureTypes = ["QUERIES"],
                QueriesConfig = new QueriesConfig
                {
                    Queries = [new Query { Text = Query }]
                }
            };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var queryResponse = await textractClient.AnalyzeDocumentAsync(queryRequest);
            stopwatch.Stop();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Time taken: {0}ms<br>", stopwatch.ElapsedMilliseconds);
            stringBuilder.AppendFormat("Query: <b>{0}</b><br>", Query);

            // Load image to modify with bounding box rectangle
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(formDocument.FileNameWithPath))
            {
                foreach (var block in queryResponse.Blocks)
                {
                    if (block.BlockType.Value == "QUERY_RESULT")
                    {
                        stringBuilder.AppendFormat(
                            "Answer: <b>{0}</b>, Confidence: <b>{1}</b><br>",
                            block.Text,
                            block.Confidence);

                        // Get the bounding box
                        var boundingBox = block.Geometry.BoundingBox;

                        // Draw the rectangle using the bounding box values
                        image.DrawRectangleUsingBoundingBox(boundingBox);
                    }
                }
                
                // Save the new image to display it
                await image.SaveAsJpegAsync(formDocument.FileNameWithPath, new JpegEncoder { ColorType = JpegEncodingColor.Rgb});
            }

            Result = stringBuilder.ToString();
        }
        
        public async Task OnPostBedrockAsync()
        {
            if (FormFile == null)
                return;

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
                            "text": "{{Query}}"
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
            var summary = bedrockResponse?.content.First().text ?? "";

            Result = summary;
        }
        
        public async Task OnPostConverseAsync()
        {
            if (FormFile == null)
                return;

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
                                Text = Query
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

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var response = await runtime.ConverseAsync(request);
            stopwatch.Stop();

            var summary = response?.Output?.Message?.Content?[0]?.Text ?? "";

            var result = new StringBuilder();
            result.AppendLine("Result: <br>");
            result.AppendLine(summary);

            result.AppendLine("<br>==========================<br>");
            result.AppendLine($"Input tokens: {response?.Usage.InputTokens} <br>");
            result.AppendLine($"Output tokens: {response?.Usage.OutputTokens} <br>");
            result.AppendFormat("Time taken: {0}ms<br>", stopwatch.ElapsedMilliseconds);
            
            Result = result.ToString();
        }
    }
}

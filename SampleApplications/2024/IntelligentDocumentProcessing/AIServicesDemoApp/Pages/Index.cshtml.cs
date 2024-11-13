using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Translate;
using Amazon.Translate.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Textract.Model;
using Amazon.Util;

namespace AIServicesDemoApp.Pages;

public class IndexModel(IAmazonComprehend comprehendClient, IAmazonTranslate translateClient)
    : PageModel
{
    [BindProperty]
        public string Text { get; set; } = string.Empty; 
        public string Result { get; set; } = string.Empty;

        public void OnGet()
        {
            
        }

        public async Task OnPostTranslateAsync()
        {
            // detect text of the language
            var request = new DetectDominantLanguageRequest()
            {
                Text = Text
            };
            var response = await comprehendClient.DetectDominantLanguageAsync(request);

            // get fist language from the list
            var languageCode = response.Languages[0].LanguageCode;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Language: <b>{0}</b><br>", languageCode);
            stringBuilder.AppendLine("==========================<br>");

            // translate text to English
            var translateRequest = new TranslateTextRequest()
            {
                Text = Text,
                SourceLanguageCode = languageCode,
                TargetLanguageCode = "en"
            };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var translateResponse = await translateClient.TranslateTextAsync(translateRequest);
            stopwatch.Stop();

            stringBuilder.AppendFormat("Translated Text: {0}<br>", translateResponse.TranslatedText);
            stringBuilder.AppendFormat("Time taken: {0}ms<br>", stopwatch.ElapsedMilliseconds);
            stringBuilder.AppendFormat("Characters: {0}<br>", Text.Length);

            Result = stringBuilder.ToString();
            

        }
        
        public async Task OnPostClaudeTranslateAsync()
        {
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
                            "type": "text",
                            "text": "Translate to English: {{Text}}"
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
            
            Result = bedrockResponse?.content.First().text ?? "";

        }
        
        public async Task OnPostLlamaTranslateAsync()
        {
            var runtime = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);
            
            // Interpolated raw string literals 
            string json = 
                $$"""
                   {
                   "max_gen_len": 512,
                   "prompt": "<s>[INST] Translate to English: {{Text}} [/INST]"
                   }
                   """;
            
            var request = new InvokeModelRequest
            {
                ModelId = "meta.llama2-13b-chat-v1",
                Accept = "*/*",
                ContentType = "application/json",
                Body = AWSSDKUtils.GenerateMemoryStreamFromString(json)
            };

            var response = await runtime.InvokeModelAsync(request);
            
            var modelResponse = await JsonNode.ParseAsync(response.Body);

            // Extract and print the response text.
            Result = modelResponse?["generation"]?.ToString() ?? "";

        }
        
        public async Task OnPostConverseTranslateAsync()
        {
            var runtime = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);

            var request = new ConverseRequest
            {
                ModelId = "anthropic.claude-3-sonnet-20240229-v1:0",
                Messages = [
                    new Message
                    {
                        Role = ConversationRole.User,
                        Content = [
                            new ContentBlock
                            {
                                Text = $"Translate to English: {Text}"
                            }
                        ]
                    }
                ]
            };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var response = await runtime.ConverseAsync(request);
            stopwatch.Stop();
            
            var result = new StringBuilder();
            result.AppendLine("Result: <br>");
            result.AppendLine(response?.Output?.Message?.Content?[0].Text ?? "");

            result.AppendLine("<br>==========================<br>");
            result.AppendLine($"Input tokens: {response?.Usage.InputTokens} <br>");
            result.AppendLine($"Output tokens: {response?.Usage.OutputTokens} <br>");
            result.AppendFormat("Time taken: {0}ms<br>", stopwatch.ElapsedMilliseconds);
            
            Result = result.ToString();
            

        }

        public async Task OnPostEntitiesAsync()
        {
            // detect entities
            var request = new DetectEntitiesRequest()
            {
                Text = Text,
                LanguageCode = "en"
            };
            var response = await comprehendClient.DetectEntitiesAsync(request);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Entities:<br>");
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

        public async Task OnPostPIIAsync()
        {
            var request = new DetectPiiEntitiesRequest()
            {
                Text = Text,
                LanguageCode = "en"
            };

            var response = await comprehendClient.DetectPiiEntitiesAsync(request);

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
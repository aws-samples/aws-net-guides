using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Textract.Model;
using SixLabors.ImageSharp;

namespace AIServicesDemoApp.Pages
{
    public class IdentityCheckModel(IAmazonRekognition rekognitionClient, IWebHostEnvironment hostEnvironment)
        : PageModel
    {
        [BindProperty]
        public IFormFile? IdentityFormFile { get; set; }
        [BindProperty]
        public IFormFile? PhotoFormFile { get; set; }
        public string IdentityFileName { get; set; } = String.Empty;
        public string PhotoFileName { get; set; } = String.Empty;
        public string Result { get; set; } = String.Empty;

        public void OnGet()
        {
        }

        public async Task OnPostIdentitiesAsync()
        {
            if ((IdentityFormFile == null) || (PhotoFormFile == null))
            {
                return;
            }
            // save id to display it
            var identityDocument = await Helpers.SaveImage(IdentityFormFile, hostEnvironment.WebRootPath);
            IdentityFileName = identityDocument.FileName;

            // save photo to display it
            var photoDocument = await Helpers.SaveImage(PhotoFormFile, hostEnvironment.WebRootPath);
            PhotoFileName = photoDocument.FileName;

            // create request
            var compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = new Amazon.Rekognition.Model.Image { Bytes = identityDocument.MemoryStream },
                TargetImage = new Amazon.Rekognition.Model.Image { Bytes = photoDocument.MemoryStream }
            };

            var compareFacesResponse = await rekognitionClient.CompareFacesAsync(compareFacesRequest);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Text:<br>");
            stringBuilder.AppendLine("==========================<br>");

            if (compareFacesResponse.FaceMatches.Count > 0)
            {
                stringBuilder.AppendFormat(
                    "Face similarity: <b>{0}</b><br>",
                    compareFacesResponse.FaceMatches[0].Similarity);

                // Load image to modify with face bounding box rectangle
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(photoDocument.FileNameWithPath))
                {
                    var faceDetail = compareFacesResponse.FaceMatches[0].Face;

                    // Get the bounding box
                    var boundingBox = faceDetail.BoundingBox;

                    // Draw the rectangle using the bounding box values
                    image.DrawRectangleUsingBoundingBox(boundingBox);

                    // Save the new image
                    await image.SaveAsJpegAsync(photoDocument.FileNameWithPath);
                }
            }
            else
            {
                stringBuilder.AppendLine("No matching faces");
            }

            Result = stringBuilder.ToString();

        }
        
        public async Task OnPostIdentitiesBedrockAsync()
        {
            if ((IdentityFormFile == null) || (PhotoFormFile == null))
            {
                return;
            }
            // save id to display it
            var identityDocument = await Helpers.SaveImage(IdentityFormFile, hostEnvironment.WebRootPath);
            IdentityFileName = identityDocument.FileName;

            // save photo to display it
            var photoDocument = await Helpers.SaveImage(PhotoFormFile, hostEnvironment.WebRootPath);
            PhotoFileName = photoDocument.FileName;

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
                                Text = "Compare persons in these 2 images, identify if it is the same person."
                            },
                            new ContentBlock
                            {
                                Image = new ImageBlock
                                {
                                    Format = ImageFormat.Jpeg,
                                    Source = new ImageSource { Bytes = identityDocument.MemoryStream }
                                }
                            },
                            new ContentBlock
                            {
                                Image = new ImageBlock
                                {
                                    Format = ImageFormat.Jpeg,
                                    Source = new ImageSource { Bytes = photoDocument.MemoryStream }
                                }
                            }
                        ]
                    }
                ]
            };

            var response = await runtime.ConverseAsync(request);
            var summary = response?.Output?.Message?.Content?[0]?.Text ?? "";

            Result = summary;

        }
    }
}

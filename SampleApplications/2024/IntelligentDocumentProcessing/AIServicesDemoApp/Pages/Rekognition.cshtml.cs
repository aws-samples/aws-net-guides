using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace AIServicesDemoApp.Pages
{
    public class RekognitionModel(IAmazonRekognition rekognitionClient, IWebHostEnvironment hostEnvironment)
        : PageModel
    {
        [BindProperty]
        public IFormFile? FormFile { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        

        public void OnGet()
        {
        }

        public async Task OnPostFacesAsync()
        {
            if (FormFile == null)
                return;
            
            // save image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var detectFacesRequest = new DetectFacesRequest()
            {
                Image = new Amazon.Rekognition.Model.Image { Bytes = formDocument.MemoryStream }
            };

            var detectFacesResponse = await rekognitionClient.DetectFacesAsync(detectFacesRequest);

            if (detectFacesResponse.FaceDetails.Count > 0)
            {
                // Load image to modify with face bounding box rectangle
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(formDocument.FileNameWithPath))
                {
                    foreach (var faceDetail in detectFacesResponse.FaceDetails)
                    {
                        // Get the bounding box
                        var boundingBox = faceDetail.BoundingBox;

                        // Draw the rectangle using the bounding box values
                        image.DrawRectangleUsingBoundingBox(boundingBox);
                    }

                    // Save the new image
                    await image.SaveAsJpegAsync(formDocument.FileNameWithPath, new JpegEncoder { ColorType = JpegEncodingColor.Rgb});
                }
            }
        }

        public async Task OnPostEntitiesAsync()
        {
            if (FormFile == null)
                return;
            
            // save image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var detectLabelsRequest = new DetectLabelsRequest()
            {
                Image = new Amazon.Rekognition.Model.Image { Bytes = formDocument.MemoryStream }
            };

            var detectLabelsResponse = await rekognitionClient.DetectLabelsAsync(detectLabelsRequest);


            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Labels:<br>");
            stringBuilder.AppendLine("==========================<br>");

            foreach (var label in detectLabelsResponse.Labels)
            {
                stringBuilder.AppendFormat(
                    "Label: <b>{0}</b>, Confidence: <b>{1}</b><br>",
                    label.Name,
                    label.Confidence);
            }

            Result = stringBuilder.ToString();

        }

        public async Task OnPostPPEAsync()
        {
            if (FormFile == null)
                return;
            
            // save image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var request = new DetectProtectiveEquipmentRequest()
            {
                Image = new Amazon.Rekognition.Model.Image { Bytes = formDocument.MemoryStream }
            };

            var response = await rekognitionClient.DetectProtectiveEquipmentAsync(request);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("PPE:<br>");
            stringBuilder.AppendLine("==========================<br>");

            foreach (var person in response.Persons)
            {
                foreach (var bodyPart in person.BodyParts)
                {
                    foreach (var eq in bodyPart.EquipmentDetections)
                    {
                        stringBuilder.AppendFormat(
                            "Body part: <b>{0}</b>, Type: <b>{1}</b>, Covered: <b>{2}</b><br>",
                            bodyPart.Name.Value,
                            eq.Type.Value,
                            eq.CoversBodyPart.Value);
                    }
                }

                Result = stringBuilder.ToString();

            }
        }

        public async Task OnPostTextAsync()
        {
            if (FormFile == null)
                return;
            
            // save image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var detectTextRequest = new DetectTextRequest()
            {
                Image = new Amazon.Rekognition.Model.Image { Bytes = formDocument.MemoryStream }
            };

            var detectTextResponse = await rekognitionClient.DetectTextAsync(detectTextRequest);
            
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Text:<br>");
            stringBuilder.AppendLine("==========================<br>");

            foreach (var text in detectTextResponse.TextDetections)
            {
                if (text.Type == TextTypes.LINE)
                {
                    stringBuilder.AppendFormat(
                        "Detected text: <b>{0}</b><br>",
                        text.DetectedText);
                }

                Result = stringBuilder.ToString();
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon.Textract;
using Amazon.Textract.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace AIServicesDemoApp.Pages
{
    public class DetectSignaturesModel(IAmazonTextract textractClient, IWebHostEnvironment hostEnvironment)
        : PageModel
    {
        [BindProperty]
        public IFormFile? FormFile { get; set; }
        public string FileName { get; set; } = string.Empty;

        public void OnGet()
        {
        }
        
        public async Task OnPostDetectSignaturesAsync()
        {
            if (FormFile == null)
                return;

            // save document image to display it
            var formDocument = await Helpers.SaveImage(FormFile, hostEnvironment.WebRootPath);
            FileName = formDocument.FileName;

            var analyzeDocumentRequest = new AnalyzeDocumentRequest
            {
                Document = new Document { Bytes = formDocument.MemoryStream },
                FeatureTypes = ["SIGNATURES"]
            };

            var analyzeDocumentResponse = await textractClient.AnalyzeDocumentAsync(analyzeDocumentRequest);
            
            // Load image to modify with bounding box rectangle
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(formDocument.FileNameWithPath))
            {
                foreach (var block in analyzeDocumentResponse.Blocks)
                {
                    if (block.BlockType.Value == "SIGNATURE")
                    {
                        // Get the bounding box
                        var boundingBox = block.Geometry.BoundingBox;

                        // Draw the rectangle using the bounding box values
                        image.DrawRectangleUsingBoundingBox(boundingBox);
                    }
                }

                // Save the new image
                await image.SaveAsJpegAsync(formDocument.FileNameWithPath, new JpegEncoder { ColorType = JpegEncodingColor.Rgb});
            }
        }
    }
}

using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MediaLibrary.Models;
using MediaLibrary.Services;

namespace MediaLibrary.Controllers
{
    public class RekognitionController : Controller
    {

        private readonly AwsSettings _configuration;
        private readonly IFileMetadataService _metadataService;
        private readonly IImageMetadataService _imageMetadataService;
        private readonly IImageLookupService _imageLookupService;
        private readonly ILogger _logger;

        public RekognitionController(IOptions<AwsSettings> options, IFileMetadataService metadataService, IImageMetadataService imageMetadataService, ILogger<RekognitionController> logger, IImageLookupService imageLookupService)
        {
            _configuration = options.Value;
            _metadataService = metadataService;
            _imageMetadataService = imageMetadataService;
            _imageLookupService = imageLookupService;
            _logger = logger;
        }

        public async Task<IActionResult> ViewItem(string item)
        {
            _logger.Log(LogLevel.Information, "RekognitionController::ViewItem", null);
            AWSXRayRecorder.Instance.BeginSubsegment("RekognitionController::ViewItem");

            try
            {
                var fileMetadata = await _metadataService.GetFileMetadata(item);

                ImageProcessingModel viewModel = new ImageProcessingModel();
                viewModel.Labels = new List<string>();
                viewModel.ImageUri = fileMetadata.ImageURL;
                viewModel.ImageName = fileMetadata.KeyName;
                viewModel.OriginalName = fileMetadata.OrigionalFileName;

                var imageMetadata = await _imageMetadataService.GetImageData(item);
                foreach (var label in imageMetadata.Labels)
                {
                    viewModel.Labels.Add(label);
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                AWSXRayRecorder.Instance.AddException(ex);
                throw;
            }
            finally
            {
                AWSXRayRecorder.Instance.EndSubsegment();
            }
        }

        public async Task<IActionResult> Process(string item)
        {
            _logger.Log(LogLevel.Information, "RekognitionController::Process", null);
            AWSXRayRecorder.Instance.BeginSubsegment("RekognitionController::Process - Getting File Metadata");
            var fileMetadata = await _metadataService.GetFileMetadata(item);
            AWSXRayRecorder.Instance.EndSubsegment();

            ImageProcessingModel viewModel = new ImageProcessingModel();
            viewModel.Labels = new List<string>();
            viewModel.ImageUri = fileMetadata.ImageURL;
            viewModel.ImageName = fileMetadata.KeyName;
            viewModel.OriginalName = fileMetadata.OrigionalFileName;

            // Run Rekognition on the element.
            Amazon.Rekognition.AmazonRekognitionClient client = new Amazon.Rekognition.AmazonRekognitionClient();
            try
            {
                _logger.Log(LogLevel.Information, "Calling Rekognition to detect imeage labels.", null);
                AWSXRayRecorder.Instance.BeginSubsegment("RekognitionController::Process - Getting Image Labels");

                var detectLabelsRequest = new Amazon.Rekognition.Model.DetectLabelsRequest()
                {
                    MaxLabels = 100,
                    MinConfidence = 75F,
                    Image = new Amazon.Rekognition.Model.Image()
                    {
                        S3Object = new Amazon.Rekognition.Model.S3Object()
                        {
                            Bucket = _configuration.BucketName,
                            Name = fileMetadata.KeyName
                        }
                    }
                };

                var labels = await client.DetectLabelsAsync(detectLabelsRequest);

                // Store the new data in a new dynamo DB Table
                foreach (var label in labels.Labels)
                {
                    float confidence = label.Confidence;
                    viewModel.Labels.Add(label.Name);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                _logger.Log(LogLevel.Error, ex, ex.Message, null);
            }
            finally
            {
                AWSXRayRecorder.Instance.EndSubsegment();
            }
            // Sore individual indexes 
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Process(ImageProcessingModel model, IFormCollection collection)
        {
            _logger.Log(LogLevel.Information, "RekognitionController::Process - Saving data to Dynamo DB", null);
            List<string> photoLabels = new List<string>();

            foreach (string photoLabel in collection.Keys)
            {
                switch (photoLabel)
                {
                    case "ImageUri":
                    case "OriginalName":
                    case "ImageName":
                    case "__RequestVerificationToken":
                        break;
                    default:
                        photoLabels.Add(photoLabel);
                        var lookupData = await _imageLookupService.GetLookupData(photoLabel);
                        if (lookupData == null)
                        {
                            lookupData = new ImageLookupDataModel();
                            lookupData.Images = new List<string>();
                            lookupData.Label = photoLabel;
                        }
                        if (!lookupData.Images.Contains(model.ImageName))
                        {
                            lookupData.Images.Add(model.ImageName);
                            _imageLookupService.SaveLookupData(lookupData);
                        }
                        break;
                }
            }

            // Write the image data to the Dynamo DB Table
            ImageMetadataDataModel imageMetaData = new ImageMetadataDataModel()
            {
                Image = model.ImageName,
                Labels = photoLabels
            };
            _imageMetadataService.SaveImageData(imageMetaData);

            // Update the file data entry to indicate that the image has been saved.
            var fileMetadata = await _metadataService.GetFileMetadata(model.ImageName);
            fileMetadata.IsProcessed = true;
            _metadataService.SaveMetadata(fileMetadata);

            return RedirectToAction("Index", "FileManagement");
        }

        public async Task<IActionResult> Search(string filter)
        {
            var fullLabelData = await _imageLookupService.GetLabelData();
            ImageLookupDataModel imageLookupList;

            SearchViewModel viewModel = new SearchViewModel();
            viewModel.Items = new List<FileMetadataDataModel>();

            if (!string.IsNullOrEmpty(filter))
            {
                viewModel.CurrentValue = filter;
                imageLookupList = (from x in fullLabelData
                                   where x.Label.ToLower() == filter.ToLower()
                                   select x).First();

                if (imageLookupList != null)
                {
                    foreach (var imageLookup in imageLookupList.Images)
                    {
                        viewModel.Items.Add(await _metadataService.GetFileMetadata(imageLookup));
                    }
                }
            }
            else
            {
                viewModel.CurrentValue = "---Choose Label---";
            }

            viewModel.Labels = new List<string>();

            foreach (var item in fullLabelData)
            {
                if (!viewModel.Labels.Contains(item.Label))
                {
                    viewModel.Labels.Add(item.Label);
                }
            }



            return View(viewModel);
        }
    }
}
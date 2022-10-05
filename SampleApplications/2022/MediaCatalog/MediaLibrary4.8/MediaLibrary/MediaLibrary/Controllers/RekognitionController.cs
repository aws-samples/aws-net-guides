using MediaLibrary.Models;
using MediaLibrary.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MediaLibrary.Controllers
{
    public class RekognitionController : Controller
    {
        private readonly AwsSettings _configuration;
        private readonly S3StorageService _storage;
        private readonly DynamoFileMetadataService _metadataService;
        private readonly DynamoImageMetadataService _imageService;
        private readonly DynamoImageLookupService _imageLookupService;
        private readonly RekognitionModerationService _moderationService;
        public RekognitionController()
        {
            _configuration = new AwsSettings();
            _storage = new S3StorageService();
            _metadataService = new DynamoFileMetadataService();
            _imageService = new DynamoImageMetadataService();
            _imageLookupService = new DynamoImageLookupService();
            _moderationService = new RekognitionModerationService();
        }

        public async Task<ActionResult> ViewItem(string item)
        {
            //_logger.Log(LogLevel.Information, "RekognitionController::ViewItem", null);
            try
            {
                var fileMetadata = await _metadataService.GetFileMetadata(item);
                ImageProcessingModel viewModel = new ImageProcessingModel();
                viewModel.Labels = new List<string>();
                viewModel.ImageUri = fileMetadata.ImageURL;
                viewModel.ImageName = fileMetadata.KeyName;
                viewModel.OriginalName = fileMetadata.OrigionalFileName;
                var imageMetadata = await _imageService.GetImageData(item);
                foreach (var label in imageMetadata.Labels)
                {
                    viewModel.Labels.Add(label);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }
        }

        public async Task<ActionResult> Process(string item)
        {
            //_logger.Log(LogLevel.Information, "RekognitionController::Process", null);
            var fileMetadata = await _metadataService.GetFileMetadata(item);
            ImageProcessingModel viewModel = new ImageProcessingModel();
            viewModel.Labels = new List<string>();
            viewModel.ImageUri = fileMetadata.ImageURL;
            viewModel.ImageName = fileMetadata.KeyName;
            viewModel.OriginalName = fileMetadata.OrigionalFileName;
            // Run Rekognition on the element.
            Amazon.Rekognition.AmazonRekognitionClient client = new Amazon.Rekognition.AmazonRekognitionClient();
            try
            {
                //_logger.Log(LogLevel.Information, "Calling Rekognition to detect imeage labels.", null);
                var detectLabelsRequest = new Amazon.Rekognition.Model.DetectLabelsRequest()
                {MaxLabels = 100, //MinConfidence = 75F,
 Image = new Amazon.Rekognition.Model.Image()
                {S3Object = new Amazon.Rekognition.Model.S3Object()
                {Bucket = _configuration.BucketName, Name = fileMetadata.KeyName}}};
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
            //_logger.Log(LogLevel.Error, ex, ex.Message, null);
            }
            finally
            {
            }

            // Sore individual indexes 
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Process(ImageProcessingModel model, FormCollection collection)
        {
            //_logger.Log(LogLevel.Information, "RekognitionController::Process - Saving data to Dynamo DB", null);
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
            {Image = model.ImageName, Labels = photoLabels};
            _imageService.SaveImageData(imageMetaData);
            // Update the file data entry to indicate that the image has been saved.
            var fileMetadata = await _metadataService.GetFileMetadata(model.ImageName);
            fileMetadata.IsProcessed = true;
            _metadataService.SaveMetadata(fileMetadata);
            return RedirectToAction("Index", "FileManagement");
        }

        public async Task<ActionResult> Search(string filter)
        {
            var fullLabelData = await _imageLookupService.GetLabelData();
            ImageLookupDataModel imageLookupList;
            SearchViewModel viewModel = new SearchViewModel();
            viewModel.Items = new List<FileMetadataDataModel>();
            if (!string.IsNullOrEmpty(filter))
            {
                viewModel.CurrentValue = filter;
                imageLookupList = (
                    from x in fullLabelData
                    where x.Label.ToLower() == filter.ToLower()select x).First();
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
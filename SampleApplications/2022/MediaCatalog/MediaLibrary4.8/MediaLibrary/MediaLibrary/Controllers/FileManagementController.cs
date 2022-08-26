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
    public class FileManagementController : Controller
    {
        private readonly AwsSettings _configuration;
        private readonly S3StorageService _storage;
        private readonly DynamoFileMetadataService _metadataService;
        private readonly DynamoImageMetadataService _imageService;
        private readonly DynamoImageLookupService _imageLookupService;
        private readonly RekognitionModerationService _moderationService;
        //private readonly ILogger _logger;


        public FileManagementController ()
        {
            _configuration = new AwsSettings();
            _storage = new S3StorageService();
            _metadataService = new DynamoFileMetadataService();
            _imageService = new DynamoImageMetadataService();
            _imageLookupService = new DynamoImageLookupService();
            _moderationService = new RekognitionModerationService();
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                //_logger.Log(LogLevel.Information, "FileManagementController::Index");
                var list = await _metadataService.GetFileList();

                return View(list);
            }
            catch (Exception ex)
            {
                //_logger.Log(LogLevel.Error, ex, ex.Message);
                return View();
            }

        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            try
            {
                //_logger.Log(LogLevel.Information, "FileManagementController::Upload");

                //string ip = HttpContext.Connection.RemoteIpAddress.ToString();
                //_logger.Log(LogLevel.Information, "Received file upload request from {0} for file {1}", ip, file.FileName);

                string s3KeyName = _storage.SaveFile(file);
                //_logger.Log(LogLevel.Information, "Saved file {0} to S3 bucket as {1}", file.FileName, s3KeyName);

                var moderationResults = await _moderationService.IsContentAllowed(s3KeyName);
                if (moderationResults.ImageAllowed)
                {
                    // Need to formulate the metadata for the file.
                    var metadata = new FileMetadataDataModel()
                    {
                        OrigionalFileName = file.FileName,
                        FileType = Path.GetExtension(file.FileName),
                        KeyName = s3KeyName,
                        TimeStamp = DateTime.Now,
                        ImageURL = String.Format("https://{0}/{1}", new[] { _configuration.CloudFrontDNS, s3KeyName })
                    };
                    _metadataService.SaveMetadata(metadata);
                    return RedirectToAction("Index");
                }
                else
                {
                    await _storage.DeleteFile(s3KeyName);
                    //_logger.Log(LogLevel.Information, "The image contained in file {0} has been rejected for the following reasons: {1}", file.FileName, moderationResults.ModerationFlags);

                    return View("FileNotAllowed", moderationResults);
                }
            }
            catch (Exception ex)
            {
                //_logger.Log(LogLevel.Error, ex, ex.Message);
                return View();
            }
        }

        public async Task<ActionResult> Delete(string item)
        {
            try
            {
                DeleteImageViewModel deleteImageViewModel = new DeleteImageViewModel();

                var fileMetadata = await _metadataService.GetFileMetadata(item);

                deleteImageViewModel.ImageUrl = fileMetadata.ImageURL;
                deleteImageViewModel.KeyName = item;

                return View(deleteImageViewModel);
            }
            catch (Exception ex)
            {
                //_logger.Log(LogLevel.Error, ex, ex.Message);
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(FormCollection formData, DeleteImageViewModel data)
        {
            try
            {
                FileMetadataDataModel fileMetadata;
                ImageMetadataDataModel imageData;

                // string ip = HttpContext.Session. .Connection.RemoteIpAddress.ToString();
                //_logger.Log(LogLevel.Information, "Received file delete request from {0} for file {1}. Delete type {2}.", ip, data.ImageUrl, data.DeleteType);

                switch (data.DeleteType)
                {
                    case "Image":
                        // Delete the image and all data associated with the image.
                        if (await _storage.DeleteFile(data.KeyName))
                        {
                            fileMetadata = await _metadataService.GetFileMetadata(data.KeyName);
                            await _metadataService.DeleteFileMetadata(fileMetadata);

                            imageData = await _imageService.GetImageData(data.KeyName);
                            await _imageService.DeleteImageData(imageData);

                            // Need to add functionality to clean up the cross referance tabble.
                            await _imageLookupService.RemoveImageFromLookups(data.KeyName);
                            //_logger.Log(LogLevel.Information, "Delete image and data successful.");
                        }
                        break;
                    case "Data":
                        // Delete just the meta data, resetting the image.
                        fileMetadata = await _metadataService.GetFileMetadata(data.KeyName);
                        fileMetadata.IsProcessed = false;
                        _metadataService.SaveMetadata(fileMetadata);

                        imageData = await _imageService.GetImageData(data.KeyName);
                        await _imageService.DeleteImageData(imageData);

                        // Need to add functionality to clean up the cross referance tabble.
                        await _imageLookupService.RemoveImageFromLookups(data.KeyName);
                        //_logger.Log(LogLevel.Information, "Delete data successful. Image has been reset.");
                        break;
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //_logger.Log(LogLevel.Error, ex, ex.Message);
                return View();
            }
        }

        public ActionResult FileNotAllowed(ModerationResultsViewModel reason)
        {
            return View(reason);
        }
    }
}
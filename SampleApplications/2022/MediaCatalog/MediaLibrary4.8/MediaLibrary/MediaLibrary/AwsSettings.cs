using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaLibrary
{
    public class AwsSettings
    {
        private static readonly string _bucketName;
        private static readonly string _cloudFrontAcessDNSName;
        private static readonly string _metadataTableName;
        private static readonly string _imagesTableName;
        private static readonly string _lookupTableName;
        static AwsSettings()
        {
            Amazon.SimpleSystemsManagement.AmazonSimpleSystemsManagementClient ssm = new Amazon.SimpleSystemsManagement.AmazonSimpleSystemsManagementClient();
            // Get the name of the S3 Bucket for the application to store uploaded images.
            var s3ParameterRequest = ssm.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest()
            {
                Name = "Media-Bucket-name"
            });
            if (s3ParameterRequest != null)
            {
                s3ParameterRequest.Wait();
                var parameter = s3ParameterRequest.Result.Parameter;

                _bucketName = parameter.Value;
            }
            // Get the name of the Cloud Front Distribution that was created for the application.
            // End users will only be able to access the images that have been uploaded via the Cloud Front Distribution
            // as the created S3 bucket does not have public access enabled. This is inaccordance with security best practices.
            var cloudFrontParameterRequest = ssm.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest()
            {
                Name = "Cloud-Front-URL"
            });
            if (cloudFrontParameterRequest != null)
            {
                cloudFrontParameterRequest.Wait();
                var parameter = cloudFrontParameterRequest.Result.Parameter;

                _cloudFrontAcessDNSName = parameter.Value;
            }

            try
            {
                // Get the name of the DynamoDB Table that the application will use to record the metadata for files that have been uploaded.
                var dynamoParameterRequest = ssm.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest()
                {
                    Name = "File-Metadata-Table"
                });
                if (dynamoParameterRequest != null)
                {
                    dynamoParameterRequest.Wait();
                    var parameter = dynamoParameterRequest.Result.Parameter;

                    _metadataTableName = parameter.Value;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            try
            {
                // Get the name of the DynamoDB Table that the application will use to record the labels for files that have been uploaded.
                var imageParameterRequest = ssm.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest()
                {
                    Name = "Image-Metadata-Table"
                });
                if (imageParameterRequest != null)
                {
                    imageParameterRequest.Wait();
                    var parameter = imageParameterRequest.Result.Parameter;

                    _imagesTableName = parameter.Value;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            try
            {
                // Get the name of the DynamoDB Table that the application will use to record the labels for files that have been uploaded.
                var labelsParameterRequest = ssm.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest()
                {
                    Name = "Image-Metadata-Table"
                });
                if (labelsParameterRequest != null)
                {
                    labelsParameterRequest.Wait();
                    var parameter = labelsParameterRequest.Result.Parameter;

                    _imagesTableName = parameter.Value;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            try
            {
                // Get the name of the DynamoDB Table that the application will use to cross referance images with labels.
                var lookupParameterRequest = ssm.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest()
                {
                    Name = "Lookup-Metadata-Table"
                });
                if (lookupParameterRequest != null)
                {
                    lookupParameterRequest.Wait();
                    var parameter = lookupParameterRequest.Result.Parameter;

                    _lookupTableName = parameter.Value;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

        }
        public string Region { get; set; }
        public string BucketName
        {
            get
            {
                return _bucketName;
            }
        }

        public string CloudFrontDNS
        {
            get
            {
                return _cloudFrontAcessDNSName;
            }
        }

        public string MetadataTableName
        {
            get
            {
                return _metadataTableName;
            }
        }

        public string ImagesTableName
        {
            get
            {
                return _imagesTableName;
            }
        }

        public string LookupTableName
        {
            get
            {
                return _lookupTableName;
            }
        }
    }
}
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.XRay.Recorder.Core;
using MediaLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaLibrary.Services
{
    public class DynamoImageLookupService : IImageLookupService
    {
        private AmazonDynamoDBClient _dynamoDBclient;

        public DynamoImageLookupService()
        {
            _dynamoDBclient = new AmazonDynamoDBClient();
        }

        public async void SaveLookupData(ImageLookupDataModel data)
        {
            DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
            await context.SaveAsync(data);
        }

        public async Task<ImageLookupDataModel> GetLookupData(string itemId)
        {
            try
            {
                DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);

                // you can add scan conditions, or leave empty
                ImageLookupDataModel lookupData = await context.LoadAsync<ImageLookupDataModel>(itemId);

                return lookupData;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

        }

        public async Task<int> RemoveImageFromLookups(string imageName)
        {
            try
            {
                
                List<string> labels = new List<string>();

                List<ScanCondition> conditionList = new List<ScanCondition>();
                conditionList.Add(new ScanCondition("Images", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Contains, new[] { imageName }));

                DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
                var scanReturn = context.ScanAsync<ImageLookupDataModel>(conditionList);
                var scanResults = await scanReturn.GetRemainingAsync();

                foreach (var result in scanResults)
                {
                    result.Images.Remove(imageName);
                    if (result.Images.Count > 0)
                    {
                        // Update the label with the new data.
                        await context.SaveAsync(result);
                    }
                    else
                    {
                        // Delete the record.
                        await context.DeleteAsync<ImageLookupDataModel>(result);
                    }
                }

                return scanResults.Count;
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
                throw;
            }
            finally
            {
            }
        }
        public async Task<IEnumerable<ImageLookupDataModel>> GetLabelData()
        {
            try
            {
                DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);

                // you can add scan conditions, or leave empty

                var queryResult = context.FromScanAsync<ImageLookupDataModel>(new Amazon.DynamoDBv2.DocumentModel.ScanOperationConfig());
                var labelData = await queryResult.GetRemainingAsync();
                return labelData;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }
        }
    }
}

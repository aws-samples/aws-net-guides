using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using MediaLibrary.Models;
using System;
using System.Threading.Tasks;

namespace MediaLibrary.Services
{
    public class DynamoImageMetadataService : IImageMetadataService
    {
        private AmazonDynamoDBClient _dynamoDBclient;

        public DynamoImageMetadataService ()
        {
            _dynamoDBclient = new AmazonDynamoDBClient();
        }

        public async void SaveImageData(ImageMetadataDataModel data)
        {
            DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
            await context.SaveAsync(data);
        }

        public async Task<ImageMetadataDataModel> GetImageData(string itemId)
        {
            try
            {
                DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);

                // you can add scan conditions, or leave empty
                ImageMetadataDataModel imageData = await context.LoadAsync<ImageMetadataDataModel>(itemId);

                return imageData;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

        }

        public async Task<bool> DeleteImageData(ImageMetadataDataModel imageData)
        {
            DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
            try
            {
                await context.DeleteAsync<ImageMetadataDataModel>(imageData);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}

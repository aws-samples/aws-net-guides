using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MediaLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaLibrary.Services
{
    public class DynamoFileMetadataService : IFileMetadataService
    {
        private AmazonDynamoDBClient _dynamoDBclient;
        public DynamoFileMetadataService ()
        {
            _dynamoDBclient = new AmazonDynamoDBClient();


        }
        public async void SaveMetadata(FileMetadataDataModel data)
        {
            DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
            var  saveTask =  context.SaveAsync(data);
            saveTask.Wait();
        }

        public async Task<IEnumerable<FileMetadataDataModel>> GetFileList()
        {
            try
            {
                IEnumerable<FileMetadataDataModel> list;

                DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
                var conditions = new List<ScanCondition>();
                // you can add scan conditions, or leave empty


                var queryResult = await context.ScanAsync<FileMetadataDataModel>(conditions).GetRemainingAsync();

                return queryResult;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

        }

        public async Task<FileMetadataDataModel> GetFileMetadata(string itemId)
        {
            try
            {
                DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);

                // you can add scan conditions, or leave empty
                FileMetadataDataModel fileData = await context.LoadAsync<FileMetadataDataModel>(itemId);

                return fileData;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

        }

        public async Task<bool> DeleteFileMetadata(FileMetadataDataModel fileData)
        {
            DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
            try
            {
                await context.DeleteAsync<FileMetadataDataModel>(fileData);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

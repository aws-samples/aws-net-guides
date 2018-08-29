using System;
using System.Collections.Generic;
using System.Text;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DAX;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;


namespace AWSAppService
{
    namespace Data
    {
        public class DBDataService<T> : IAWSAppService<IData>
        {
            private AmazonDynamoDBClient dynamoDBClient;
     
            private DynamoDBContext dynamoDBContext;
            private CognitoAWSCredentials _awsCreds;
            private Amazon.RegionEndpoint _awsRegion;
            private AWSOptions _awsOptions;

            public DBDataService(IOptions<AWSOptions> awsOptions)
            {
                _awsOptions = awsOptions.Value;
                _awsRegion = Amazon.RegionEndpoint.GetBySystemName(
                                            _awsOptions.Region);
                _awsCreds = new CognitoAWSCredentials(
                                        _awsOptions.CognitoPoolId, _awsRegion
                                        );
                if (!InitService()) throw new AmazonDynamoDBException("Couldn't connect to DynamoDb Service", new Exception("InitService - DynamoClient"));
            }

            public bool InitService()
            {
                dynamoDBClient = new AmazonDynamoDBClient(_awsCreds, _awsRegion);

                return dynamoDBClient != null ? true : false;
            }
           
            public async Task<T> RetrieveDataFromDB<T>(int DataID)
            {
         
                T dataResult = default(T);

                if (dynamoDBContext == null) { dynamoDBContext = new DynamoDBContext(dynamoDBClient); }

                try
                {
                    dataResult = await dynamoDBContext.LoadAsync<T>(DataID);
                   
                }
                catch (AmazonDynamoDBException dbException) { Console.WriteLine(dbException.Message); }
                catch (AmazonServiceException dbSvcException) { Console.WriteLine(dbSvcException.Message); }


                return dataResult;
            }

            public async void PostToDB<T>(T data)
            {
                if(dynamoDBContext == null)
                {
                    dynamoDBContext = new DynamoDBContext(dynamoDBClient);
                }

                try
                {
                    await dynamoDBContext.SaveAsync<T>(data);
                }
                catch (AmazonDynamoDBException dbException) { Console.WriteLine(dbException.Message); }
                catch (AmazonServiceException dbSvcException) { Console.WriteLine(dbSvcException.Message); }
            }

            public async Task<int> GetItemCount<T>()
            {
                int totalItems = 0;
                if (dynamoDBContext == null)
                {
                    dynamoDBContext = new DynamoDBContext(dynamoDBClient);
                }

                try
                {   //try to re-write to avoid doing full table scan as performance costs are extensive. Atomic counter table maintaining one additional table on dynamodb? NK
                    var itemBatch = dynamoDBContext.CreateBatchGet<T>();
                    await itemBatch.ExecuteAsync();
                    totalItems = itemBatch.Results.Count;

                }
                catch (AmazonDynamoDBException dbException) { Console.WriteLine(dbException.Message); }
                catch (AmazonServiceException dbSvcException) { Console.WriteLine(dbSvcException.Message); }

                return totalItems;
            }

           
        }
    }
}

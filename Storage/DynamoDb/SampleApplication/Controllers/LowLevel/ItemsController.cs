using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using DynamoDBWebApiSample.Utilities;

namespace DynamoDBWebApiSample.Controllers.LowLevel
{
    [Route("api/lowlevel/[controller]")]
    public class ItemsController : Controller
    {
        private IAmazonDynamoDB _dynamoClient;
        public ItemsController(IAmazonDynamoDB dynamoClient)
        {
            _dynamoClient = dynamoClient;
        }
        [HttpGet("{tableName}")]
        public async Task<IActionResult> Get(string tableName)
        {
            try
            {
                Dictionary<string, AttributeValue> queryInfo = new Dictionary<string, AttributeValue>();
                var queryString = HttpContext.Request.Query;
                foreach(var queryVariable in queryString)
                {
                    queryInfo.Add(queryVariable.Key, new AttributeValue(queryVariable.Value));
                }
                GetItemRequest request = new GetItemRequest(tableName, queryInfo);   
                
                var res = await _dynamoClient.GetItemAsync(request);
                return Ok(res.Item);
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] PutItemRequest request)
        {
            try
            {
                var res = await _dynamoClient.PutItemAsync(request);
                return new JsonResult(res) { StatusCode = 201 };
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);

        }
        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateItemRequest request)
        {
            try
            {
                var res = await _dynamoClient.UpdateItemAsync(request);
                return new JsonResult(res) { StatusCode = 200 };
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteItem([FromBody] DeleteItemRequest request)
        {
            try
            {
                var res = await _dynamoClient.DeleteItemAsync(request);
                return StatusCode(204);
            }
            catch (AmazonDynamoDBException addbe)
            {
                return AmazonExceptionHandlers.HandleAmazonDynamoDBException(addbe);
            }
            catch (AmazonServiceException ase)
            {
                AmazonExceptionHandlers.HandleAmazonServiceExceptionException(ase);
            }
            catch (AmazonClientException ace)
            {
                AmazonExceptionHandlers.HandleAmazonClientExceptionException(ace);
            }
            return StatusCode(500);
        }
    }
}
using System;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace DynamoDBWebApiSample.Utilities
{
    public static class AmazonExceptionHandlers
    {
        public static IActionResult HandleAmazonDynamoDBException(AmazonDynamoDBException addbe)
        {
            Console.WriteLine("AmazonDynamoDBException:");
            Console.WriteLine(addbe.Message);
            Console.WriteLine(addbe.Source);
            Console.WriteLine(addbe.StatusCode);
            Console.WriteLine(addbe.ErrorCode);
            var message = new { message = addbe.Message };
            if (addbe.ErrorCode == "ResourceNotFoundException") return new JsonResult(message) { StatusCode = 404 };
            return new JsonResult(message) { StatusCode = (int)addbe.StatusCode };
        }
        public static void HandleAmazonServiceExceptionException(AmazonServiceException ase)
        {
            Console.WriteLine("AmazonServiceException:");
            Console.WriteLine(ase.Message);
            Console.WriteLine(ase.StatusCode);
        }

        public static void HandleAmazonClientExceptionException(AmazonClientException ace)
        {
            Console.WriteLine("AmazonClientException:");
            Console.WriteLine(ace.Message);
        }
    }
}
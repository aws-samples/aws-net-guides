using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using DynamoDBWebApiSample.Utilities;

namespace DynamoDBWebApiSample.Controllers.Document
{
    [Route("api/document/[controller]")]
    public class ProductsController : Controller
    {
        private DynamoDBTableBuilder _dbTableBuilder;
        public ProductsController(DynamoDBTableBuilder dbTableBuilder)
        {
            _dbTableBuilder = dbTableBuilder;
        }

        [HttpGet("{productId}/{publishedOn}")]
        public async Task<IActionResult> Get(string productId, string publishedOn)
        {
            var table = await _dbTableBuilder.Build();
            try
            {
                var product = await table.GetItemAsync(hashKey: productId, rangeKey: publishedOn);
                if (product == null) return NotFound();
                Console.WriteLine(product["DocumentProductId"]);
                Console.WriteLine(product["PublishOn"]);
                return Ok(product.ToJson());
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
        public async Task<IActionResult> Create([FromBody] Amazon.DynamoDBv2.DocumentModel.Document product)
        {
            var table = await _dbTableBuilder.Build();
            try
            {
                Console.WriteLine(product["DocumentProductId"]);
                Console.WriteLine(product["PublishOn"]);

                await table.PutItemAsync(product);
                var productId = product["DocumentProductId"].AsString();
                return new JsonResult(
                    new
                    {
                        message = $"Created new product: {productId}",
                        product = product.ToJson()
                    })
                { StatusCode = 201 };
            }
            catch (AmazonDynamoDBException addbe)
            {
                if (addbe.ErrorCode == "ConditionalCheckFailedException") return new JsonResult(
                     new
                     {
                         message = $"Product {product["DocumentProductId"]} already exists"
                     })
                { StatusCode = 409 };
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
        [HttpPut("{productId}/{publishedOn}")]
        public async Task<IActionResult> Update([FromBody] Amazon.DynamoDBv2.DocumentModel.Document product, string productId, string publishedOn)
        {
            var table = await _dbTableBuilder.Build();
            try
            {
                var checkForProduct = await table.GetItemAsync(productId, publishedOn);
                if (checkForProduct == null) return NotFound();
                product["DocumentProductId"] = productId;
                product["PublishOn"] = publishedOn;
                await table.UpdateItemAsync(product);
                var updatedProduct = await table.GetItemAsync(productId, publishedOn);
                return Ok(updatedProduct.ToJson());
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

        [HttpDelete("{productId}/{publishedOn}")]
        public async Task<IActionResult> Delete(string productId, string publishedOn)
        {
            var table = await _dbTableBuilder.Build();
            try
            {
                var checkForProduct = await table.GetItemAsync(productId, publishedOn);
                if (checkForProduct == null) return NotFound();
                await table.DeleteItemAsync(productId, publishedOn);
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
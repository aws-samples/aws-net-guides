using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using DynamoDBWebApiSample.ObjectPersistenceModels;
using DynamoDBWebApiSample.Utilities;

namespace DynamoDBWebApiSample.Controllers.ObjectPersistence
{
    [Route("api/opm/[controller]")]
    public class ProductsController : Controller
    {
        private DynamoDBContextBuilder _dbContextBuilder;
        public ProductsController(DynamoDBContextBuilder dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }

        [HttpGet("{productId}/{publishedOn}")]
        public async Task<IActionResult> Get(string productId, string publishedOn)
        {
            var db = await _dbContextBuilder.Build();
            try
            {
                var product = await db.LoadAsync<Product>(hashKey: productId, rangeKey: publishedOn);
                if (product == null) return NotFound();
                Console.WriteLine(product.ObjectPersistenceProductId);
                Console.WriteLine(product.PublishOn);
                return Ok(product);
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
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            var db = await _dbContextBuilder.Build();
            try
            {
                Console.WriteLine(product.ObjectPersistenceProductId);
                Console.WriteLine(product.PublishOn);
                await db.SaveAsync<Product>(product);
                return new JsonResult(
                    new
                    {
                        message = $"Created new product: {product.ObjectPersistenceProductId}",
                        product = product
                    })
                { StatusCode = 201 };
            }
            catch (AmazonDynamoDBException addbe)
            {
                if (addbe.ErrorCode == "ConditionalCheckFailedException") return new JsonResult(
                     new
                     {
                         message = $"Product {product.ObjectPersistenceProductId} already exists"
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
        public async Task<IActionResult> Update([FromBody] Product product, string productId, string publishedOn)
        {
            var db = await _dbContextBuilder.Build();
            try
            {
                var checkForProduct = await db.LoadAsync<Product>(productId, publishedOn);
                if (checkForProduct == null) return NotFound();
                product.ObjectPersistenceProductId = productId;
                product.PublishOn = publishedOn;
                await db.SaveAsync<Product>(product,
                    new DynamoDBOperationConfig { SkipVersionCheck = true, IgnoreNullValues = true });
                var updatedProduct = await db.LoadAsync<Product>(productId, publishedOn);
                return Ok(updatedProduct);
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
            var db = await _dbContextBuilder.Build();
            try
            {
                var checkForProduct = await db.LoadAsync<Product>(productId, publishedOn);
                if (checkForProduct == null) return NotFound();
                await db.DeleteAsync<Product>(productId, publishedOn,
                    new DynamoDBOperationConfig { SkipVersionCheck = true });
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
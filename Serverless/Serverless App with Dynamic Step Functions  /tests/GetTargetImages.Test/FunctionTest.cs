using Amazon.Lambda.TestUtilities;
using Xunit;
using Moq;
using Amazon.S3;
using DomainModel.EventBridgeEvent;
using System.Threading.Tasks;
using Amazon.S3.Model;
using System.Threading;
using System;

namespace GetTargetImages.Tests
{
    public class FunctionTest
    {

        [Fact]
        public async Task TestEmptyEventShouldReturnFalse()
        {
            var context = new TestLambdaContext();
            var function = new Function();
            var evt = new S3ObjectCreateEvent();

            var response = await function.FunctionHandler(evt, context);

            Assert.True(response.HasFaceComparisonRequestItems == false);
        }

        [Fact]
        public async Task TestEmptyTargetBucketShouldReturnFalse()
        {
            var context = new TestLambdaContext();
            var listObjects = new ListObjectsResponse();

            var mock = new Mock<IAmazonS3>();
            mock.Setup(c => c.ListObjectsAsync(It.IsAny<ListObjectsRequest>(), default(CancellationToken)))
              .ReturnsAsync(new ListObjectsResponse());

            var function = new Function(mock.Object);
            var evt = new S3ObjectCreateEvent();
            evt.Detail = new S3ObjectCreate();
            evt.Detail.Bucket = new Bucket();
            evt.Detail.Bucket.Name = "TestBucket";
            evt.Detail.Object = new DomainModel.EventBridgeEvent.S3Object();
            evt.Detail.Object.Key = "TestKey";

            var response = await function.FunctionHandler(evt, context);

            Assert.True(response.HasFaceComparisonRequestItems == false);
        }

        [Fact]
        public async Task TestTargetBucketWithObjectShouldReturnTrue()
        {
            var context = new TestLambdaContext();
            var listObjects = new ListObjectsResponse();
            var mock = new Mock<IAmazonS3>();

            var objectResponse = new ListObjectsResponse();
            var s3Object = new Amazon.S3.Model.S3Object();
            s3Object.Key = "TestKey";
            s3Object.Key = "TestBucket";

            objectResponse.S3Objects = new System.Collections.Generic.List<Amazon.S3.Model.S3Object>();
            objectResponse.S3Objects.Add(s3Object);

            mock.Setup(c => c.ListObjectsAsync(It.IsAny<ListObjectsRequest>(), default(CancellationToken)))
              .ReturnsAsync(objectResponse);

            var function = new Function(mock.Object);
            var evt = new S3ObjectCreateEvent();
            evt.Detail = new S3ObjectCreate();
            evt.Detail.Bucket = new Bucket();
            evt.Detail.Bucket.Name = "TestBucket";
            evt.Detail.Object = new DomainModel.EventBridgeEvent.S3Object();
            evt.Detail.Object.Key = "TestKey";

            var response = await function.FunctionHandler(evt, context);

            Assert.True(response.HasFaceComparisonRequestItems == true);
        }


        [Fact]
        public async Task TestExceptionShouldReturnFalse()
        {
            var context = new TestLambdaContext();
            var listObjects = new ListObjectsResponse();
            var mock = new Mock<IAmazonS3>();

            mock.Setup(c => c.ListObjectsAsync(It.IsAny<ListObjectsRequest>(), default(CancellationToken)))
              .ThrowsAsync(new Exception());

            var function = new Function(mock.Object);
            var evt = new S3ObjectCreateEvent();
            evt.Detail = new S3ObjectCreate();
            evt.Detail.Bucket = new Bucket();
            evt.Detail.Bucket.Name = "TestBucket";
            evt.Detail.Object = new DomainModel.EventBridgeEvent.S3Object();
            evt.Detail.Object.Key = "TestKey";

            var response = await function.FunctionHandler(evt, context);

            Assert.True(response.HasFaceComparisonRequestItems == false);
        }
    }
}
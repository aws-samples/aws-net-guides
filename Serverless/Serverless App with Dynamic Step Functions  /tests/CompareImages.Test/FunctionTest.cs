using System.Threading;
using Amazon.Lambda.TestUtilities;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Bogus;
using DomainModel;
using Moq;
using Xunit;

namespace CompareImages.Tests
{
    public class FunctionTest
    {

        [Fact]
        public async void TestEmptyEventShouldReturnFalse()
        {
            var context = new TestLambdaContext();
            var mock = new Mock<IAmazonRekognition>();
            mock.Setup(c => c.CompareFacesAsync(It.IsAny<CompareFacesRequest>(), default(CancellationToken)))
              .ReturnsAsync(new CompareFacesResponse());

            var function = new Function(mock.Object);
            var request = new FaceComparisonRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var response = await function.FunctionHandler(request, context);
            Assert.True(response.HasResults == false);
        }

        [Fact]
        public async void TestCompareFacesWithExceptionShouldThrowException()
        {
            var context = new TestLambdaContext();
            var mock = new Mock<IAmazonRekognition>();
            mock.Setup(c => c.CompareFacesAsync(It.IsAny<CompareFacesRequest>(), default(CancellationToken)))
              .Throws(new InvalidS3ObjectException("Unable to get object metadata from S3. Check object key, region and/or access permissions."));

            var function = new Function(mock.Object);
            var request = new FaceComparisonRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            await Assert.ThrowsAsync<InvalidS3ObjectException>(async () => await function.FunctionHandler(request, context));
        }

        [Fact]
        public async void TestCompareFacesShouldReturnComparison()
        {
            var context = new TestLambdaContext();
            var mock = new Mock<IAmazonRekognition>();
            var comparisonResult = new CompareFacesResponse();
            var boundingBox = new Faker<BoundingBox>()
                .RuleFor(o => o.Left, f => f.Random.Float(0, 1))
                .RuleFor(o => o.Top, f => f.Random.Float(0, 1));

            var compareFace = new Faker<ComparedFace>()
                .RuleFor(o => o.BoundingBox, () => boundingBox.Generate());

            var userFaker = new Faker<CompareFacesMatch>()
                .RuleFor(o => o.Face, () => compareFace.Generate())
                .RuleFor(o => o.Similarity, f => f.Random.Float(0, 1));

            var faceMatchs = userFaker.Generate(1000);

            comparisonResult.FaceMatches = faceMatchs;

            mock.Setup(c => c.CompareFacesAsync(It.IsAny<CompareFacesRequest>(), default(CancellationToken)))
               .ReturnsAsync(comparisonResult);

            var function = new Function(mock.Object);
            var request = new FaceComparisonRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var result = await function.FunctionHandler(request, context);

            Assert.True(result.Results.Count == 1000);
        }
    }
}
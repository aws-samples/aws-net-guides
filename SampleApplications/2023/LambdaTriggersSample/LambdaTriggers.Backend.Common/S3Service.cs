using System.Net;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using LambdaTriggers.Common;

namespace LambdaTriggers.Backend.Common;

public static class S3Service
{
	public const string BucketName = "lambdatriggersbucket";

	public static async Task<Uri> UploadContentToS3<T>(IAmazonS3 s3Client, string bucket, string key, T content, ILambdaLogger logger)
	{
		var request = content switch
		{
			Stream stream => new PutObjectRequest
			{
				InputStream = stream,
				BucketName = bucket,
				Key = key
			},
			_ => new PutObjectRequest
			{
				ContentType = "application/json",
				ContentBody = JsonSerializer.Serialize(content),
				BucketName = bucket,
				Key = key
			}
		};

		logger.LogInformation($"Uploading object to S3...");

		var putObjectResponse = await s3Client.PutObjectAsync(request).ConfigureAwait(false);
		var fileUrl = s3Client.GeneratePreSignedURL(bucket, key, DateTime.UtcNow.AddYears(1), null);

		if (putObjectResponse.HttpStatusCode is not HttpStatusCode.OK)
			throw new HttpRequestException($"{nameof(IAmazonS3.PutObjectAsync)} Failed: {putObjectResponse.HttpStatusCode}");

		logger.LogInformation($"Upload suceeded");
		logger.LogInformation($"{nameof(putObjectResponse.ChecksumSHA256)}: {putObjectResponse.ChecksumSHA256}");

		return new Uri(fileUrl);
	}

	public static string GenerateThumbnailFilename(in string fileName) => Path.GetFileNameWithoutExtension(fileName) + Constants.ThumbnailSuffix;

	public static async Task<Uri?> GetFileUri(IAmazonS3 s3Client, string bucket, string key, ILambdaLogger lambdaLogger, DateTime? expirationDate = default)
	{
		expirationDate ??= DateTime.UtcNow.AddYears(1);

		lambdaLogger.LogInformation("Creating Presigned URL...");


		var doesFileExist = await DoesFileExist(bucket, key, s3Client, lambdaLogger).ConfigureAwait(false);
		if (!doesFileExist)
			return null;

		var url = s3Client.GeneratePreSignedURL(bucket, key, expirationDate.Value, null);

		lambdaLogger.LogInformation($"Presigned URL Expiring on {expirationDate:MMMM dd, yyyy} Generated: {url}");

		return new Uri(url);
	}

	static async Task<bool> DoesFileExist(string bucket, string key, IAmazonS3 s3Client, ILambdaLogger lambdaLogger)
	{
		try
		{
			var response = await s3Client.GetObjectAsync(bucket, key).ConfigureAwait(false);
			return response is not null;
		}
		catch (AmazonS3Exception e)
		{
			lambdaLogger.LogError(e.Message);
			return false;
		}
	}
}
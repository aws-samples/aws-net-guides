using Refit;

namespace LambdaTriggers.Mobile;

class PhotosApiService
{
	readonly IUploadPhotosAPI _uploadPhotosApiClient;
	readonly IGetThumbnailApi _getThumbnailApiClient;

	public PhotosApiService(IUploadPhotosAPI uploadPhotosApiClient, IGetThumbnailApi getThumbnailApiClient) =>
		(_uploadPhotosApiClient, _getThumbnailApiClient) = (uploadPhotosApiClient, getThumbnailApiClient);

	public async Task<Uri> UploadPhoto(string photoTitle, FileResult photoMediaFile, CancellationToken token)
	{
		var fileStream = await photoMediaFile.OpenReadAsync().ConfigureAwait(false);
		return await _uploadPhotosApiClient.UploadPhoto(photoTitle, new StreamPart(fileStream, $"{photoTitle}"), token).ConfigureAwait(false);
	}

	public Task<Uri> GetThumbnailUri(string photoTitle, CancellationToken token) => _getThumbnailApiClient.GetThumbnailUri(photoTitle, token);
}
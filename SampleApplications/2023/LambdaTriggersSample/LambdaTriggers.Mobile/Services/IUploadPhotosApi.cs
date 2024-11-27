using LambdaTriggers.Common;
using Refit;

namespace LambdaTriggers.Mobile;

[Headers("Accept-Encoding: gzip", "Accept: application/json")]
public interface IUploadPhotosAPI
{
	[Post($"/LambdaTriggers_UploadImage?{Constants.ImageFileNameQueryParameter}={{photoTitle}}"), Multipart]
	Task<Uri> UploadPhoto(string photoTitle, [AliasAs("photo")] StreamPart photoStream, CancellationToken token);
}
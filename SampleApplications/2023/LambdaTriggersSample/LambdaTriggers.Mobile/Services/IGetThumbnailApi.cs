using LambdaTriggers.Common;
using Refit;

namespace LambdaTriggers.Mobile;

[Headers("Accept-Encoding: gzip", "Accept: application/json")]
public interface IGetThumbnailApi
{
	[Get($"/LambdaTriggers_GetThumbnail?{Constants.ImageFileNameQueryParameter}={{photoTitle}}")]
	Task<Uri> GetThumbnailUri(string photoTitle, CancellationToken token);
}
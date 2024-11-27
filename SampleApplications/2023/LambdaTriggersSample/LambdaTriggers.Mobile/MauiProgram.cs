using System.Net;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using LambdaTriggers.Common;
using Polly;
using Refit;

namespace LambdaTriggers.Mobile;

public class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder()
								.UseMauiApp<App>()
								.UseMauiCommunityToolkit()
								.UseMauiCommunityToolkitMarkup();

		// App Shell
		builder.Services.AddTransient<AppShell>();

		// Services
		builder.Services.AddSingleton<App>();
		builder.Services.AddSingleton(MediaPicker.Default);
		builder.Services.AddSingleton<PhotosApiService>();
		builder.Services.AddRefitClient<IUploadPhotosAPI>()
							.ConfigureHttpClient(client => client.BaseAddress = new Uri(Constants.UploadPhotoApiUrl))
							.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, sleepDurationProvider));

		builder.Services.AddRefitClient<IGetThumbnailApi>()
							.ConfigureHttpClient(client => client.BaseAddress = new Uri(Constants.GetThumbnailApiUrl))
							.AddTransientHttpErrorPolicy(builder => builder.OrResult(response => response.StatusCode is HttpStatusCode.NotFound).WaitAndRetryAsync(3, sleepDurationProvider));

		// Pages + View Models
		builder.Services.AddTransient<PhotoPage, PhotoViewModel>();

		return builder.Build();

		static TimeSpan sleepDurationProvider(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
	}
}
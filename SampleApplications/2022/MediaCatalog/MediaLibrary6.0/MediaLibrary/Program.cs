using Amazon.XRay.Recorder.Handlers.AwsSdk;
using AWS.Logger.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MediaLibrary;
using MediaLibrary.Services;

AwsSettings _settings = new AwsSettings();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IStorageService, S3StorageService>();
builder.Services.AddScoped<IFileMetadataService, DynamoFileMetadataService>();
builder.Services.AddScoped<IImageMetadataService, DynamoImageMetadataService>();
builder.Services.AddScoped<IImageLookupService, DynamoImageLookupService>();



builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWSConfig"));

builder.Logging.AddAWSProvider();

AWSSDKHandler.RegisterXRayForAllServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseXRay("MediaCatalog");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

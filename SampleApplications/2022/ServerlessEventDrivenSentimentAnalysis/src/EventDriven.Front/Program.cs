using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using EventDriven.Front.Data;
using Amazon.SQS;
using Amazon.StepFunctions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient<RequestService>();
builder.Services.AddHttpClient<CustomerInteractionService>();

builder.Services.AddSingleton<AmazonSQSClient>(new AmazonSQSClient(Amazon.RegionEndpoint.USEast1));
builder.Services.AddSingleton<AmazonStepFunctionsClient>(new AmazonStepFunctionsClient(Amazon.RegionEndpoint.USEast1));
builder.Services.AddScoped<CustomerInteractionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

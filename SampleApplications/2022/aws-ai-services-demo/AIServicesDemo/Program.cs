using Amazon.Comprehend;
using Amazon.Rekognition;
using Amazon.Textract;
using Amazon.Translate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add AI Services
builder.Services.AddAWSService<IAmazonComprehend>();
builder.Services.AddAWSService<IAmazonTranslate>();
builder.Services.AddAWSService<IAmazonTextract>();
builder.Services.AddAWSService<IAmazonRekognition>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

using Amazon.DAX;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Pass the Credenbtial from code - not recommended
//var credentials = new BasicAWSCredentials("accessKey", "secretKey");


// Get Role Credentials
var credentials = FallbackCredentialsFactory.GetCredentials();

var config = new AmazonDynamoDBConfig()
{
    RegionEndpoint = Amazon.RegionEndpoint.USEast1
};
var client = new AmazonDynamoDBClient(credentials, config);

builder.Services.AddSingleton<IAmazonDynamoDB>(client);
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

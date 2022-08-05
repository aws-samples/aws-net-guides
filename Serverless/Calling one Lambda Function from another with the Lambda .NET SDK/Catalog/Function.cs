using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Catalog;

public class Function
{

    public async Task<string> FunctionHandler(string catalogItemId, ILambdaContext context)
    {
        context.Logger.LogInformation($"Searching the catalog for an item with id: {catalogItemId}");

        AmazonLambdaClient client = new AmazonLambdaClient();

        var request = new InvokeRequest
        {
            FunctionName = "Inventory",
            Payload = JsonSerializer.Serialize(catalogItemId) // note that we are serializing the catalogItemId to JSON, this is important.
        };
        var result = await client.InvokeAsync(request);
        var item = JsonSerializer.Deserialize<Item>(result.Payload);

        return $"Catalog function called the Inventory function which returned an item: {item.InventoryId}, {item.Name}, {item.Quantity}";
    }

}

public class Item
{
    public string InventoryId { get; init; }
    public string Name { get; init; }
    public int Quantity { get; init; }
} 

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Inventory;

public class Function
{
    
    public Item FunctionHandler(string inventoryId, ILambdaContext context)
    {
        // Perform a search for the inventory item
        context.Logger.LogInformation($"Searching for inventory item with id: {inventoryId}");
        var item = new Item(){ InventoryId = "15a", Name = "Red Running Shoes", Quantity = 12};
        return item;
    }
}

public class Item
{
    public string InventoryId { get; init; }
    public string Name { get; init; }
    public int Quantity { get; init; }
}
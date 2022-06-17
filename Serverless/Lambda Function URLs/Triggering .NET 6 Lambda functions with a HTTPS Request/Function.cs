using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FunctionUrlExample;

public class Function
{
    public string FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        
        var serializationOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling= JsonNumberHandling.AllowReadingFromString
        };
        Person person = JsonSerializer.Deserialize<Person>(request.Body, serializationOptions); 

        return $"Hello {person.FirstName} {person.LastName}, you are {person.Age} years old.";
    }
}

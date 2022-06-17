using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FunctionUrlExample;

public class Function
{
    public Person FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
       Person person = new Person { FirstName = "Jane", LastName = "Doe", Age = 42 };

       return person;
    }
}

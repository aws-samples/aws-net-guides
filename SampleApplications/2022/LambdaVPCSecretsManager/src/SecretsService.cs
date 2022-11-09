using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace AspNetCoreWebApiRds;

public static class SecretsService
{
    public static async Task<string> GetConnectionStringFromSecret(ApplicationOptions options)
    {
        IAmazonSecretsManager client = string.IsNullOrWhiteSpace(options.Secret.Region)  
            ? new AmazonSecretsManagerClient() 
            : new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(options.Secret.Region));

        GetSecretValueRequest request = new GetSecretValueRequest(){
            SecretId =  options.Secret.Name,
            VersionStage = options.Secret.VersionStage 
        };

        try
        {
            GetSecretValueResponse response = await client.GetSecretValueAsync(request);
            SqlServerSecretModel sqlServerSecretModel = JsonSerializer.Deserialize<SqlServerSecretModel>(response.SecretString);
            Console.WriteLine(sqlServerSecretModel.Host);
            return sqlServerSecretModel.GetConnectionString(options.Database.Name, options.Database.MultipleActiveResultSets);
        }
        catch (Exception ex )
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
            throw;
        }
    }
}
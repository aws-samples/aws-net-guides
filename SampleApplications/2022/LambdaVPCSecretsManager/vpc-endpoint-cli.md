# Using a VPC Endpoint - Command Line

This approach uses a [VPC endpoint](https://docs.aws.amazon.com/vpc/latest/privatelink/what-is-privatelink.html) to give your VPC connected Lambda function access to the Secrets Manager service only. It does not give your Lambda function access to the internet.

The traffic between the Lambda function and the Secrets Manager service will travel over the AWS network.

## Some Background and Concepts

### VPC Endpoints

A VPC endpoint is a logical connection between your VPC and another AWS service. It is a private connection between your VPC and the service, and does not traverse the internet. In the case of a Secrets Manager VPC endpoint, the traffic between your Lambda function and the Secrets Manager service will travel over the AWS network.

For each subnet you create a VPC endpoint for you are charged a fee. You need to find a balance between resilience and availability of the VPC endpoint and the price you pay for it.

For this tutorial, you should create a VPC endpoint for the subnet(s) that your Lambda function will be connected to, otherwise you will see errors when you try to access Secrets Manager from your Lambda function.

## 1 - Create the AWS RDS SQL Server database

The first thing you will do is create the database. Each AWS region has different SQL Server versions available. To see what is available in your region, run the following command:

`aws rds describe-db-engine-versions --engine sqlserver-ex --query 'DBEngineVersions[*].EngineVersion'`

You will see a list that looks like this:
```
[
    "12.00.5571.0.v1",
    "12.00.6293.0.v1",
    "12.00.6329.1.v1",
    "12.00.6433.1.v1",
    "13.00.5598.27.v1",
    "13.00.5820.21.v1",
    "13.00.5850.14.v1",
    "13.00.5882.1.v1",
    "13.00.6300.2.v1",
    "14.00.3281.6.v1",
    "14.00.3294.2.v1",
    "14.00.3356.20.v1",
    "14.00.3381.3.v1",
    "14.00.3401.7.v1",
    "14.00.3421.10.v1",
    "15.00.4043.16.v1",
    "15.00.4073.23.v1",
    "15.00.4153.1.v1",
    "15.00.4198.2.v1"
]
```
Remember, depending on the region you are in, the list will be different.

Choose the highest version available in your region, and use that in the following command:

`aws rds create-db-instance --db-instance-identifier demo-sql-server --db-instance-class db.t3.small --engine sqlserver-ex --master-username admin --master-user-password SOME_COMPLEX_PASSWORD123! --allocated-storage 20 --license-model license-included --engine-version VERSION --no-publicly-accessible` 

Replace the `VERSION` with the version you chose from the list above.

This will take some time to run complete. 

Once the server is running, you can't access from your home computer, because you made the accessible from with the VPC only. Note the `--no-publicly-accessible` flag.

To get the endpoint, run the following command, but remember, it takes a while for the server to start up:
`aws rds describe-db-instances --db-instance-identifier demo-sql-server --query 'DBInstances[0].Endpoint.[Address,Port]' --output text`

If you get "None" back, it means the server has not started yet. Wait a few minutes and try again.

Once the server is running, the command will return the endpoint and port, it will look like:

`demo-sql-server.yyyyyyyyy.us-east-1.rds.amazonaws.com  1443`

Because you didn't specify a security group, the database uses the default security group, and because you didn't specify any subnets, the database will be available in all subnets in the VPC. This will be important later, when connecting the Lambda function to the VPC.

Now you have a SQL Server database running. Next you will add the credentials to Secrets Manager.

## 2 - Add the credentials to Secrets Manager

Open the file `Credentials\DatabaseCredentials.json`. Replace the `host` value with the endpoint address you got from the previous step.

From the command line run:
`aws secretsmanager create-secret --name demo-sql-server --secret-string file://Credentials/DatabaseCredentials.json`

You will see output like:
```
{
    "ARN": "arn:aws:secretsmanager:us-east-1:000000000000:secret:demo-sql-server-UFE2ki",
    "Name": "demo-sql-server",
    "VersionId": "4a60c581-e613-4e32-8907-ecdf2c26b653"
}
```

Note the ARN, you will need this later. 

Now you have a database and the credentials to access it. Next you will create the Lambda function that tries to access both Secrets Manager and the database.

## 3 - Create an S3 bucket

Before you can deploy the Lambda function, you need to create an S3 bucket to store the CloudFormation stack that will be created. Run:

`aws s3api create-bucket --bucket cloudformation-templates-2022`

If you are using a region other than us-east-1 region, add `--create-bucket-configuration LocationConstraint=REGION` to the end of the above command.

*Note, you must use a [unique name](https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html) for the bucket, you can’t use the one shown here.*

## 4 - Deploy the Lambda function

At the end of this step you will have a Lambda function that partially works, it will be able to access Secrets Manager, but not the database. 

This is because the Lambda function will not be connected to the VPC yet, that is for later step. Then, when the Lambda function is connected to the VPC, it will be able to access the database, but not Secrets Manager!

I'm not going to guide you through each step in creating the Lambda function code, because it is available as part of this repository. The project was created using the [AWS Lambda Templates](https://github.com/aws/aws-lambda-dotnet/#dotnet-cli-templates), specifically using the `serverless.AspNetCoreWebAPI` template.

Open the accompanying directory, `src`, in your favorite IDE.

Open the `Startup.cs` file. The `ConfigureServices(..)` does three things 
1. loads configuration settings from the `appsettings.json` file
2. creates the database connection string by getting the credentials from Secrets Manager
3. creates an Entity Framework database context

The `Configure(..)` method configures makes sure that the database is created, and seeded. 

Open the `SecretsService.cs` file, it uses the configuration settings to load the appropriate secret from Secrets Manager, and returns a connection string for the database.

Open the `Data` directory, it contains the invoice model, the database context, and the database seeder.  

Open the `Controllers` directory, it contains an `InvoicesController`, its `Get(..)` method returns a list of invoices.

Open the file `appsettings.json`, the `Region` blank, if you need to deploy your Lambda function to a different region than where your secret is stored, set the region of the secret here.

You don't need to make any changes to the code. 

To deploy the function, run the following from the `src` directory:

`dotnet lambda deploy-serverless --stack-name AspNetCoreWebApiRds --s3-bucket cloudformation-templates-2022`

This will take a few minutes to complete the deployment, and will return output that ends like this:
```
//snip..
9/7/2022 3:56 PM     ServerlessRestApiProdStage               CREATE_COMPLETE
9/7/2022 3:56 PM     AspNetCoreFunctionRootResourcePermissionProd CREATE_COMPLETE
9/7/2022 3:56 PM     AspNetCoreFunctionProxyResourcePermissionProd CREATE_COMPLETE
9/7/2022 3:56 PM     AspNetCoreWebApiRdsTestForArticle        CREATE_COMPLETE
Stack finished updating with status: CREATE_COMPLETE

Output Name                    Value
------------------------------ --------------------------------------------------
ApiURL                         https://xxxxxxxx.execute-api.us-east-1.amazonaws.com/Prod/
```

That last line is the URL of your API.

If you try to access it now, you will get an internal server error, because the Lambda function attempts to access the Secret Manager secret, but does NOT have permission to do so. 

## 5 - Grant the Lambda function role access to the secret in Secrets Manager

Before the function can access the secret it needs to be granted permission to do so. You do this by adding a policy to the role that the Lambda function uses.

But, the function name is not the same as the stack name. To find the function name run the following command:

```
aws lambda list-functions --query 'Functions[?starts_with(FunctionName, `AspNetCoreWebApiRds`) == `true`].FunctionName' --output text
```

You will see something like:

AspNetCoreWebApiRds-AspNetCoreFunction-aaaaa

To find the name of the role the function uses, run the following command:

`aws lambda get-function --function-name AspNetCoreWebApiRds-AspNetCoreFunction-aaaaa --query 'Configuration.Role' --output text`

You will see something like:
`arn:aws:iam::000000000000:role/AspNetCoreWebApiRds-AspNetCoreFunctionRole-bbbbb`

Now you are going to give that role access to the secret you created earlier. 

Open the file `Policies\SecretesManagerAccess.json`, and replace the `Resource` with the ARN of the secret you created earlier.

Then run the following command:

`aws iam put-role-policy --role-name AspNetCoreWebApiRds-AspNetCoreFunctionRole-bbbbb --policy-name SecretsManagerAccess --policy-document file://policies/SecretsManagerAccess.json`

This command adds a policy to the role that allows the Lambda function to access the secret in Secrets Manager.

At this point, your function can access the secret in Secrets Manager, but not the database. If you invoked the Lambda now, it would fail when it tries to connect to the database, but you would see a message in the log with the database name on port.

<figure>
  <img
  src="media/1.drawio.svg"
  alt="">
  <figcaption>Lambda function has access to Secrets Manager, but not the database</figcaption>
</figure>

## 6 - Connect the Lambda function the VPC

Now you are going to connect the Lambda function to the VPC. This will allow the Lambda function to access the database, but you will lose access to Secrets Manager!

For simplicity, you are going to connect the Lambda function to a single subnet in the VPC. If you want more resiliency, you can connect the Lambda function to multiple subnets in different availability zones. If you do connect the Lambda function to multiple subnets, you should to make the VPC endpoint available on those subnets too (keep in mind that you pay more for VPC endpoints connected to multiple subnets).

Before you connect the Lambda function the VPC, you need to find out the ID of your default security group, and choose a subnet to use.

### 6.1 - Get the VPC ID and Default Security Group ID

The following command will get the VPC id and its default security group id :
```aws ec2 describe-security-groups --query 'SecurityGroups[?GroupName==`default`].[VpcId, GroupId]' --output text```

You will see output like:

`vpc-11111   sg-22222`

### 6.2 - Get the list of subnets in your VPC

You are going to attach your Lambda function to one subnet.

But first, get the list of all subnets in your VPC:

```aws ec2 describe-subnets --query 'Subnets[?VpcId==`vpc-11111`].SubnetId' --output text```

The output will look like:
`subnet-33333        subnet-44444        subnet-yyyyy        subnet-yyyyy        subnet-yyyyy        subnet-yyyyy`

Depending on your region you may have fewer subnets. 

Select one of these subnets for use with your Lambda function.

I will connect the Lambda function to `subnet-33333`.  

### 6.4 - Update the Lambda function, connecting it to the VPC

Run the following command:

`aws lambda update-function-configuration --function-name AspNetCoreWebApiRds-AspNetCoreFunction-aaaaa --vpc-config SubnetIds=subnet-33333,SecurityGroupIds=sg-22222`

You will see output showing the new function configuration, but it will take a few minutes to complete. 

Once complete, the Lambda function will have access to the database, but not Secrets Manager! If you attempt to invoke the function, it will fail again!

<figure>
  <img
  src="media/vpc-endpoint/2.drawio.svg"
  alt="">
  <figcaption>Lambda function has access to the database, but not Secrets Manager</figcaption>
</figure>

#### Permissions to connect the Lambda function to the VPC

If you are wondering how the Lambda function was able to connect to the VPC, it's because the Lambda function role has a policy that allows it to do so.
Open the `serverless.template` file in the root of the project, there is a section that looks like this:
```
"Policies": [
    "AWSLambda_FullAccess",
    "AWSLambdaVPCAccessExecutionRole"
]
```
These are the polices that are attached to the role that the Lambda function uses. The policy `AWSLambdaVPCAccessExecutionRole` allows the role to create the required network interfaces to connect to the VPC.

## 7 - Create a VPC endpoint for Secrets Manager

The AWS SDK accesses AWS services using well known endpoint addresses. To see a list of these run:
`aws ec2 describe-vpc-endpoint-services --query 'ServiceDetails[].ServiceName'`

But we are looking for the Secrets Manager service, so run:
```
aws ec2 describe-vpc-endpoint-services --query 'ServiceDetails[?contains(ServiceName, `secretsmanager`)].ServiceName' --output text
```

You will use the output of this command in the next step.

The following command will create a VPC endpoint for Secrets Manager on the same subnet and security group you used when connecting the Lambda function to the VPC. 

`aws ec2 create-vpc-endpoint --vpc-endpoint-type Interface --private-dns-enabled --vpc-id vpc-11111 --service-name com.amazonaws.us-east-1.secretsmanager --subnet-ids subnet-33333 --security-group-ids sg-22222 --tag-specifications 'ResourceType=vpc-endpoint,Tags=[{Key=Name,Value=My-Secrets-Manager-Endpoint}]'`

The output of this command will include the VPCEndpointId, it will look like:
```
{
    "VpcEndpoint": {
        "VpcEndpointId": "vpce-55555"
```

This will take a few minutes to complete.

Note the `VpcEndpointId`, you will use it when you delete the resources at the end of this tutorial.

<figure>
  <img
  src="media/vpc-endpoint/3.drawio.svg"
  alt="">
  <figcaption>Lambda function has access to Secrets Manager, and the database</figcaption>
</figure>

## 9 - Invoke the Lambda function

Back in your browser, go to the URL from step 4, https://xxxxxxxx.execute-api.us-east-1.amazonaws.com/Prod/Invoices.

You should see a list of invoices displayed in the browser.

There you go, a Lambda function that accesses a non public database, and Secrets Manager via a VPC endpoint.

If you want to create more Lambda functions that do the same, the process is much easier now that the VPC endpoint in place. All you need to do is connect the Lambda function to the VPC, using the same subnet and security group as you did for this function.

## Clean up

It's always a good idea to delete resources after you are finished with them.

#### Delete the database

`aws rds delete-db-instance --db-instance-identifier demo-sql-server --final-db-snapshot-identifier SkipFinalSnapshot`

#### Delete the secret

`aws secretsmanager delete-secret --secret-id demo-sql-server`

#### Delete the Lambda function

`dotnet lambda delete-serverless --stack-name AspNetCoreWebApiRds`

#### Delete the VPC endpoint

`aws ec2 delete-vpc-endpoints --vpc-endpoint-ids vpce-55555`

If **successful**, the output of this last command will be:
```
{
    "Unsuccessful": []
} 
```

# .NET Core Web API Application using Amazon DynamoDB

## Step-by-Step Walkthrough
--------

This walk-through comprises a Microsoft Word doc, along with a .NET Core Web API application. The application provides an API that demonstrates interacting with Amazon DynamoDB through the low-level, document, and object persistence programming models.

Start the project with Visual Studio or with the `dotnet` CLI tool's `dotnet run` command. Use the `curl` or `PowerShell` scripts below or a tool like Postman to interact with the API.

+ Links to documentation
  * Amazon DynamoDB Page: [https://aws.amazon.com/dynamodb/](https://aws.amazon.com/dynamodb/)
  * Amazon DynamoDB Developer Guide: [https://aws.amazon.com/dynamodb/developer-resources/](https://aws.amazon.com/dynamodb/developer-resources/)

+ Prerequisites
  * AWS CLI
	* .NET Core 2.x SDK or higher installed
	* AWS Account with credentials configured locally in Visual Studio or using the CLI
        * Requires an AWS profile named `local-test-profile`. See [Configuring the AWS SDK for .NET with .NET Core](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-netcore.html) for more information
	* Optional: Visual Studio 2017 or Visual Studio Mac

+ External libraries:
  * NuGet Package AWSSDK.DynamoDBv2
  * NuGet Package AWSSDK.Extensions.NETCore.Setup

## `PowerShell` Scripts for Testing Amazon DynamoDB through Web API
### Low-level API
#### List All Tables
`Invoke-RestMethod http://localhost:5000/api/lowlevel/tables`
#### Get a Table
`Invoke-RestMethod http://localhost:5000/api/lowlevel/tables/Products`
#### Get a Table Status
`Invoke-RestMethod http://localhost:5000/api/lowlevel/tables/Products/status`
#### Create a Table
```ps1
$URI = "http://localhost:5000/api/lowlevel/tables"
$BODY = Get-Content -Raw -Path data\create_table.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Update a Table
```ps1
$URI = "http://localhost:5000/api/lowlevel/tables/Products"
$BODY = Get-Content -Raw -Path data\update_table.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Delete a Table 
`Invoke-RestMethod -Method 'Delete' http://localhost:5000/api/lowlevel/tables/Products`
#### Get an Item
```ps1
$URI = "http://localhost:5000/api/lowlevel/items"
$BODY = Get-Content -Raw -Path data\get_item.json
Invoke-RestMethod -Method 'Get' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Create an Item
```ps1
$URI = "http://localhost:5000/api/lowlevel/items"
$BODY = Get-Content -Raw -Path data\create_item.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Update an Item
```ps1
$URI = "http://localhost:5000/api/lowlevel/items"
$BODY = Get-Content -Raw -Path data\update_item.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Delete an Item
```ps1
$URI = "http://localhost:5000/api/lowlevel/items"
$BODY = Get-Content -Raw -Path data\delete_item.json
Invoke-RestMethod -Method 'Delete' -Uri $URI -ContentType 'application/json' -Body $BODY
```
### Document Model API
#### Get a Product
`Invoke-RestMethod http://localhost:5000/api/document/products/1234/2018-01-01`
#### Create a Product
```ps1
$URI = "http://localhost:5000/api/document/products"
$BODY = Get-Content -Raw -Path data\create_item_document.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Update a Product
```ps1
$URI = "http://localhost:5000/api/document/products/1234/2018-01-01"
$BODY = Get-Content -Raw -Path data\update_item_document.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Delete a Product
`Invoke-RestMethod -Method 'Delete' http://localhost:5000/api/document/products/1234/2018-01-01`
### Object Persistence Model API
#### Get a Product
`Invoke-RestMethod http://localhost:5000/api/opm/products/123/08-01-2018`
#### Create a Product
```ps1
$URI = "http://localhost:5000/api/opm/products"
$BODY = Get-Content -Raw -Path data\create_product.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Update a Product
```ps1
$URI = "http://localhost:5000/api/opm/products/1234/2018-01-01"
$BODY = Get-Content -Raw -Path data\update_product.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```
#### Delete a Product
`Invoke-RestMethod -Method 'Delete' http://localhost:5000/api/opm/products/1234/2018-01-01`

## `curl` Scripts for Testing Amazon DynamoDB through Web API
### Low-level API
#### List All Tables
`curl http://localhost:5000/api/lowlevel/tables`
#### Get a Table
`curl http://localhost:5000/api/lowlevel/tables/Products`
#### Get a Table Status
`curl http://localhost:5000/api/lowlevel/tables/Products/status`
#### Create a Table
`curl -d @data/table.json -H "Content-Type: application/json" http://localhost:5000/api/lowlevel/tables`
#### Update a Table
`curl -d @data/update_table.json -H "Content-Type: application/json" -X PUT http://localhost:5000/api/lowlevel/tables/Products`
#### Delete a Table 
`curl -X DELETE http://localhost:5000/api/lowlevel/tables/Products`
#### Get an Item
`curl -d @data/get_item.json -H "Content-Type:application/json" -X GET http://localhost:5000/api/lowlevel/items`
#### Create an Item
`curl -d @data/get_item.json -H "Content-Type:application/json" -X POST http://localhost:5000/api/lowlevel/items`
#### Update an Item
`curl -d @data/update_item.json -H "Content-Type:application/json" -X PUT http://localhost:5000/api/lowlevel/items`
#### Delete an Item
`curl -d @data/delete_item.json -H "Content-Type:application/json" -X DELETE http://localhost:5000/api/lowlevel/items`
### Document Model API
#### Get a Product
`curl http://localhost:5000/api/document/products/1234/2018-01-01`
#### Create a Product
`curl -d @data/create_item_document.json -H "Content-Type: application/json" -X POST http://localhost:5000/api/document/products`
#### Update a Product
`curl -d @data/update_item_document.json -H "Content-Type: application/json" -X PUT http://localhost:5000/api/document/products/1234/2018-01-01`
#### Delete a Product
`curl -X DELETE http://localhost:5000/api/document/products/1234/2018-01-01`
### Object Persistence Model API
#### Get a Product
`curl http://localhost:5000/api/opm/products/123/08-01-2018`
#### Create a Product
`curl -d @data/create_product.json -H "Content-Type: application/json" -X POST http://localhost:5000/api/opm/products`
#### Update a Product
`curl -d @data/update_product.json -H "Content-Type: application/json" -X PUT http://localhost:5000/api/opm/products/1234/2018-01-01`
#### Delete a Product
`curl -X DELETE http://localhost:5000/api/opm/products/1234/2018-01-01`

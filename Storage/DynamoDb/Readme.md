# Accessing Amazon DynamoDB from a .NET Core Web API Application

## Overview

Amazon DynamoDB is a fast and flexible NoSQL database service for all applications that need consistent, single-digit millisecond latency at any scale. It is a fully managed cloud database and supports both document and key-value store models. Its flexible data model, reliable performance, and automatic scaling of throughput capacity make it a great fit for mobile, web, gaming, ad tech, IoT, and many other applications

This walk-through will show you to how access DynamoDB from a .NET Core Web API application. The application will use NuGet packages belonging to the [AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/) to show how easy it is to make calls to the service. Each API controller group in the sample application demonstrates common functionality in DynamoDB, such as creating tables, writing/reading items, listing tables and items, and deleting tables and items.

To accompany the sample application, this file also contains a set of *PowerShell* and *curl* commands that can be used to exercise the application's web API from a command shell or a tool like Postman. The web API application itself can be found in the SampleApplication subfolder of this guide.

* Links to documentation
  * [Amazon DynamoDB](https://aws.amazon.com/dynamodb/)
  * [Amazon DynamoDB Developer Guide](https://aws.amazon.com/dynamodb/developer-resources/)

### Prerequisites

* .NET Core 3.1 SDK or higher installed
* AWS Account with credentials configured locally in an AWS credential profile named **local-test-profile**. See [Configuring the AWS SDK for .NET with .NET Core](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-netcore.html) for more information.

> Note: You can update the sample to use a different credential profile by editing the *appsettings.Development.json* file in the *SampleApplication* subfolder.

* Optional: Visual Studio 2019+ (you can also use the command line for .NET Core)

## Included in the Web API Application

The sample application code demonstrates how to use the *AWSSDK.Extensions.NETCore.Setup* NuGet package to configure and add an AWS SDK DynamoDB client through the built in dependency injection (DI) container provided by .NET Core. Additionally, each API Controller group demonstrates the three programming models available for working with DynamoDB using the AWS SDK for .NET. For more information about the .NET DynamoDB programming models [see here](https://docs.aws.amazon.com/sdk-for-net/v2/developer-guide/dynamodb-intro.html).

## Running the Sample Application

To run through the DynamoDB functionality using the Web API project, open a command shell and navigate to the project folder, then run the following command to build (compile) and execute:

```bash
dotnet run
```

If you want to run the app again without compiling, just pass the --no-build flag, for example:

```bash
dotnet run --no-build
```

The Web API will run and you can use the *PowerShell* or *curl* scripts shown below to test the functionality.

## PowerShell Commands for Testing Amazon DynamoDB through Web API

### Low-level API

#### List All Tables

```powershell
Invoke-RestMethod http://localhost:5000/api/lowlevel/tables
```

#### Create a Table

```powershell
$URI = "http://localhost:5000/api/lowlevel/tables"
$BODY = Get-Content -Raw -Path data\create_table.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Get a Table

```powershell
Invoke-RestMethod http://localhost:5000/api/lowlevel/tables/Products
```

#### Get a Table Status

```powershell
Invoke-RestMethod http://localhost:5000/api/lowlevel/tables/Products/status
```

#### Update a Table

```powershell
$URI = "http://localhost:5000/api/lowlevel/tables/Products"
$BODY = Get-Content -Raw -Path data\update_table.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Delete a Table

```powershell
Invoke-RestMethod -Method 'Delete' http://localhost:5000/api/lowlevel/tables/Products
```

#### Create an Item

```powershell
$URI = "http://localhost:5000/api/lowlevel/items"
$BODY = Get-Content -Raw -Path data\create_item.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Get an Item

```powershell
$QUERY = Get-Content -Raw -Path data\get_item.txt
Invoke-RestMethod $QUERY
```

#### Update an Item

```powershell
$URI = "http://localhost:5000/api/lowlevel/items"
$BODY = Get-Content -Raw -Path data\update_item.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Delete an Item

```powershell
$URI = "http://localhost:5000/api/lowlevel/items"
$BODY = Get-Content -Raw -Path data\delete_item.json
Invoke-RestMethod -Method 'Delete' -Uri $URI -ContentType 'application/json' -Body $BODY
```

### Document Model API

#### Get a Product

```powershell
Invoke-RestMethod http://localhost:5000/api/document/products/1234/2018-01-01
```

#### Create a Product

```powershell
$URI = "http://localhost:5000/api/document/products"
$BODY = Get-Content -Raw -Path data\create_item_document.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Update a Product

```powershell
$URI = "http://localhost:5000/api/document/products/1234/2018-01-01"
$BODY = Get-Content -Raw -Path data\update_item_document.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Delete a Product

```powershell
Invoke-RestMethod -Method 'Delete' http://localhost:5000/api/document/products/1234/2018-01-01
```

### Object Persistence Model API

#### Get a Product

```powershell
Invoke-RestMethod http://localhost:5000/api/opm/products/1234/08-01-2018
```

#### Create a Product

```powershell
$URI = "http://localhost:5000/api/opm/products"
$BODY = Get-Content -Raw -Path data\create_product.json
Invoke-RestMethod -Method 'Post' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Update a Product

```powershell
$URI = "http://localhost:5000/api/opm/products/1234/2018-01-01"
$BODY = Get-Content -Raw -Path data\update_product.json
Invoke-RestMethod -Method 'Put' -Uri $URI -ContentType 'application/json' -Body $BODY
```

#### Delete a Product

```powershell
Invoke-RestMethod -Method 'Delete' http://localhost:5000/api/opm/products/1234/2018-01-01
```

## curl Commands for Testing Amazon DynamoDB through Web API

### Low-level API

#### List All Tables

`curl http://localhost:5000/api/lowlevel/tables`

#### Create a Table

`curl -d @data/table.json -H "Content-Type: application/json" http://localhost:5000/api/lowlevel/tables`

#### Get a Table

`curl http://localhost:5000/api/lowlevel/tables/Products`

#### Get a Table Status

`curl http://localhost:5000/api/lowlevel/tables/Products/status`

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

`curl http://localhost:5000/api/opm/products/1234/08-01-2018`

#### Create a Product

`curl -d @data/create_product.json -H "Content-Type: application/json" -X POST http://localhost:5000/api/opm/products`

#### Update a Product

`curl -d @data/update_product.json -H "Content-Type: application/json" -X PUT http://localhost:5000/api/opm/products/1234/2018-01-01`

#### Delete a Product

`curl -X DELETE http://localhost:5000/api/opm/products/1234/2018-01-01`

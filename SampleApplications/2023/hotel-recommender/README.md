# Welcome to the .NET Sample App for Hotel Recommender!
​
## Introduction
​
Hotel Recommender is an app for recommending hotels using [Amazon Personalize](https://aws.amazon.com/personalize/).
​

The app uses the dataset from an [Expedia Hotels Recommendations competition](https://www.kaggle.com/c/expedia-hotel-recommendations/overview) on Kaggle.
​

More information on the dataset can be found [here](https://francescopochetti.com/recommend-expedia-hotels-with-amazon-personalize-the-magic-of-hierarchical-rnns/#The_Expedia_dataset).
​

The Hotel Recommender app consists of two .NET 6 C# projects 
- A CDK project for provisioning the Hotel Recommender cloud infrastructure 
- A console app for creating the machine learning data models and providing recommenderations for users



## Hotel Recommender AWS Cloud Development Kit (CDK)

AWS CDK is a framwork for provisioning cloud infrastructure in CloudFormation via a choice of several programming languages.

You can find out more about AWS CDK [here](https://docs.aws.amazon.com/cdk/v2/guide/home.html)

You should explore the contents of the hotel-recommender-cdk project. It provides a CDK app with an instance of the stack `HotelRecommenderStack` to provision the infrastructure needed for Hotel Recommender.

The CDK provisions the following:
​
- An [S3](https://docs.aws.amazon.com/AmazonS3/latest/userguide/Welcome.html) Bucket for storing the necessary data sets used by Amazon Personalize to provide recommendations
- [Role](https://docs.aws.amazon.com/IAM/latest/UserGuide/id_roles.html) and [Policies](https://docs.aws.amazon.com/IAM/latest/UserGuide/access_policies.html) for accessing S3 and Amazon Personalize services
- [Dataset and schemas](https://docs.aws.amazon.com/personalize/latest/dg/how-it-works-dataset-schema.html) required by Amazon Personalize for [user data](https://docs.aws.amazon.com/personalize/latest/dg/users-datasets.html), [item data](https://docs.aws.amazon.com/personalize/latest/dg/items-datasets.html) and [interaction data](https://docs.aws.amazon.com/personalize/latest/dg/interactions-datasets.html)
- A [Custom dataset group](https://docs.aws.amazon.com/personalize/latest/dg/custom-dataset-groups.html). You can find out more about Customer dataset groups [here](https://docs.aws.amazon.com/personalize/latest/dg/data-prep-ds-group.html)
​

The `cdk.json` file tells the CDK Toolkit how to execute your app.

​
It uses the [.NET CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.
​
## How to run the CDK project for Hotel Recommender
From the root directory of this project (where the `cdk.json` file is located), run the following command: 
​
```
    cdk deploy
```
​
When prompted with the question 

```
    Do you wish to deploy these changes (y/n)? 
```

Type `y` in the terminal and press enter

## Console app

To run the console app please ensure ensure you first run the CDK project as provided in the instructions above

### Initial Run

Change to the directory `src/hotel-recommender-app`

The first ever time you run the app, the app will have to:
​
- Run the data import jobs - read [here](https://docs.aws.amazon.com/personalize/latest/dg/data-prep.html) for more information.
- Create the solution and solution version - read [here](https://docs.aws.amazon.com/personalize/latest/dg/training-deploying-solutions.html) for more information.
- Create the campaign - read [here](https://docs.aws.amazon.com/personalize/latest/dg/campaigns.html) for more information.
​

To run the app first time run with the (`-i`) command line argument in the directory `src/hotel-recommender-app` as this will run the above steps:


```
    dotnet run -i
```
​
The app will run through the above steps, waiting and checking until the step  is (`ACTIVE`) before moving on to the next. The app provides (`-t`) flag to allow a developer to specify in milliseconds how long to wait. 
​

To run the app with a wait time of 30 seconds, run the app with the following command line argument
​
```
    dotnet run -w 30000 
```
​
### Console app arguments

The console app allows you to specify command line arguments, in the event a developer has to run the app for specific steps, be sure the read the command line options in [program.cs](src/hotel-recommender-app/Program.cs).

The table below also provides provides a summary of the command line arguments

| Argument (short) | Argument (long) | Description |
| ---------------- | --------------- | ----------- |
| ​-t | --waittime | (Default: 60000) Set wait time in milliseconds to wait for models to complete |
| -i | --initialload | (Default: false) Set initial load to true if running for the first time |
| -d | --dataimport | (Default: false) Create import jobs |
| -s | --solution | (Default: false) Create solutions |
| -v | --solutionarn | (Default: ) Provide solution arn to create solution version
| -c | --solutionversionarn | (Default: ) Provide solution version arn to create campaign |
             

### Getting recommendations
​
For getting recommendations ensure the CDK and console app is run as detailed in the steps above
​

The console app can then be run with no command line arguments in the directory `src/hotel-recommender-app` with just a 
​
```
    dotnet run
```
​
When prompted with the question 
​
```
    Getting recommendations, please enter your userId ->
```
​
Type `45896486` (or any user id you wish) in the terminal and press enter

The response from the app will be similar to the below
​
```
    Hello User 45896486
    Amazon Personlize recommnds the following hotel for you 91
    Amazon Personlize recommnds the following hotel for you 48
    Amazon Personlize recommnds the following hotel for you 28
...
```
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

**Training the solution**

Please note that the commands below may take some time to execute when running for the first time. In some cases, this can take anywhere from 20 minutes to 45 minutes. The reason for the delay is that the commands are used to train solutions, which involves training machine learning models with data. So, please keep this in mind and be patient when running these commands for the first time.

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

The app will run through the above steps, waiting and checking until the step is in the  (`ACTIVE`) state before moving on to the next. The app provides the (`-t`) flag to allow a developer to specify, in milliseconds, how long to wait before checking the status of the step again.
​
To run the app with a wait time of 30 seconds, you would add the -t flag followed by the value 30000 (which represents 30 seconds in milliseconds) to the command line, like this:

```
    dotnet run -w 30000 
```
​
### Console app arguments

It's important for developers to read and understand the available command line options in [program.cs](src/hotel-recommender-app/Program.cs) when running the console app. This allows developers to customize the behavior of the app for their specific use case. By understanding the command line options, developers can modify how the app interacts with the user and the underlying services.

This table provides a summary of the command line arguments for the console app:

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

The console app can then be run with no command line arguments in the directory `src/hotel-recommender-app` with just

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
Type `45896486` (or any user id you can find in ./hotel-recommender/src/hotel-recommender-cdk/data/users.csv) in the terminal and press enter

The response from the app will be similar to the below
​
```
    Hello User 45896486
    Amazon Personlize recommends the following hotel for you 91
    Amazon Personlize recommends the following hotel for you 48
    Amazon Personlize recommends the following hotel for you 28
...
```

At this point, Amazon Personalize is using your ID to find recommendations on hotels and providing the IDs of those recommended hotels.

### Future Enhancements

As you can imagine, the usage of IDs for users or hotels is not very user-friendly. Typically, users are more comfortable with names, such as a hotel name or their own name. An enhancement could involve correlating the IDs to a username and a hotel name. This would provide a much better user experience by displaying names instead of IDs. Typically, we could store the user ID, hotel ID, and names in a datastore like a database and retrieve the names based on the ID.

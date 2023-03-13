# Welcome to the .NET Sample App for Hotel Recommender!

# Introduction

Hotel Recommender is an app for the recommendation of hotels using [Amazon Personalize](https://aws.amazon.com/personalize/)

The app uses the dataset which comes with the [Expedia Hotels Recommendations competition](https://www.kaggle.com/c/expedia-hotel-recommendations/overview) on Kaggle.

More information on the dataset can be found [here](https://francescopochetti.com/recommend-expedia-hotels-with-amazon-personalize-the-magic-of-hierarchical-rnns/#The_Expedia_dataset)

## CDK
You should explore the contents of this project. It demonstrates a CDK app with an instance of a stack (`HotelPickerStack`)
which contains an Amazon SQS queue that is subscribed to an Amazon SNS topic.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Useful commands

* `dotnet build src` compile this app
* `cdk ls`           list all stacks in the app
* `cdk synth`       emits the synthesized CloudFormation template
* `cdk deploy`      deploy this stack to your default AWS account/region
* `cdk diff`        compare deployed stack with current state
* `cdk docs`        open CDK documentation

The CDK provisions the following 

- S3 Bucket for storing the neccesary data sets Users, Items and Interactions
- Role and Policies for accessing S3 and Amazon Personalize services
- [Dataset and schemas](https://docs.aws.amazon.com/personalize/latest/dg/how-it-works-dataset-schema.html) for each data
- A [Custom dataSet group](https://docs.aws.amazon.com/personalize/latest/dg/custom-dataset-groups.html)

## Recommendation App - Getting recommendations

**Prerequisite** Ensure you have run the CDK project before running the app

The App Source code can be found at [hotel-recommender-app](./src/hotel-recommender-app/)

### Initial Run
**Prerequisite** Ensure you have enter your AWS AccountID and Region in [program.cs](./src/hotel-recommender-app/Program.cs) and they are the same used to run the CDK app

The first ever time you run the app, the app will have to 

- Run the data import jobs - read [here](https://docs.aws.amazon.com/personalize/latest/dg/data-prep.html)
- Create the solution and solution version - read [here](https://docs.aws.amazon.com/personalize/latest/dg/training-deploying-solutions.html) for further information
- Create the campaign - read [here](https://docs.aws.amazon.com/personalize/latest/dg/campaigns.html) for further information

To run the app first time run with the (`-i`) command line argument as this will run the above steps

```
    dotnet run -i
```

The app will run through the above steps, waiting and checking until the step  is (`ACTIVE`) before moving on to the next. The app provides (`-t`) flag to allow the developer to specify in milliseconds how long to wait. 

To run the app with a wait time of 30 seconds, run the app with the following command line argument

```
    dotnet run -w 30000 
```

The following commandline arguments are supported in the event a developer has to run the app for specific steps, be sure the read the command line options in [program.cs](./src/hotel-recommender-app/Program.cs)

### Getting recommendations

For getting recommendations ensure the CDK and App is run with the initial load command line argument (`-i`).

The app can then be run as normal with 

```
    dotnet run
```

The developer is presented with the option to enter a userId

```
    Getting recommendations, please enter your userId ->
```

The response from the app will be similar to the below

```
    Hello User 45896486
    Amazon Personlize recommnds the following hotel for you 91
    Amazon Personlize recommnds the following hotel for you 48
    Amazon Personlize recommnds the following hotel for you 28
...
```

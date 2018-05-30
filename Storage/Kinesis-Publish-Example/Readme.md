# Publish data into Kinesis using .NET SDK

## Step-by-Step Walkthrough
--------

**Overview and Services Used:**

Amazon Kinesis makes it easy to collect, process, and analyze real-time, streaming data so you can get timely insights and react quickly to new information. Amazon Kinesis offers key capabilities to cost-effectively process streaming data at any scale, along with the flexibility to choose the tools that best suit the requirements of your application. 

The walk-through, we’ll use the AWS Management Console to make a Kinesis Data Stream. Next, we’ll create a new .NET console application and add the AWS SDK for Kinesis to use it. The sample application will run like simulated IoT devices which are sending temperature and humidity values collected from their sensors into Amazon Kinesis stream. Finally, we will use Amazon Kinesis Data Analytics to aggregate average temperature values in real-time.

+ Links to documentation

  * Amazon Kinesis Page: https://aws.amazon.com/kinesis/
  * Amazon Kinesis Data Streams Page: https://aws.amazon.com/kinesis/data-streams/
  * Amazon Kinesis Data Streams Developer Guide: https://docs.aws.amazon.com/streams/latest/dev/introduction.html
  * Amazon Kinesis Data Streams API Reference: https://docs.aws.amazon.com/kinesis/latest/APIReference/Welcome.html
  * Amazon Kinesis Data Analytics Page: https://aws.amazon.com/kinesis/data-analytics/
  * Amazon Kinesis Data Analytics Developer Guide: https://docs.aws.amazon.com/kinesisanalytics/latest/dev/what-is.html
  * AWS Kinesis Data Analytics SQL Reference: https://docs.aws.amazon.com/kinesisanalytics/latest/sqlref/analytics-sql-reference.html

+ Prerequisites
  * AWS CLI
  * AWS Account with credentials configured locally in Visual Studio or using the CLI
  * Visual Studio 2015 or higher


+ External libraries:
	* NuGet Package AWSSDK.Kinesis
	* NuGet Package Newtonsoft.Json
	

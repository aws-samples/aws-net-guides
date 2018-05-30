# **Sending Emails with AWS SES and .NET**

## Step-by-Step Walkthrough
### Level 200 Content
--------

#### Create a .NET Framework or .NET Core 2 App to Send Email via Amazon Simple Email Service 

--------

**Overview and Services Used:**

This walk-through comprises a Microsoft Word doc, along with a .NET Core console application. The only service used is Amazon Simple Email Service (SES). The walk-through includes verifying the sender email address in the console, creating and configuring a simple console app in either Visual Studio or via the command line, and then running the app to actually send the email. The code could easily be run in a Lambda function or anywhere else .NET code (Framework or Core) can run.
	
+ Links to documentation
	* SES Service Page:  https://aws.amazon.com/ses/
	* SES Documentation:  https://docs.aws.amazon.com/ses/latest/DeveloperGuide/Welcome.html 

+ Prerequisites
	* .NET Framework 3.5 or higher, **or** .NET Core 2.0 or higher installed
	* AWS Account with credentials configured locally in Visual Studio or using the CLI
	* Optional: Visual Studio 2017 (you can also use the command line for .NET Core)

+ External libraries:
	* NuGet Package AWSSDK.SimpleEmail (adding package is included in walk-through)
# Deploy an ASP.NET Core Application to Amazon EC2 using AWS CodeDeploy

## Overview

In this guide, you will configure build tasks from the [AWS Toolkit for Azure DevOps](https://aws.amazon.com/vsts/) to deploy an ASP<span></span>.NET web application from an Azure DevOps project to [Amazon EC2](https://aws.amazon.com/ec2/?ec2-whats-new.sort-by=item.additionalFields.postDateTime&ec2-whats-new.sort-order=desc) using [AWS CodeDeploy](https://aws.amazon.com/codedeploy/)

## Architecture

![architecture](./media/image1.png)

## Modules
1.  Create a New Project in Azure DevOps or Azure DevOps Server, or use an existing project
1.  Create an ASP<span></span>.NET MVC web application
1.  Configure the project to support deployment using [AWS CodeDeploy](https://aws.amazon.com/codedeploy/)
1.  Check-in to Azure DevOps source control
1.  Create an [Amazon S3](https://aws.amazon.com/s3/) bucket to store the built application during deployment
1.  Set up an [AWS Identity and Access Management](https://aws.amazon.com/iam/) (IAM) Role for your EC2 instance(s).
1.  Create an EC2 instance
1.  Set up an IAM Service Role for AWS CodeDeploy
1.  Create an AWS CodeDeploy application and deployment group resource
1.  Add a task in your Azure DevOps build pipeline to deploy the application using AWS CodeDeploy
1.  Test the Deployment


### Module 1: Create a New Project in Azure DevOps or Azure DevOps Server, or use an existing project

#### Overview
In this module we will login to Azure DevOps or Azure DevOps Server to create a New Project for our application

> Note: You can also use an existing project if you have one.
#### Implementation instructions

1. Login to Azure DevOps using your credentials

    ![new_project](./media/azureproject.png)

1. Click **New Project**

    ![new_project](./media/create_project.png)

1. The project window will open 
1. Click on **Repos** > **Files** (This is the location where we will add our sample ASP<span></span>.NET application code later)
1. Select **Visual Studio** under *Initialize main branch with a ReadME or gitignore*
1. Click **Initialize**

    ![new_project](./media/repo.png)


### Module 2: Create an ASP<span></span>.NET MVC web application

#### Overview

In this module we will use Visual Studio Community edition to create an
ASP<span></span>.NET MVC web application.

#### Implementation Instructions

1. Open Visual Studio
1. Select **View** > **Team Explorer**

    ![team_explorer](./media/team.png)

1. Select **Home** > **Projects and My Teams** > **Manage Connections**

    ![team_explorer](./media/team_connection.png)

1. Select **Connect to a Project**

    ![team_explorer](./media/connect_project.png)

1. Select the project SampleWebApp created in Module 1 and click **Connect**

    ![team_explorer](./media/connect_to_project.png)

1. In the *Team Explorer - Home*, under *Project*, click **Clone Repository** > **Clone**

    ![team_explorer](./media/clone_target.png)


1.  Select **File** \> **New** \> **Project...**

    ![new_project](./media/image2.png)

1.  The project dialog window will popup. Select the *ASP<span>.</span>NET Core Web App* template.

    ![new_visual_studio_project](./media/new_project.png)

1.  Enter the project name. Create the project in the same location where you cloned the repository
1.  Click **Next**
    ![new_visual_studio_project](./media/new_project2.png)

1.  Select **<span>.</span>NET 5.0 (Current)** as the *Target Framework*
1. Click **Create**

    ![new_visual_studio_project](./media/new_project3.png)

### Module 3: Configure the project to support deployment using [AWS CodeDeploy](https://aws.amazon.com/codedeploy/)

#### Overview

Now that we have an ASP<span></span>.NET application, let's add the files needed to allow AWS CodeDeploy to understand the deployment and configuration.
There are two files we will need to add:

1.  **appspec.yml** -- The application specification file is a YAML or
    JSON-formatted file used by AWS CodeDeploy to manage a deployment.
1.  **installapp.sh** -- A script to deploy the application binaries, held in a WebDeploy package file, into IIS on the EC2 instance.

Implementation Instructions

1.  Create the appspec.yml file by right clicking on the project \>
    **Add** \> **New Item...**

    ![new_item](./media/image5.png)

1.  Name the file appspec.yml and click **Add**.
   
    ![new_item](./media/create_yml.png)


1.  Copy the following code, paste into the file and save.

    ```yaml
    version: 0.0
    os: linux
    files:
      - source: /appbin/SampleWebApp
        destination: /var/www

    hooks:
      AfterInstall:
        - location: ./installapp.sh

    ```

1.  Use the same process to create the installapp.ps1

1.  Copy the following code, paste into the installapp<span>.</span>sh file and
    save. This specifies the path to the application *.dll* file to run the application in the background

    ```
    dotnet /var/www/SampleWebApp.dll > /dev/null 2>&1 &
    ```
1. Modify the Program<span>.</span>cs to be as follows
    ```
    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => { options.Listen(IPAddress.Any, 1025); });
                    webBuilder.UseStartup<Startup>();
                });
    ```

### Module 4: Check-in to Azure DevOps source control

#### Overview

Now that we have the code we want deployed to EC2, we need to check it
into a source control system. In this module we will add our code to
Azure DevOps Repos through the Git window in Visual Studio.

#### Implementation Instructions

1.  With the ASP<span></span>.NET Core project open,  select **Git** from the menu bar, select **Commit or Stash...**

    ![publish_git_repo](./media/git.png)

1. Enter a commit message and select **Commit All and Push**

    ![publish_git_repo](./media/git_commit.png)

1.  You will see a message once the push is complete.
   
    ![push_complete](./media/git_success.png)

### Module 5:  Create an [Amazon S3](https://aws.amazon.com/s3/) bucket to store the built application during deployment

#### Overview

In this module, we will create an Amazon S3 bucket to which the revision bundle will be uploaded. A revision in AWS CodeDeploy is a version of an application you want to deploy. Our sample application revisions will be stored in Amazon S3

#### Implementation

1. From the AWS Console search for S3.
1. Click **Create bucket**

    ![s3_bucket](./media/s31.png)

1. Enter the *Bucket name* and select the *AWS Region*. The *AWS Region* needs to be in the same region as the CodeDeploy resources which will be subsequently created in the tutorial.

    ![s3_bucket](./media/s32.png)

1. Leave the remaining settings to default and click **Create bucket**

    ![s3_bucket](./media/s33.png)

### Module 6: Set up an [AWS Identity and Access Management](https://aws.amazon.com/iam/) (IAM) Role for your EC2 instance(s).

#### Overview

In this step we will create an IAMs role which will give EC2 instances the required access to Amazon S3

#### Implementation Instructions

1.  From the AWS Console search for IAM.
1.  Select **Roles** > **Create Role**

    ![New_Role](./media/new_role.png)

1.  Select **AWS service** under *Select type of trusted entity*
1.  Select **EC2** under *Choose a use case*
1.  Click **Next: Permissions**

    ![New_Role](./media/role2.png)

1. Click **Create Policy**

    ![New_Role](./media/role3.png)

1. Add the following permission to the policy as follows
    - *Service*: S3
    - Select *GetObject* under *Read* *Actions*
    - Select *Specific* for *Resources*
1. Click **Add ARN**

    ![New_Role](./media/bucket2.png)

1. Enter the *Bucket name* created in Module 5
1. Select *Any* for *Object name*. Specifying the Amazon Resource Name(ARN) will give the specified Read permission only to the specified bucket 
1. Click **Add**

    ![New_Role](./media/bucket1.png)

1. Click **Next: Tags**

    ![New_Role](./media/bucket5.png)

1.  We will not be adding any Tags. Click **Next: Review**

    ![New_Role](./media/bucket4.png)


1.  Enter *Name*: s3-policy
1.  Click **Create policy**

    ![New_Role](./media/bucket6.png)


1.  In the Create role window, select the policy created above 
1.  Click **Next: Tags**

    ![New_Role](./media/role4.png)

1.  We will not be adding any Tags. Click **Next: Review**

    ![New_Role](./media/bucket3.png)

1.  Enter the *Role name* and click **Create role**

    ![New_Role](./media/role5.png)


### Module 7: Create an EC2 instance

#### Overview

Now we will create an EC2 instance where our sample application code will be deployed.

#### Implementation Instructions

1.  From the AWS Console search for EC2.
1.  Click on **Instances** > **Launch instances**

    ![launch_instance](./media/ec21.png)

1.  **Select** the Free tier eligible *Amazon Linux 2 AMI (HVM)*

    ![instance_type](./media/ec2_create1.png)

1.  Select *Type* - *t2.micro*
1.  Click **Next: Configure Instance Details**

    ![instance_type](./media/ec23.png)

1.  For *Network*, select default VPC
1.  For *IAM role*, select the role you created in Module 6

    ![instance_type](./media/ec24.png)

1. Copy the following code, paste into the User data As text.

    ```
    #!/bin/bash
    yum -y update
    rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
    yum -y install dotnet-runtime-5.0
    yum -y install aspnetcore-runtime-5.0
    yum -y install ruby
    yum -y install wget
    cd /tmp
    wget https://aws-codedeploy-us-west-2.s3.us-west-2.amazonaws.com/latest/install
    chmod +x ./install
    ./install auto
    service codedeploy-agent start

    ```

1.  The above code initializes the EC2 instance with our needed software at launch
    time using a UserData script. It downloads and installs the CodeDeploy agent, installs <span>.</span>NET 5 and ASP<span>.</span>NET Core packages
1.  Click **Next: Add Storage**

    ![User_Data](./media/ec2_create2.png)

1.  Keep the default settings for storage
1.  Click **Next: Add Tags**

    ![Add_Storage](./media/ec25.png)

1.  Enter *Key* - Name, *Value* - CodeDeployInstance. This tag will be used to identify the EC2 instance(s) that CodeDeploy should deploy our application to
1.  Click **Next: Configure Security Group**

    ![Add_Tags](./media/ec26.png)

1.  *Select an existing security group* and select the *default* option
1.  Click **Review and Launch**

    ![Configure_SG](./media/def_sg.png)

1.  Click **Launch**

    ![Launch](./media/ec28.png)

1.  Select **Proceed without a key pair** 
1.  Click **Launch instances**

    ![Key_Pair](./media/ec29.png)

1.  You can view the status of the EC2 instance by clicking **View Instances**

    ![View_Instance](./media/ec210.png)

### Module 8: Set up an IAM Service Role for AWS CodeDeploy

#### Overview

In this step we will create a Service Role which will grant AWS CodeDeploy access to the target EC2 instance.

#### Implementation Instructions

1.  From the AWS Console search for IAM.
1.  Select **Roles** > **Create Role**

    ![New_Role](./media/new_role.png)

1.  Select **AWS service** under *Select type of trusted entity*.
1.  Select **CodeDeploy** under *Choose a use case*

    ![New_Role](./media/service_role.png)

1.  Click **Next: Permissions**

    ![New_Role](./media/role.png)

1.  A policy has already been attached. Click **Next:Tags**

    ![New_Role](./media/create_role.png)

1.  We will not be adding any Tags. Click **Next: Review** 

    ![New_Role](./media/role_tag.png)

1.  Enter *Role name* and click **Create role**

    ![New_Role](./media/role_final.png)


### Module 9: Create an AWS CodeDeploy application and deployment group resource

#### Overview

CodeDeploy is a deployment service that automates application deployments to Amazon EC2 instances, on-premises instances, serverless Lambda functions, or Amazon ECS services.

In this step we will create an application which is a name or container used by CodeDeploy to ensure that the correct revision, deployment configuration, and deployment group are referenced during a deployment. We will also create a deployment group. Each application deployment uses one of its deployment groups. The deployment group contains settings and configurations used during the deployment.
deployment group in CodeDeploy.

#### Implementation Instructions

1.  From the AWS Console, search for CodeDeploy.

    ![codedeploy_search](./media/codedeploy.png)

1.  Go to **Applications**, click **Create application**

    ![get_started_now](./media/cd_app.png)

1.  Enter the *Application name*
1.  Select **EC2/On-premises** under *Compute platform* and click **Create application**

    ![sample_deployment](./media/cd_create_app.png)

1.  Go to the application and click **Create deployment group**

    ![inplace_deployment](./media/cd_create_grp.png)

1.  Enter the *deployment group name*
1.  Select the *Service Role* created in Module 8

    ![windows_type](./media/cd_grp1.png)

1.  Choose **In-place deployment**
1.  Select **Amazon EC2 instances** under *Environment configuration*
1.  Add a tag with *Key* - Name, *Value* - (Name of the EC2 instance created in Module 7) This will identify the EC2 instance created in Module 7

    ![select_windows_sample](./media/cd_grp2.png)

1.  Select **Never** under *Agent configuration with AWS Systems Manager* as we have already configured the EC2 instance with the CodeDeploy Agent.
1.  Select **CodeDeployDefault.AllAtOnce** under *Deployment Settings*. This option attempts to deploy an application revision to as many instances as possible at once. We will be dealing with a single instance.
1.  Since its a single instance application we will not be needing a Load balancer. Deselect *Enable load balancing*
1.  Click **Create deployment group**

    ![service_role](./media/cdgrp3.png)

1.  CodeDeploy is ready to receive our application.

### Module 10: Add a task in your Azure DevOps build pipeline to deploy the application using AWS CodeDeploy

#### Overview

In this step we will create an Azure DevOps build job that will deploy our
application to EC2 instances via CodeDeploy.

#### Implementation Instructions

1.  Open our project in Azure DevOps, select **Pipelines** > **Create Pipeline**

    ![new_build](./media/pipeline1.png)

1.  Select **Other Git**

    ![vsts_git](./media/pipeline2.png)

1.  Choose the **Azure Repos Git**, select your project and Repository
1.  Click **Continue**

    ![.net_template](./media/pipeline3.png)

1.  Select the **ASP<span></span>.NET** template and click **Apply**

    ![add_task](./media/image21.png)

1. Select the Tasks - *Build solution*, *Test Assemblies*, *Publish symbols path* and *Publish Artifact* and select **Remove selected task(s)**. We won't be needing these three tasks.

    ![add_copy_files_task](./media/create_pipeline1.png)

1.  Click the + symbol to add a new task.

    ![add_task](./media/new_task.png)

1.  Search for the *<span>.</span>NET Core* task and click **Add**. The task will go to
    the bottom of the list on the left.

    ![add_net_core_task](./media/create_pipeline2.png)

1.  Select the *<span>.</span>NET Core* task and update the parameters as shown in the image.

    - *Display name*: It specifies the name of the task in the pipeline
    - *Command*: Select *publish*. This will publishes the application and its dependencies to the specified output folder for deployment to a hosting system which in this case is an EC2 instance(s)
    - *Arguments*: Enter the following. It specifies the folder for the output of *dotnet publish* command

    ```
    --output $(build.artifactstagingdirectory)/appbin
    ```

    ![update_net_core_Task](./media/create_pipeline3.png)

1.  We will add another new task. Click the + symbol. Search for the *Copy Files* task and click **Add**. The task will go to
    the bottom of the list on the left.

    ![add_copy_files_task](./media/image23.png)

1.  Select the *Copy Files* task and update the parameters as shown in the image.

    - *Source Folder*: It is the folder that contains the files you want to copy. Select the project folder - SampleWebApp
    - *Contents*: The specified files mentioned here will be copied to the specified Target folder containing other build artifacts which will be used by the CodeDeploy task when it builds the application bundle for deployment. Enter *installapp<span>.</span>sh* and *appspec.yml* as shown below

    ![update_copy_files_Task](./media/create_pipeline4.png)

1.  We will be adding *AWS CodeDeploy Application Deployment* task in the next step . The *AWS Toolkit for Azure DevOps* adds tasks to easily enable build and release pipelines in Azure DevOps. If you are using the toolkit for the first time, you can install and configure it from the [Visual Studio Marketplace.](https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.aws-vsts-tools)
1.  Click the + symbol to add a new task. Search for the CodeDeploy task
    and click **Add**. The Task will go to the bottom of the list on the
    left. Update the parameters.

    ![add_codedeploy_task](./media/image25.png)


1.  Configure the Code Deploy task.
    - *AWS Credentials* - Specify the AWS credentials to be used by the task in the build agent environment
    - *AWS Region* - Specify the region containing the AWS resources for this project
    - *Application Name* - Specify the name of the AWS CodeDeploy application (Module 9)
    - *Deployment Group Name* - Specify the name of the deployment group the revision is to be deployed to (Module 9)
    - *Revision Bundle* - Specify the location of the application revision artifacts to deploy which is *$(build.artifactstagingdirectory)*
    - *S3 Bucket Name* - Specify the name of the Amazon S3 bucket to which the revision bundle is uploaded or can be found (Module 5)
    - *Existing File Behavior* - This specifies how AWS CodeDeploy should handle files that already exist in a deployment target location but weren't part of the previous successful deployment. Select *Overwrite the version already on the instance with the version in the new application revision*

    ![configure_task](./media/pipeline4.png)


1.  Select **Triggers** and enable *Continuous integration*

    ![enable_ci](./media/pipeline6.png)

1.  Save the build.

### Module 11: Test the Deployment

#### Overview

In this module, we will deploy the code to the EC2 instance(s).

#### Implementation

1.  Make a change to your code and check it into Azure DevOps which initiates a
    build. Once the build is complete you will be able to click through
    the various steps in the process.

    ![initiate_build](./media/build_success.png)

1.  You can view your deployed application by getting the public DNS or
    IP address for the EC2 instance and appending the port added in Step 6 of Module 3 and navigate to the site in a browser.

    ![view_deployed_app](./media/aspnet.png)

1.  You now have a working CI/CD pipeline in Azure DevOps that deploys an ASP<span></span>.NET
    Core application from source control to EC2.




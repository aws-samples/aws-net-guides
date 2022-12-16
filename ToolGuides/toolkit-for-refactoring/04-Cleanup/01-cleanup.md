# Cleaning up your resources

It is important to clean up the resources that have been deployed into your AWS Account to prevent unintentional charges from being accumulated in your account. This will require a combination of manual and automated processes.

## Clean up S3
The deployment of the CloudFormation template has created an S3 bucket in your account. This bucket will contain copies of the images that you have uploaded to the application. CloudFormation cannot automatically delete these files, as they were not part of the stack deployment. You should manually delete the content of that bucket.

## Clean up DynamoDB
The deployment process created three Dynamo DB tables. These tables contain the metadata for the images that have been uploaded to your application. Cloud Formation will not automatically delete these tables as they have user data in them. Delete the following DynamoDB tables manually.
* MetadataService-files
* MetadataService-Images
* MetadataService-Lookups

## Delete Cloud Formation Stack
Once the manual cleanup has been completed, you can go into CloudFormation and delete the stack that you deployed in the first section of this guide. Cloud formation will clean up the rest of the resources in your account.

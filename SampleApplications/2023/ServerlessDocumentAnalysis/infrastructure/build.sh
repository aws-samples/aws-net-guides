dotnet lambda package -pl ../functions/src/ProcessingFunctions/ -o ./function-output/ProcessingFunctions.zip
dotnet lambda package -pl ../functions/src/SubmitToTextract/ -o ./function-output/SubmitToTextract.zip
dotnet lambda package -pl ../functions/src/ProcessTextractQueryResults/ -o ./function-output/ProcessTextractQueryResults.zip
dotnet lambda package -pl ../functions/src/RestartStepFunction/ -o ./function-output/RestartStepFunction.zip
dotnet lambda package -pl ../functions/src/ProcessTextractExpenseResults/ -o ./function-output/ProcessTextractExpenseResults.zip
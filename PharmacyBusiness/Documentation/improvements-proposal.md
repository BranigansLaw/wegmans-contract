# Pharmacy Business Improvements Proposal

This document highlights improvements to the Pharmacy Business project.

## Goals

- Culture of Currency
  - [Most recent versions of .NET](#net-version)
  - [Better security practices](#application-secrets)
  - [Remove high level complexity for support](#continue-using-control-m)

- The following is a list of improvements that should be made to the Pharmacy Business project:
  - [Upgrade from .NET 4.8 to modern .NET](#net-version)
  - [Jobs are re-run from the beginning every time](#continue-using-control-m)
  - [Remove the ability for jobs to re-run from a mid-point step](#continue-using-control-m)
  - [Logging for all jobs is in one place](#logging)
  - [When errors occur, a ticket to the support team is created](#logging)
  - [Jobs are run from Control-M for audit and ease of access purposes](#continue-using-control-m)
  - [Job kickoff and scheduling is handled by Control-M](#continue-using-control-m)
  - [Remove Interface Engine](#continue-using-control-m)
  - [Use C# retry logic for all network calls (database and otherwise) to prevent jobs from failing due to network issues](#network-connection-retry)
  - [Move application secrets out of drive readable appsettings files](#application-secrets)
  - [Code with Test Driven Development](#test-driven-development)

## Solutions

### .NET Version

Build using latest version of .NET 8.0.x.

### Continuous Integration Pipeline

Azure DevOps pipeline will be used to build, run unit tests, and deploy the jobs to the on-prem server.

### Continue using Control-M

We will continue to use Control-M for audit and ease of access purposes. The code will compile into a single executable that does not require Interface Engine to run.

#### Single Step Control-M Jobs

Jobs will be, as much as possible, run on in a single step via a compiled EXE

##### Potential Exceptions

- Upload to NFM may need to be a separate Control-M step **needs investigation**

#### Job Scheduling

Job scheduling will be handled by Control-M. The EXE should be agnostic to when it is run, and all available resources that it requests should be available by the time the EXE is run.

*Note: At this time, 1010 is implementing a flag that will upload a file to our system when then data is ready. Control-M should have a watch setup for this file and when it is present, kick off the related EXE for that job.*

#### One Project per Job

Each Job will be a runtime argument passed to the main project EXE (INN.JobRunner). This will allow for a single pipeline to build all jobs, and a single EXE to be deployed to the on-prem server.

#### `Cocona`

Command line arguments will be handled by the `Cocona` library. This allows for easy setup of new arguments, options handling, and command line help documentation out of the box.

##### `Cocona` Documentation

https://github.com/mayuki/Cocona

### Logging

Application Insights used for logging in all jobs to centralize error detection and reporting.

#### AppInsights Requirements

- The on-prem server should have access to make Application Insights logs
- The AI resource should be setup in an Azure pipeline
- No secrets or client identifying information should be logged in Application Insights
- When an error or custom event of a certain type occurs, a ticket should be created assigned to the appropriate support team. If no appropriate team is found for the error, a ticket should be assigned to the Pharmacy team.
- On job completion (whether job success or failed) log to Control-M that detailed logs are available in ApplicationInsights with a link to the Azure resource

### Network Connection Retry

All network calls will be wrapped in a retry logic to prevent jobs from failing due to network issues. The C# pattern is as follows:
    
```csharp
public async Task<HttpResponseMessage> GetAsync(string url)
{
    var retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    return await retryPolicy.ExecuteAsync(async () =>
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return response;
    });
}
```

### Dataflow

The existing jobs work in Control-M configured steps. To simplify, we are moving these steps into the code itself. To configue these steps, we'll be using C# DataFlow.

#### Example

```csharp
public async Task RunDataFlowAsync(CancellationToken cancelationToken) {
    DataflowLinkOptions LinkOptions = new() { PropagateCompletion = true };
    
    step1.LinkTo(step2, LinkOptions);
    step2.LinkTo(step3, LinkOptions);
    step3.LinkTo(step4, LinkOptions);

    await step1.SendAsync(true, cancelationToken);
    step1.Complete();

    await step4.Completion;
}
```

#### Microsoft Documentation

https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library

### Application Secrets

Currently, application secrets are copied into the application's `appsettings.json` file by the classic pipeline. These secrets should be stored in a more secure place, like Azure Key Vault.

#### Azure Key Vault Requirements

- Executables will use a appKey/secret combination to authenticate with Azure Key Vault, and the Azure Vault will restrict IP access to the on-prem server
- A PIM role should be added to update key vault entries, but not read them
- Both the above should be setup in an Azure pipeline

##### Long Term Vision

At this time, Azure cannot authenticate with managed identities with the on-prem server. In the future, ARC managed identities may be setup so managed identity can be used to access secrets which would be more secure, but for the time being, the appKey/secret combination stored on the security restricted server is sufficient.

### Test Driven Development

Code will follow test driver development whereby for each job:

- The job will be it's own class and an `ExecuteAsync` method that kicks off the job logic. When the stub is created, it can simple throw a `NotImplementedException`
- Injected dependencies (`HttpClient`, Oracle clients, etc) are added to the constructor of the new job class
- A test class will be created for the job class injecting mocked dependencies
- Test cases for all happy paths and error paths will be created
- After all test cases are written, only then will implementation of the `ExecuteAsync` method begin

## Implementation Plan

### New Job Naming

New jobs will use a new naming convention that will be decided on in the future.

To better cover the different types of jobs that will be run, the 3-letter acronym should cover:
- Pharmacy
- Restaurants
- Bottle Return

Current chosen convention: **REX###**.

### Implementation

Jobs will be created, deployed, and added to Control-M before the old jobs are decomissioned. We can then run new REX jobs in parallel with old jobs as needed.

This gives us a rollback plan as well. If new jobs fail for some reason, the old jobs can be rerun until fixes around found.

Once an appropriate amount of time has passed and all the new jobs are succeeding on a regular basis, old jobs can be decommissioned.

## Work Breakdown

### Proof of Concept

- Create an exe running on the on-prem server that Control-M jobs are stored on that can read from Azure Key Vault
- Create an exe running on the on-prem server that Control-M jobs that logs to Application Insights
- Create a pipeline using the Wegmans Azure template (https://dev.azure.com/wegmans/DevOps/_git/pipeline-base-templates?path=/base/template.yml) that copies an EXE on to the Control-M server
- Run one of the compiled EXEs via Control-M
- Read/Write a file to the local file system via the .NET executable
- Read/Write a file into NFM via the .NET executable
- A sample linear to multi-path job implementing C# `DataFlow`
- A sample job that uses C# retry logic for network calls
- A query to an Oracle database (preferably from one of the existing Oracle databases used by the current jobs)
- Remotely execute a 1010 executable (ex. 10up or 10do)

### Setup Work

- Setup the project using conditional compilation for each of the job types required to be created
- Create the pipeline to deploy each of those resulting EXEs to the on-prem server that Control-M runs from
- Create the bicep file that creates the Azure Key Vault and related roles, and Application Insights resource that the application will use
- Create stories for each of the jobs to be implemented

### Job Implementation

Once the above skeleton has been created, jobs can be implemented using TDD as needed in any order.
# Future Improvements

## 1010 Parquet Files

### Federated Credentials
- Use Azure Federated Credentials to authenticate with 1010s Azure Blob storage instead of using keys and NAT (IP Whitelisting)
    - Similar to how we authenticate with Snowflake, Federated Credentials would tell 1010 which applications have write access to upload .parquet files. This would make authentication easier, safer, and not require updating keys in the vault when passwords change

## Retire NFM

### Problem
Scheduling is split between NFM and Control-M between jobs causing headaches for developers in remembering where jobs are called from. It also relies on an external system to manage uploads/downloads via SFTP output of code

### Plan
Move all file uploads/downloads into Azure Blobs and managed in C# code

### Solutions


## Job Scheduling with Azure Functions

### Problem
Schedule coordination with NFM and Control-M causes a lot of developer headaches and leads to human errors. Moving to Azure Timer fcuntions would mean our team would control the schedules of running the jobs

### Plan
Use Azure Timer Functions to schedule jobs instead of NFM and Control-M

### Solutions
- For logging, API calls to create change requests, or using Azures job log (not sure exactly where this is, but I believe that it logs running jobs in an Azure Table), or Application Insights could be used to give transparency into which jobs are run when
- For errors, we could either send team alerts via email, a teams channel (that would receive alerts of failures), or create bugs via an API when jobs fail so our team and the organization and on-call person is alerted when a job fails or returns an unexpected job code

## 100% Unit Test Code Coverage

### Problem

There is incosistent unit test code coverage throughout the project

### Plan

Prevent code check ins that add code without creating unit tests for them

### Solution

- All code that covers connecting to external resources should have the `IgnoreCodeCoverage` tag attached to it
- Code that accesses external resources should be very isolated, and have only the responsibility of creating the connection (no logic, `if`s, `for` loops, etc ...)
- All other code should use `NSubstitute` mocks where needed to test the logic
- A step in the pipeline after unit tests are run should check that code coverage remains at 100% and builds should fail if this requirements is not reached

## Linux Build Machine

### Problem

The build pipeline requires a Windows machine to build over Linux due to the use of some SQL Reader libraries (`DataReader` I believe) which leads to very slow build times over building on Linux machines.

### Plan

When old SQL connections are retired, or interfacing those SQL libraries can happen with a different tool, upgrade the pipeline to use Linux

### Solution
- Retire jobs that won't build in non-Windows environments
- Change the build setting [here](../pipelines/build.yml).
```yml
...
- stage: ${{ parameters.stageName }}
  displayName: Build
  pool:
    # Can be removed or changes to "Linux" when projects can be compiled as Linux
    # Currently, certain SQL database connections require the old Windows-only libraries for data mapping meaning the project has to be compiled as Windows
    # When these requirements are removed, this can changed to "Linux" and should give a massive bump to build speed
    vmImage: windows-latest
  jobs:
...
```
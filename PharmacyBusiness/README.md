# PharmacyBusiness

## RX.* Projects

These projects relate to existing Pharmacy Business batch jobs that run using the Interface Engine.

This repository contains code for Pharmacy Accounting related applications.
These applications will most likely take the form of ETLs to/from 1010data.

These should go in the **RX.Accounting.ETL** project.  Each job should have its own folder in the project, containing all code specific to the job, and named for the Control-M job.

DataExtracts and tenup/tendo jobs work the same way as ETLs.  The files for the job should be in a folder named for the Control-M job.  The difference is that these files do not get compiled, and therefore all of them need to be modified to be content to be included in the output.
To do this, right-click on the file and click on properties.  Set the **Build Action** to **Content**, and the **Copy to Output Directory** to **Copy always**.

## INN.* and Library.* Projects

These project relate to the new standalone EXE batch jobs that run without Interface Engine, and run as a single standalone Control-M job not broken down into steps.

### Development

#### Project Naming

- **INN.** projects are standalone EXE batch jobs that run without Interface Engine, and run as a single standalone Control-M job not broken down into steps.
- **Library.** projects are libraries that are used by one or more INN projects.

#### Development

##### Developing Jobs Testing Integrated Systems

The rights to all the different systems that the batch processes touch can't be granted to developers or have their machines whitelisted. For this reason, development involving integration with these live systems must be done by deploying to the test server and then running the EXE from there.

###### Instructions

You can use the [Pharmacy Business Developer Testing Pipeline](https://dev.azure.com/wegmans/Pharmacy/_build?definitionId=3212) for faster deployment as it skips several validation steps that the main pipeline uses. 

To run the pipeline:

1. Commit and publish any changes you'd like to test from your branch
2. Go to the [Pharmacy Business Developer Testing Pipeline](https://dev.azure.com/wegmans/Pharmacy/_build?definitionId=3212).
3. Click on the **Run pipeline** button.
4. Select the branch that you are working in from the drop down
5. In the **Project to Deploy** text box, copy the name of your project (do not include the *.csproj* extension).
6. Click the **Run** button.

##### Developing Jobs Without Testing Integrated Systems

To be expanded (can be done using Unit Tests and mocks)

#### Certification Environment

There is a certification environment setup for this project, but currently it is missing connection access to many of dependencies and therefore doesn't run.

For the time being, deployment and integration tests will be disabled since the environment isn't setup, but it can be implemented at a later date.

##### Certification Deployment Pipeline YAML

```yml
  - template: deploy.yml
    parameters:
      stageName: deploy_cert
      dependsOn: deploy_test
      artifactName: drop
      bicepTemplateArtifactName: bicepTemplates
      isDeveloperBuild: true # DELETE THIS LINE BEFORE MERGE

  - template: run-integration-tests.yml
    parameters:
      deployedProjectServerPath: 'D:\Batch\Innovation\INN.JobRunner\INN.JobRunner.exe integration-tests'
      condition: succeeded('deploy_cert')
      environment: cert
      dependsOn: deploy_cert
```


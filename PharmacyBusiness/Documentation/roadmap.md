# Roadmap

## Goals

The project goal is to replace the existing Control-M job commands in the PharmacyBusiness project for the following reasons:

- **Culture of Currency**
  - **.NET Upgrade** - The current project is based in .NET 4.8 which can't be updated to the most recent version (.NET 8) of .NET directly requiring a complete rewrite of the project.
  - **Pipelines Update** - The project will build and deploy using Azure Pipelines replacing the current Azure DevOps build and release pipelines.
- **Replace Deprecated InterfaceEngine** - InterfaceEngine is the engine behind all of the current jobs and has been deprecated by EA for some time. Removing it requires a complete rewrite of the project.
- **Improve Maintainability** - The jobs in Control-M are currently setup as multi-step jobs that require deep domain knowledge to support. To simplify support, jobs will be designed so they can be restarted from the first step at any time without causing issues.
- **Self Documenting Errors** - Error logging will also be designed to output intuitive logs with follow up instructions so TCS will have prompts when supporting it.
- **Handle Dependencies** - The current jobs have a lot of dependencies where connections fail causing the entire job to fail. The new jobs will be designed to handle these dependencies gracefully and will retry when expected errors (network, etc) occur.
- **Better Security**
  - **Password Security** - Current jobs hold passwords into settings files as plain text. This project will use Azure Key Vault to store these passwords securely.
  - **Password Access** - Developers will ONLY have access to modify an existing password, not read them. This could be expanded to allow other teams up update the passwords for their systems separate from our team.
  - **No Sensitive Data in Files** - HIPPA data will be stored in memory in jobs as much as possible instead of being writted to the file system to be used by other jobs.
- **TenTen Rollups** - Instead of appending data to the TenTen tables, every day the jobs are run a table for just that day's data will be created, and then a main table will rollup all those tables into a single table with all days data.
- **Unit and Integration Testing** - The new jobs will be designed to be testable and will have unit and integration tests to ensure they work as expected. There will be an emphasis on unit testing the logic of the jobs, and using mocks to test the dependencies so they will not have to be tested against live data.
- **EA Approved Architecture** - Architectural structure has been signed off on by EA.

## Diagrams

### Current Flow

[![](https://mermaid.ink/img/pako:eNpFkM9qwzAMh1_F6Ny8QBiF1t5gI7mst9o9qLHSmvpPcGRGKX33pvPodJK-3wdCusGQLEELo08_wxkzi-7bRLHUXvfFs5s8CZki5-Sb_u2Y11_pKHZM03wQTbMWm2pv9EuqfFv5VsteiQ_nqWJZsdSfkSmPOJB4jycX_2JVY6VlmTkF8bKem6soVNcdqgYrCJQDOrtccHsyA3ymQAbapbWYLwZMvC8eFk67axyg5VxoBWWyyKQcnjIGaEf080InjPuU_meyjlPu64t-P3V_AEddYXw?type=png)](https://mermaid.live/edit#pako:eNpFkM9qwzAMh1_F6Ny8QBiF1t5gI7mst9o9qLHSmvpPcGRGKX33pvPodJK-3wdCusGQLEELo08_wxkzi-7bRLHUXvfFs5s8CZki5-Sb_u2Y11_pKHZM03wQTbMWm2pv9EuqfFv5VsteiQ_nqWJZsdSfkSmPOJB4jycX_2JVY6VlmTkF8bKem6soVNcdqgYrCJQDOrtccHsyA3ymQAbapbWYLwZMvC8eFk67axyg5VxoBWWyyKQcnjIGaEf080InjPuU_meyjlPu64t-P3V_AEddYXw)

- A single job (ex. Download data from McKesson to TenTen) would be implemented as several Control-M job steps
- Interface Engine is a deprecated Wegmans product that would take a DLL referencing specific instructions and run them

### New Flow

[![](https://mermaid.ink/img/pako:eNpFj81uwkAMhF9l5TPhASKEBOmFih8JbmU5mKxpVmTtyHUECPHu3XYP-GR_M7JmntBKIKjh0sut7VDNrfeeXZ7FsRE2lb7auE85z846v0Xr3I7JHYyGk6uquVsW8_K42m6n2bYfmUmndKeiN0Vv8rOUkEPVR6ZTgTCBRJowhhzg-cc8WEeJPNR5DahXD55f2YejyeHBLdSmI01gHAIafUT8VkxQX7D_yXRA_hJ53xSiiW5Kw_-ir18YI03u?type=png)](https://mermaid.live/edit#pako:eNpFj81uwkAMhF9l5TPhASKEBOmFih8JbmU5mKxpVmTtyHUECPHu3XYP-GR_M7JmntBKIKjh0sut7VDNrfeeXZ7FsRE2lb7auE85z846v0Xr3I7JHYyGk6uquVsW8_K42m6n2bYfmUmndKeiN0Vv8rOUkEPVR6ZTgTCBRJowhhzg-cc8WEeJPNR5DahXD55f2YejyeHBLdSmI01gHAIafUT8VkxQX7D_yXRA_hJ53xSiiW5Kw_-ir18YI03u)

- Control-M jobs will be implemented as one step that calls the `INN.JobRunner.exe` with a command line argument
- The command line argument will kick off the specific steps for that job (ex. Netezza Sales upload to TenTen)

## Phase 1 - Job Integrations

The first phase will involve a selection of jobs that contain different dependencies in an effort to get a good view of all the different types of dependencies that the existing jobs touch.

Focusing on a few sample jobs and the jobs currently running on Eddie's machine, they will be migrated to the news system, and integration libraries will be build for the existing dependencies as they are discovered.

We will also formalize a rollout plan for future jobs for TenTen during this phase

### Current Status

**In Progress**

### Known Dependencies

- TenTen
- McKesson DW
- McKesson CPS
- Netezza
- Cisco Informix

### Migrated Jobs

- INN501 "Net Sales ETL from internal team EDW Netezza database to 1010data" (old job is CRX650).
- INN502 "IVR Outbound Calls ETL from vendor Omnicell file to 1010data" (old job is RXS528).
- INN503 "Alternative Payments ETL from vendor McKesson Oracle DW database to 1010data" (old job is RXS612).
- INN504 "VeriFone Payments ETL from vendor McKesson Oracle DW database to 1010data" (old job is RXS623).
- INN505 "Mail Fill Fact ETL from vendor McKesson Oracle DW database to 1010data" (old job is RXS624).
- INN506 "Xarelto Subscription ETL from vendor McKesson Oracle DW database to 1010data" (old job is RXS650).
- INN507 "Call CSQ Agent ETL from internal team Cisco Informix database to 1010data" (old job is from Eddie).
- INN508 "Call Record Detail ETL from internal team Cisco Informix database to 1010data" (old job is from Eddie).
- INN509 "Call State Detail ETL from internal team Cisco Informix database to 1010data" (old job is from Eddie).
- INN510 "New Tag Patient Groups ETL from vendor McKesson Oracle DW database to 1010data" (old job is from Eddie).
- INN511 "Rx Erp ETL from vendor McKesson Oracle DW database to 1010data" (old job is from Eddie).
- INN512 "Sold Detail ETL from vendor McKesson Oracle DW database to 1010data" (old job is from Eddie).
- INN513 "Conversation Fact ETL from vendor McKesson SQL Server CPS database to 1010data" (old job is from Eddie).
- INN514 "Enrollment Fact ETL from vendor McKesson SQL Server CPS database to 1010data" (old job is from Eddie).
- INN515 "Measure Fact ETL from vendor McKesson SQL Server CPS database to 1010data" (old job is from Eddie).
- INN516 "Message Fact ETL from vendor McKesson SQL Server CPS database to 1010data" (old job is from Eddie).
- INN517 "PCS Pref ETL from vendor McKesson SQL Server CPS database to 1010data" (old job is from Eddie).
- INN518 "PCS Pref Full ETL from vendor McKesson SQL Server CPS database to 1010data" (old job is from Eddie).
- INN519 "Questionnaire Fact ETL from vendor McKesson SQL Server CPS database to 1010data" (old job is from Eddie).
- INN520 "Store Inventory History ETL from vendor McKesson Oracle DW database to 1010data"

## Phase 2 - SFTP Uploads

During this phase, we will take jobs that deal with either sending or receiving files via SFTP from and to different vendors and migrate them to the new system.

Additional dependencies that need libraries may pop up at this time, but during this phase, we will start to see more momentum when adding new jobs that rely on dependencies that have already been implemented.

### Current Status

**Not Started**

### Known Dependencies

- SFTP inbox
- SFTP outbox

... Fill in more dependencies as they are discovered

### Migrated Jobs

**To be filled in**

## Phase 3 - Remaining Jobs

This phase will be finding all remaining jobs, creating stories for each of them, and migrating them to the new system.

At this phase, all major dependencies should be implemented, so the major difficulty with this phase will be managing the more complex logic of some of the existing jobs. This complexity will be tested using mocks and unit tests.

### Current Status

**Not Started**

### Known Dependencies

Hopefully none, but these can be captured as we investigate.

### Migrated Jobs

**To be filled in**

### Updating support responsibilities

See the [TenTen Rollout Plan](tenten-rollout-plan.md#ongoing-support) for more information on how support responsibilities will be updated.
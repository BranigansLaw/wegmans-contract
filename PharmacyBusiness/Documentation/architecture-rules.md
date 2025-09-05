# Innovation Project Architectural Rules

## External dependency connection code (ex. connection setup to Snowflake) should be segmented into it's own classes as much as possible

- It should have it's own interface class (for mocking purposes)
- It should not do any business logic
- It should be general enough to run different queries based on parameters that are passed in

## Business Logic should be placed in the specific job's `IHelper` class

Steps where business logic (ex. transforming data returned from Snowflake) is done should be in that job's `IHelper` class

## `//TODO:` Comments

`//TODO:` comments are acceptable, but should include a link to a user story explaining the fix, and the user story should reference the line where the `TODO` comment is and what needs to be done.

## Unit Testing

All non-external dependency connection related code should be unit tested

## Integration Testing

All external dependency connection related code should be integration tested

## Story Completion

A user story should be considering "Completed" when the following conditions are met:

- The code written has unit tests covering new functionality (if applicable)
- The code written has integration tests covering new functionality (if applicable)
- The code is merged into `master`
- The code has been deployed into production
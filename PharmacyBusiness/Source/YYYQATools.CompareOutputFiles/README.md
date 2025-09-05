# YYYQATools.CompareOutputFiles

This is a tool to compare two output files and generate a report of the differences.

## Some thoughts about Quality Assurance when it comes to comparing output files

When migrating code from one system to another, it is important to compare the output files of the old system to the new system to ensure that the new system is producing the same results as the old system. 
This is especially important when migrating code from one database system to another, like from McKesson Oracle DW to Snowflake.

- You are encouraged to explore for yourself the many tools that can compare two files, like Notepad++, WinMerge, Beyond Compare, etc.
- You may also want to explore open source compare programs.
- You may also want to explore modifying sort order and formatting of the output files you are working on to make comparisons easier.
  For example, you could import your data files into Excel, then sort (perhaps by each and every column, if needed), then export to CSV, then compare those two CSV files.
- You may want to use this compare tool to add to your overall tool bag.

At the end of the day, each developer is responsible for comparing new to old files and getting end user sign-off on all differences between the two.
Additionally, each developer is responsible for letting end users know when the Go Live Date is planned for the new system and to include you in any incidents or concerns following the Go Live Date.

## Tips & Tricks

It is recommended to create both new and old files at the same time since databases like McKesson Oracle DW is not static and can change over time.


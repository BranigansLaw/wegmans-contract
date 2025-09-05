# XXXDeveloperTools.CompareDerivedData

This is a tool to compare two output files and generate a report of the differences.

While Notepad++ can also compare two files, it might fail under certain circumstances (like sort order of rows in the files).
See screen snippet of Notepad++ comparison here: "./DemoData/CompareResults/NotepadFileCompareOf_DEMO01_to_OLD001_ForRunDate_20240702.PNG".

There might also be slight differences between two files that can be tolerated. For example, Oracle and C# .Net both have rounding functions, but do not round the same way in every situation. So, we can set a tolerance threshhold configuration and track the scope of these occurences.

For these reasons, this tool to better assist developers while migrating code from one system to another.

## Tips & Tricks

It is recommended to create both new and old files at the same time since databases like McKesson Oracle DW is not static and can change over time.

## Demo

Set arguments for Program.cs to "DEMO01 07/02/2024 TRUE" to step through the code and see the results.

Also, carefully read through the report file to see the differences that relate specifically to code development of filters and business logic as the report offers some suggestions.

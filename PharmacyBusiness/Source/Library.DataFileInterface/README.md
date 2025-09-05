# Library Data File Interface

This project is used by other `Library` projects and contains shared interfaces for data files.

## Considerations when importing vendor data files.

When importing vendor data files, the following considerations should be taken into account:
(A) The data file may contain multiple records types.
    (i) The file may have a header row.
    (ii) Some vendors use a row delimiter other than new line feed.
(B) The data file may have a custom column delimiter.
(C) The data file may contain multiple data types.
(D) The data file may contain multiple data formats.
    (i) The file may contain dates in a non-standard format.
(E) Some vendors are known to have routine issues making a data file in a consistent manner.
    (i) The file may contain extra columns.
    (ii) The file may contain missing columns.
    (iii) The file may contain data in varying formats from line to line.
(F) The business team may, or may not, want to handle bad data differently depending on the situation.
    (i) The business team may want to process the good data and ignore the bad data.
    (ii) The business team may want to hault data processing until all data is good.
    (iii) The business team may want to to revise the overall manner in which data files are processed.

## Data File Interfaces in this library to address the above considerations.

(1) Vendor file data model classes can be customized to handle the various data types and formats within the data file.
(2) Data integrity issues are handled line by line rather than all or nothing.
(3) Notifications are sent to allow for the business team to decide how to handle bad data.
(4) The data file interface allows for the processing the data file in a consistent manner.
(5) The data file interface allows for the processing the data file in a scalable manner.


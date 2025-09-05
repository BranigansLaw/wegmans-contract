

create or replace TYPE SURESCRIPTS_OBJ AS OBJECT
(
     row_as_one_big_varchar2    VARCHAR2(4000 CHAR)
);

create or replace TYPE SURESCRIPTS_TABLE_TYPE AS TABLE OF SURESCRIPTS_OBJ;


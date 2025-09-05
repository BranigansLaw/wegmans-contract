create or replace FUNCTION         Clean_String_ForTenUp(p_Original_String IN VARCHAR2)
RETURN VARCHAR2
AS
    v_Special_Char_Found    VARCHAR2(4000 Char);
    v_Working_String        VARCHAR2(4000 Char);
    v_Working_String_Length NUMBER;
    v_Cleaned_String        VARCHAR2(4000 Char);
    v_Temp_Char             VARCHAR2(1 Char);
    v_Temp_AsciiNbr         NUMBER;
BEGIN  
    v_Cleaned_String := '';
    v_Special_Char_Found := '';
    v_Working_String := Trim(p_Original_String);
    v_Working_String_Length := Length(v_Working_String);

    IF (v_Working_String_Length >= 1) THEN
        FOR i IN 1..v_Working_String_Length LOOP
            v_Temp_Char := Substr(v_Working_String, i, 1);            
            v_Temp_AsciiNbr := ASCII(v_Temp_Char);

            --Include alphanumeric characters (32 through 126)
            --Exclude Double Quote (34) and Vertical line/Vertical bar (124) that would break executing TenUp from a file.
            IF (ASCII(v_Temp_Char) BETWEEN 32 AND 126 AND
                ASCII(v_Temp_Char) NOT IN (34,124)
               ) THEN 
                --Keep only alphanumeric characters.
                v_Cleaned_String := v_Cleaned_String || v_Temp_Char;
            ELSE
                --This is a special character.
                IF (Length(v_Special_Char_Found) > 0) THEN
                    v_Special_Char_Found := v_Special_Char_Found || ',';                
                END IF;
                v_Special_Char_Found := v_Special_Char_Found || To_Char(v_Temp_AsciiNbr);
            END IF;
        END LOOP;
    END IF;

    --DBMS_OUTPUT.PUT_LINE('v_Special_Char_Found=' || v_Special_Char_Found);
    --DBMS_OUTPUT.PUT_LINE('v_Cleaned_String=' || v_Cleaned_String);
    RETURN v_Cleaned_String;
END Clean_String_ForTenUp;
/

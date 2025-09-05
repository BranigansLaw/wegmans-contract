create or replace FUNCTION KEEP_ONLY_ALPHANUMERIC(
    p_String_Varchar2     IN    VARCHAR2
)
RETURN VARCHAR2
AS
    v_String_Varchar2     VARCHAR2(4000 Char) := '';
    v_Temp_Char           VARCHAR2(1 Char);
    v_Char_Length         NUMBER;
BEGIN  
    v_Char_Length := Length(p_String_Varchar2);

    IF (v_Char_Length >= 1) THEN
        FOR i IN 1..v_Char_Length LOOP
            v_Temp_Char := Substr(p_String_Varchar2, i, 1);
            IF (ASCII(v_Temp_Char) BETWEEN 32 AND 126) THEN
                --Keep only alphanumeric characters
                v_String_Varchar2 := v_String_Varchar2 || v_Temp_Char;
            END IF;
        END LOOP;
    END IF;
    
    RETURN v_String_Varchar2;
END;
/


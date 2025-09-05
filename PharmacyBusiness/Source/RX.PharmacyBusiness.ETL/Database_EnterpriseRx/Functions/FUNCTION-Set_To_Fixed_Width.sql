create or replace FUNCTION Set_To_Fixed_Width(
    p_String_Varchar2     IN    VARCHAR2,
    p_Char_Length         IN    NUMBER,
    p_Justify_Left_Right  IN    VARCHAR2
)
RETURN CHAR
AS
    REGEXP_ISNUMERIC       VARCHAR2(18 Char) := '^-?[[:digit:],.]+$';
    v_String_Varchar2      VARCHAR2(4000 Char) := '';
    v_Temp_Char            VARCHAR2(1 Char);
    v_Char_Length          NUMBER;
BEGIN  
    IF (Length(p_String_Varchar2) >= p_Char_Length) THEN
        v_Char_Length := p_Char_Length;
    ELSE
        v_Char_Length := Length(p_String_Varchar2);
    END IF;
    
    v_String_Varchar2 := Substr(wegmans.KEEP_ONLY_ALPHANUMERIC(p_String_Varchar2), 1, v_Char_Length);

    IF (p_Justify_Left_Right = 'R') THEN 
        IF (RegExp_Like(NVL(v_String_Varchar2,0), REGEXP_ISNUMERIC)) THEN
            IF (To_Number(NVL(v_String_Varchar2,0)) < 0) THEN 
                v_String_Varchar2 := '-' || LPad(To_Char(Abs(To_Number(NVL(v_String_Varchar2,0)))), (p_Char_Length - 1), '0');
            ELSE 
                v_String_Varchar2 := LPad(NVL(v_String_Varchar2,'0'), p_Char_Length, '0');
            END IF;
        ELSE 
            v_String_Varchar2 := LPad(NVL(v_String_Varchar2,'0'), p_Char_Length, '0');
        END IF;
    ELSE 
        v_String_Varchar2 := RPad(NVL(v_String_Varchar2,' '), p_Char_Length, ' ');
    END IF;
    
    RETURN v_String_Varchar2;
END;
/

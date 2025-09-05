SELECT
     atc.owner AS table_owner
    ,atc.table_name
    ,atc.column_name
    ,atc.column_id
    ,Max(Decode(data_type,
        'BLOB', 'BLOB',
        'CLOB', 'CLOB',
        'DATE', 'DATE',
        'LONG', 'LONG',
        'RAW', 'RAW',
        'T_NUMBER_TABLE', 'T_NUMBER_TABLE',
        'TIMESTAMP(6)', 'TIMESTAMP(6)',
        'CHAR', data_type || '(' || char_length || ' CHAR)',
        'VARCHAR2', data_type || '(' || char_length || ' CHAR)',
        'NVARCHAR2', data_type || '(' || char_length || ' CHAR)',
        'NUMBER', data_type || 
                  CASE WHEN data_precision IS NOT NULL AND
                            data_scale IS NOT NULL
                            THEN '(' || data_precision || ',' || data_scale || ')'
                       ELSE NULL
                  END,
        data_type)
      ) AS column_data_type
     ,Max(acc.comments) AS column_comments
     ,(SELECT Distinct 'Y'
       FROM all_cons_columns a
            INNER JOIN all_constraints ac
               ON ac.constraint_type = 'P'
              AND ac.owner = atc.owner
              AND ac.table_name = atc.table_name
              AND ac.constraint_name  = a.constraint_name 
       WHERE a.owner = atc.owner
         AND a.table_name = atc.table_name
         AND a.column_name = atc.column_name
      ) AS is_table_key
 FROM all_tab_columns atc
    , all_col_comments acc
WHERE atc.TABLE_NAME = acc.TABLE_NAME (+)
  AND atc.COLUMN_NAME = acc.COLUMN_NAME (+)
  AND atc.owner LIKE 'RX%' --Keep to make sure no system tables.
  AND atc.owner LIKE :LikeSchemaOwner
  AND atc.table_name LIKE :LikeTableName
  AND (atc.owner, atc.table_name) NOT IN (
          SELECT av.owner, av.view_name
          FROM all_views av
      )
GROUP BY atc.owner, atc.table_name, atc.column_id, atc.column_name
ORDER BY atc.owner, atc.table_name, atc.column_id
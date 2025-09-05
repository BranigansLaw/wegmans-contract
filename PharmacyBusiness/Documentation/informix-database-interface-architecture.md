# Informix Interface Architecture

This document outlines a proposed architecture for libraries that interface with the IBM Informix database.

## Goals

1. **Encapsulation**: The library should encapsulate what type of data source is used so that if the database is moved, the library can be changed without having to change the jobs that use it.
2. **Ease of Configuration**: The method called should know what configuration (ie. SQL file) is used just by the type of data or method that is called.
3. **Abstraction**: The method used to connect should be shared and as little code as possible should be duplicated.
4. **Ease of Unit Testing**: Class interfaces will be designed in such a way to allow for easy unit testing.

## Proposed Architecture

Methods describing the data being requested at their return types will be created in `IInformixInterface`.

All database specific code will be encapsulated in it's own interface `IInformixConnection`.

A custom `InformixMapper` will be implemented for each SQL file that will map the `DbDataReader` results to a C# class with named properties. The `IfxDataReader` inherits from `DbDataReader` and can be mocked for easy unit testing.

The distribution of responsibility will allow for easy unit testing. The `InformixInterfaceImp` tests will focus on making use the correct SQL files and parameters are passed to the correct interface methods (either a query or stored procedure), while the `InformixMapper` tests will focus on mapping `DbDataReader`s to the correct return types.

[![](https://mermaid.ink/img/pako:eNqNkttqAjEQhl9lyZWl7j7AUgRxpWxhvdCrlkAZk1kbmk3WHMAivntHV-uhajs3yUwy3_wk_5oJK5HlzOMyohFYKFg4aLhJKMrJJHux82k0Bl06GDyWpamta9SqNAFdDQLz5BnDcIEmzAIELDCA0kP_ZUTvYQ_51XNGGlmCi6CsyZMRaH2o9zgTlPotVe6o768UVVUUmV9qzvqJi6ag08s5R-Ju0L6aTTBkI-tIsWhkNl6hiAGnCBLdj9TLuwRIrypV9YpGQ9f-r_kVtC26PKH15KV6cn7kXKjoOm5KcBiiM4nQ4H1ygrynJr32gXtQOTaxQQdzjU8nuMGdX0zPDPInifUZlRtQkgy33nI5Cx_YIGc5bSW4T8642dA9iMHOyEQsDy5in8VWbjmdOQ_FFsybtZTWoD3lKFWwruoMvfP15huy7wP3?type=png)](https://mermaid.live/edit#pako:eNqNkttqAjEQhl9lyZWl7j7AUgRxpWxhvdCrlkAZk1kbmk3WHMAivntHV-uhajs3yUwy3_wk_5oJK5HlzOMyohFYKFg4aLhJKMrJJHux82k0Bl06GDyWpamta9SqNAFdDQLz5BnDcIEmzAIELDCA0kP_ZUTvYQ_51XNGGlmCi6CsyZMRaH2o9zgTlPotVe6o768UVVUUmV9qzvqJi6ag08s5R-Ju0L6aTTBkI-tIsWhkNl6hiAGnCBLdj9TLuwRIrypV9YpGQ9f-r_kVtC26PKH15KV6cn7kXKjoOm5KcBiiM4nQ4H1ygrynJr32gXtQOTaxQQdzjU8nuMGdX0zPDPInifUZlRtQkgy33nI5Cx_YIGc5bSW4T8642dA9iMHOyEQsDy5in8VWbjmdOQ_FFsybtZTWoD3lKFWwruoMvfP15huy7wP3)
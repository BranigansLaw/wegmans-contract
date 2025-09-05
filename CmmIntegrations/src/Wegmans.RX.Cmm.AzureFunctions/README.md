# CoverMyMeds Azure Functions

## Introduction

This project contains all of the Azure Function integrations we need to create for CoverMyMeds (Cmm)

## Local Development

If you are developing locally [Contributing.md](../../docs/Wegmans.RX.Cmm.AzureFunctions/Contributing.md) contains the settings you need as well as some guidance on naming and structure of the project

## CoverMyMeds Proxy to Astute

This set of functions were created to act as a proxy betweeen CMM and Astute. The functions were designed around the following DTA: [Cmm Astute Proxy DTA](../../docs/Wegmans.RX.Cmm.AzureFunctions/assets/Wegmans.%20CMM%20DTA%20Final%2010.5.20%20copy.pdf)

### System Design
  
![cmm astute proxy system design](../../docs/Wegmans.RX.Cmm.AzureFunctions/Assets/Cmm%20Astute%20Proxy%20-%20System.png)

### Sequence Diagrams
  
#### **Patient Enrollment Sequence**

![cmm astute proxy patient enrollment sequence](../../docs/Wegmans.RX.Cmm.AzureFunctions/assets/Cmm%20Astute%20Proxy%20-%20Enrollment%20Sequence.png)

#### **Case Status Sequence**

![cmm astute proxy case status sequence](../../docs/Wegmans.RX.Cmm.AzureFunctions/assets/Cmm%20Astute%20Proxy%20-%20Case%20Status%20Sequence.png)

#### **Patient Status Sequence**

![cmm astute proxy patient status sequence](../../docs/Wegmans.RX.Cmm.AzureFunctions/assets/Cmm%20Astute%20Proxy%20-%20Patient%20Status%20Sequence.png)
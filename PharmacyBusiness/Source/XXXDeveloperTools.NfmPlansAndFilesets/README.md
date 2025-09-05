# XXXDeveloperTools.NfmPlansAndFilesets

This is a tool to generate a report for IT Security regarding how files are moved around within NFM.

## Keeping the report up to date

You can find Production NFM data files (*.dat)* on TPSTEST here "/wegappl/techsvcs/data/".
Think of this Unix directory as like a folder off the C drive on a Windows box. 
The Tech Serv Unix team manages this Unix box as their Test environment, and in this folder is NFM data from their Production environment intended for easy access to all developers.

There is no download from Control-M but rather it is manually generated.
Fortunately, once this list is created it is easily updated by looking at CRQs in SmartIt since this list was last updated.

The first run of this program produces a txt file of GREP commands to run on the TPSPROD server to get the exact encryption and decryption information to supplement the main report.
Those results can be put into the project folder named "ProductionTpsprodLogs" to replace the older ones.

## About the reports generated

There is a CSV file of all NFM information merged into one file you can open in Excel and filter and sort as needed.

The other file gerenated is all the GREP searches you can run on the TPSPROD server to dig through the logs to get exact encryption and decryption information to supplement the main report.


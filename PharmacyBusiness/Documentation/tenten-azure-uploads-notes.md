# 1010 Azure Uploads

## Rollout Strategy

1. Identify a feed that we want to replace
2. Create the code that generates the records that you want to append and upload it to the Azure blob (prod folder)
3. Contact Steven Stine and CC Alex Casanova that files are available for upload for your feed in the new folder
4. TenTen will copy the current live feed to a new QA feed and start appending your data to it and verify the data is identical to the live feed
5. Once TenTen are confident, they will un-hide this QA feed and have the business look at it and verify it's authenticity
6. Once this is completed, on a cutover day agreed upon by the business, the new Parquet feed will become the live feed, and the old feed will be deleted

## File Naming

Parquet files that append data to a feed will be placed in the prod blob container in a folder that matches the feed name, and a file name that contains the feed name, the date that the data was collected for (usually the jobs run date), as well as the date and time that the file was created (EST).

Example file path: `340B_SALES_ADJUSTMENTS/340b_sales_adjustments_20240912_20240913_103000.parquet`

### Full Naming Conventions Document from 1010

https://wegmans.sharepoint.com/sites/DevStore&Rx/_layouts/OneNote.aspx?id=%2Fsites%2FDevStore%26Rx%2FSiteAssets%2FPharmacyandInovationTeamHub&wd=target%281010%20Azure.one%7C32D16BE9-D66F-4608-B0EC-CFB4FB8B2541%2FAzure%20Naming%20Conventions%7C33A91868-A9BA-4FF0-8838-1F7EB81EBD8F%2F%29

## Data Replacement

If a file is incorrectly loaded, you can simply reupload the generated file to Azure (with a different processing date) and the data will be replaced once it's picked up.

## Processing Issues

If there are issues with processing a Parquet file sent, 1010 will reach out to use with the issues so we can fix it and reupload the file

## Future QA

- We can use the DEV folder to upload parquet files to a separate visible table in TenTen for testing by request
- We could get a live copy of historical data on demand as well for QA, but having it copy as a daily process would be very expensive

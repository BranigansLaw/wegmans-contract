# Library.SnowflakeInterface

## Snowflake Access

### Developer Access

Access to Snowflake is granted from Azure Groups. To add users to this group, a person must be in the Snowflake Admin group.

To add a user:

1. Go to [this link](https://portal.azure.com/#view/Microsoft_AAD_IAM/GroupDetailsMenuBlade/~/Members/groupId/ccc00c38-1740-4f99-8619-87b972040486/menuId/) or login to Azure, go to Groups, search "SNOWFLAKE_PHARMACY_ENGINEERS" and click "Members"
2. Click "Add members"
3. Search and select the members you want to add and click "Update"

### Admin Access

Admin access is granted to Snowflake only to those developers who are in the Snowflake Admin group, but those members will need to PIM up to do administrative actions. To PIM up:

1. Login to Azure
2. Go to "Microsoft Entra Privileged Identity Management"
3. Click the "Activate" button under "Activate just in time"
4. Click "Groups" on the right
5. "SNOWFLAKE_ADMINISTRATORS" should appear as a group, and an "Activate" link on the right should allow you to activate it
6. Once activated, the privilege can take up to 40 minutes to propogate. To propogate it immediately, go to [this link](https://entra.microsoft.com/?feature.msaljs=true#view/Microsoft_AAD_Connect_Provisioning/UserProvisioningProvisionOnDemandBlade/objectId/a5468e31-790b-40cd-98c7-69e912376d8d/taskId/snowFlake.1318d57f757b45b3b1b09b3c3842774f.231f6f98-2123-4b1f-87cf-5606a42b579f/supportProvisionOnDemand~/true/isInboundApplication~/false) to go to Microsoft Entra.
7. Search for the SNOWFLAKE_ADMINISTRATORS group
8. From the dropdown, select your name
9. Click "Provision" to immediately provision
10. You may need to sign out and sign back into Snowflake to see the privileges change

## Environment Setups

### Check you have OpenSSL

Windows machines don't tend to come with OpenSSL by default. There are a few ways to access it:

- Using GitBash if you install Git for Windows
- **Untested** - Using `winget` command `winget install --id Git.Git -e --source winget`

### Setup Local Environment

Instructions derived from [here](https://docs.snowflake.com/en/user-guide/key-pair-auth#configuring-key-pair-authentication).

1. Wherever you can run OpenSSL, run the following command:
    ```bash
    openssl genrsa 2048 | openssl pkcs8 -topk8 -inform PEM -out rsa_key.p8 -nocrypt
    ```
2. Locate the `rsa_key.p8` file on your drive. Verify it looks something like this:
    ```x509
    -----BEGIN PRIVATE KEY-----
    MIIE6T...
    -----END PRIVATE KEY-----
    ```
3. Copy the contents of this private key file (removing carriage returns) and copy into the INN.JobRunner project's user secrets under the `secrets` settings as `SnowflakeUnencryptedPublicUserRsaToken`. It should look something like this:
    ```json
        {
          "secrets": {
            ...
            "SnowflakeUnencryptedPublicUserRsaToken": "-----BEGIN PRIVATE KEY-----MIIEwA...hQEItRtocE=-----END PRIVATE KEY-----"
            ...
          }
        }
    ```
4. Generate a public key from the private key:
    ```bash
    openssl rsa -in rsa_key.p8 -pubout -out rsa_key.pub
    ```
5. Locate the `rsa_key.pub` file on your drive. It should look something like this:
    ```x509
    -----BEGIN PUBLIC KEY-----
    MIIBIj...
    -----END PUBLIC KEY-----
    ```
6. Copy the contents of `rsa_key.pub` into notepad and remove the `-----BEGIN ...` and `-----END ...` lines and carriage returns to turn the contents into a single line.
7. Open Snowflake and open a new SQL command
8. Run the following `ALTER USER` command that assigns this public key to your Snowflake user:
    ```sql
    ALTER USER <your_username> SET RSA_PUBLIC_KEY='<contents of modified notepad file from step 6>';
    ```
9.  Double check that you have deleted `rsa_key.p8` and `rsa_key.pub` from your machine before checking in. These files should not be made public as they will give bad actors access to make changes and run queries under your Snowflake account.
10. Try running just the Snowflake connection integration test from your local machine to test everything is working correctly.

### Setup TST/Production Environment

The user and private key should be stored in Azure Key Vault, with the public user key assigned to the app user on Snowflake.

*TODO*

## Migrating Oracle to Snowflake

These are the suggested steps for testing your Snowflake job (INN) has the same results as the Oracle job (CRX, RXS, etc):

1. Download the archived data files for the job you're migrating (ex. CRX515) from the production archive folder **ADD PATH HERE**. Note the run dates of the archived file so you can run the Snowflake query on the same run date.
2. Run your new command to match the run date(s) of the file(s) you've downloaded
3. Compare the results of each with one of the following methods
    - Excel
    - WinMerge
4. If there are small differences in data fields (decimal places or whitespace) that can't be rectified, get approval from the business to check it's OK
# TenTen New Table Rollout Plan

## Goals

1. When the new tables are created, there should be an easy way to switch all existing queries to point to the new table
2. If there are issues with the new table, it should be easy to switch back to the old table
3. Going forward, the Phramacy dev team should not be responsible for handling requests regarding errors with queries that point to our tables, unless there is a specific issue with the table
4. The Dev Pharmacy team should only be responsible for adding new data fields to existing tables (not modifying or deleting since this could break existing queries)
5. The business is affected minimally by the rollout

## Plan

### Rollout Plan v2 - Live Replacement

Due to the business requirement to replace the active table where it sits today, the table will be replaced by the new batch methods live.

#### Accepted Risks

With in place replacements of tables, there is a risk that some data may be lost on the days that it is not running.

#### Pre-Rollout Certification

Notes about matching old and new tables

#### Rollout Plan

1. Schedule a time with the business when the rollout will occur
   - Schedule with Chris Gardiner and Mike Rosilio and they will communicate with the business about the rollout
2. Take a backup of the current live table in TenTen
3. Create a PR that contains:
   - A change to the new job to overwrite the live table
   - A change to the old job that has it do nothing
4. At the scheduled time, merge the PR *this will kick off both the deployment of the old job and new jobs changes simultaneously*
5. When the old pipeline job completes, create a release for it so the change is deployed
6. Mark the date the old job was deactivated
7. Monitor the tables updates for the warranty period of 14 days

#### Rollback Plan

1. Create a PR that reactives the old job, and has the new job rollup to it's previous place in production
2. Merge and deploy the PR
3. Once deployed, rerun the old job for all the days that it was inactive for (use the runAs argument)

#### After Warranty Period

If there are issues after the warranty period, only the new job will be repaired to fix the issue. The old job will not be reactivated after this time.

### Rollout Plan v1 - Business Rejected

**This plan was rejected by the business due to the inability to replace the referenced table on Excel exports.**

The rollout plan will involce creating `library` queries that point to the old table in TenTen, using a query to find all library queries that reference the old table, in those libraries, changing the `<base table="..."` with a `<insert block="..."` pointing to the new pointer (which, at this time, will still point to the old table). Once all the libraries are updated, the pointer will be changed to point to the new table.

Current state:

[![](https://mermaid.ink/img/pako:eNpdkclqw0AMhl_F6NRCYrCdkw-F0BzbQJdTOyEoHjk2mY1ZakLIu3ccm9aOLjP_h8Sv5QKV5gQl1EJ3VYPWJy_vTCUxXDgcLZomeQtkW3ID7WOdPfTsnD1OWD6wfMqKgRVTthrYdmSk-J3dJx7E1O05--7oKFG5dHw5ekx5VxNxlyryDmPBblKR_1Vw-jFxKpkaq3mfu78l760WIpjdXQ_rLFkun6LjKPO5LOZyNZOwAElWYsvjNi89Y-AbksSgjF-O9sSAqWvMw-D1x1lVUHobaAHBxIlo02JcgISyRuEiNai-tP7XxFuv7etwrtvVrr9i2IZ_?type=png)](https://mermaid.live/edit#pako:eNpdkclqw0AMhl_F6NRCYrCdkw-F0BzbQJdTOyEoHjk2mY1ZakLIu3ccm9aOLjP_h8Sv5QKV5gQl1EJ3VYPWJy_vTCUxXDgcLZomeQtkW3ID7WOdPfTsnD1OWD6wfMqKgRVTthrYdmSk-J3dJx7E1O05--7oKFG5dHw5ekx5VxNxlyryDmPBblKR_1Vw-jFxKpkaq3mfu78l760WIpjdXQ_rLFkun6LjKPO5LOZyNZOwAElWYsvjNi89Y-AbksSgjF-O9sSAqWvMw-D1x1lVUHobaAHBxIlo02JcgISyRuEiNai-tP7XxFuv7etwrtvVrr9i2IZ_)

For existing TenTen table X:
1. Create a new library PointerX that selects all data from old table X.
2. Run a query in TenTen to find a list of all queries currently referencing the old table X
3. Modify all existing queries from a `<base table="..."` to a `<insert block="..."`
   1. Run the query without changes
   2. Update to use the `insert block`
   3. Run again to make sure old and new data looks the same
[![](https://mermaid.ink/img/pako:eNp1kstqwzAQRX_FaJVCYrCdlReFJF22pa9VaxMm1jg2tSSjR00I-ffKkWjkkGoj3cPM1Yw0R1IJiiQndSeGqgGpo8e3gkd2KbPbS-ib6NWgbFE5Oq5VMhvZIbkLWOpYGrLMsSxkS8eePUNOr657ES3XKFV0SVons68B9wy4iin-9LZOFvc-Luaotwo6VFuPyn-9P2DXhZ1skj9fv1PQENOhRqRn57NxGWSkNyqRggZVSNF1pi-valgl0WJxb1vxMp3KbCqXE7l2uRsvyZwwlAxaaj_uOLKC6AYZFiS3RwryuyAFP9k4MFq8H3hFci0NzonpbYP40IJ9D0byGjplaQ_8U4iLRtpqIZ_cZJwH5PQLkgKo6g?type=png)](https://mermaid.live/edit#pako:eNp1kstqwzAQRX_FaJVCYrCdlReFJF22pa9VaxMm1jg2tSSjR00I-ffKkWjkkGoj3cPM1Yw0R1IJiiQndSeGqgGpo8e3gkd2KbPbS-ib6NWgbFE5Oq5VMhvZIbkLWOpYGrLMsSxkS8eePUNOr657ES3XKFV0SVons68B9wy4iin-9LZOFvc-Luaotwo6VFuPyn-9P2DXhZ1skj9fv1PQENOhRqRn57NxGWSkNyqRggZVSNF1pi-valgl0WJxb1vxMp3KbCqXE7l2uRsvyZwwlAxaaj_uOLKC6AYZFiS3RwryuyAFP9k4MFq8H3hFci0NzonpbYP40IJ9D0byGjplaQ_8U4iLRtpqIZ_cZJwH5PQLkgKo6g)
4. When a new data feed is ready, deploy it into DWFeeds.Production.X and let it run in parallel with the old Control-M job still updating X
5. Compare the data in X and DWFeeds.Production.X checking the data looks the same
6. With the business, schedule a time when PointerX will change to point to DWFeeds.Production.X
7. If issues occur, see [Rollback Plan](#rollback-plan).
8. If no issues occur, point the PointerX to DWFeeds.Production.X
[![](https://mermaid.ink/img/pako:eNp1kstqwzAQRX_FaJVCIrCdlReFJF22pa9VaxMm1jg2tR7oURNC_r1ybBo5pNpI9zBzNSPNkZSSIclI1cqurEHb6PEtF5Ffxu32GlQdvTrUDZqB9msVz3p2iO8ClgwsCVk6sDRky4E9jwwFu7ruRTbCojbRJWkdz7463HMQhjL8Ub5OTtUYRwXarYEWzXZExb_eH7Brw0428Z_vuDOwQFlXIbKz89m4CDKSG5VoyYIqtGxbp4qrGlZxtFjc-1ZGmUxlOpXLiVz7XEo92ASgl8kgyZxw1Bwa5n_y2LOc2Bo55iTzRwb6Oye5OPk4cFa-H0RJMqsdzolTvmN8aMA_ECdZBa3xVIH4lPKikTVW6qdhVM4Tc_oF2besUA?type=png)](https://mermaid.live/edit#pako:eNp1kstqwzAQRX_FaJVCIrCdlReFJF22pa9VaxMm1jg2tR7oURNC_r1ybBo5pNpI9zBzNSPNkZSSIclI1cqurEHb6PEtF5Ffxu32GlQdvTrUDZqB9msVz3p2iO8ClgwsCVk6sDRky4E9jwwFu7ruRTbCojbRJWkdz7463HMQhjL8Ub5OTtUYRwXarYEWzXZExb_eH7Brw0428Z_vuDOwQFlXIbKz89m4CDKSG5VoyYIqtGxbp4qrGlZxtFjc-1ZGmUxlOpXLiVz7XEo92ASgl8kgyZxw1Bwa5n_y2LOc2Bo55iTzRwb6Oye5OPk4cFa-H0RJMqsdzolTvmN8aMA_ECdZBa3xVIH4lPKikTVW6qdhVM4Tc_oF2besUA)

### Rollback Plan

1. If there are issues with DWFeeds.Production.X, change PointerX to point back to X
2. Investigate the issues with DWFeeds.Production.X
3. Repeat rollout plan when ready

#### Warranty Period

1. After a period of 2 weeks, if there are no issues with DWFeeds.Production.X, the old table X will be dropped, and the existing Control-M job will be disabled

### Code Samples

#### TenTen Pointers Setup Example

```xml
<library>
    <block name="net_sales_pointer">
        <note>Dev Pharm team can switch between old and new versions of this table.</note>
        <!--     <base table="wegmans.wegmansdata.dwfeeds.netsales"/> -->
        <base table="wegmans.devpharm.prod.net_sales_rollup"/>
    </block>
</library>
```

#### TenTen Pointers Reference Example

```xml
<import library="wegmans_pointers">
<insert block="net_sales_pointer"/>
<sel ...>
```

### Ongoing support

Currently, the Dev Pharmacy team (specifically Chris McCarthy) field a lot of TenTen questions about queries and creating libraries. Going forward, the goal is to have the Dev Pharmacy team exclusively support the data that is used in the queries, not queries themselves

#### TenTen (Steven Stine and TenTen Support)

1. Optimizing Queries
2. First tier support for issues with TenTen queries

#### Dev Pharmacy Team Responsibilities

1. Adding new tables to TenTen
2. Adding new columns to existing tables
3. Modified columns will be added as new columns where requested to prevent breaking existing queries
4. Issues with queries directly caused by the data table (as determined by TenTen support)

## Alternate Considerations

### All Queries in Source Control

All queries currently in TenTen would be moved to source control. Current TenTen users would be given access to a version of their queries and libraries that they can test changes to the queries on, but would be responsible for merging their changes into source control. There would be an automated process that would then "deploy" these changes to TenTen, and overwrite any changes made manually in TenTen by users.

#### Pros

1. Dev Pharmacy Team has full control and view into changes in the TenTen system
2. Makes TenTen more of an enterprise system
3. Gives and easy view into what libraries exist in TenTen (ex. search for which queries impact which tables)

#### Cons

1. Requires the business to understand software development practices and source control
2. Dev Pharmacy now "owns" TenTen queries and will be asked to support changes and issues with them

### Replace Existing Tables

The old tables would be backed up and the old Control-M commands would update those backup tables, and our system would write to the live table used by queries. In the event of a rollback, we would copy the backup table into the live table and change the old Control-M command to continue updating the live table.

#### Pros

1. Business is uninvolved in the rollout process
2. Dev Pharmacy team stays uninvolved in maintaining the user created TenTen queries

#### Cons

1. Rollback requires copying data in TenTen
2. Rollout requires coordination between TenTen copy commands and source control deployment

### Table Pointers

Create a new query (pointer table) that returns the columns and rows of new current live table the old job is updating and update all existing queries to query this query table. During rollout, change the pointer table to point to the new table created with the new system. On rollback, change the pointer table to point to the old table.

#### Pros

1. Business is minimally involved in changing their queries
2. Rollback is very easy
3. Old Control-M jobs and new jobs don't have to be changed what they are pointing to
4. Pharmacy Dev team stays uninvolved in maintaining the user created TenTen queries
5. In the future as we make changes to the current jobs, we can reuse the rollout plan

#### Cons

1. Intensive changes to existing customer
2. Might not be possible with all TenTen queries

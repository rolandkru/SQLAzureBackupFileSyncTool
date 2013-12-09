SQLAzureBackupFileSyncTool
==========================

A small tool to sync sql azure backup files to local file system

Intention
========
May SQL Azure backup strategies result in backup-files on windows azure blob storage. Having the backup in the cloud is not always preferable. Some customers may need to have them in the on-premise backup infrastructure, because of legal or other reasons.

There are many tools to up-sync files from local file system to azure blob storage but not a single useful one to down-sync files from blobstorage to local file system. So I decided to write this small tool. It downloads new files from blobstorage. Furthermore it deletes files from local file system if they are not in blob storage anymore (due to retention policy).


Usage
=====

Step 1:
Configure SQL Azure Backup by using one of the strategies described on:
http://msdn.microsoft.com/en-us/library/windowsazure/jj650016.aspx

Step 2:
The result of your sql azure backup strategy should be a containter on azure blob storage where the backup files are stored. To download them on a scheduled, regular basis fork this repository and build it.
Copy the resulting binary files and the config file to your backup server. 

Step 3:
There is SqlAzureBackupFileSyncTool.exe.config file with 4 settings:
AccountName: Enter the blob storage account name
AccountKey: Enter the blob storage account private key
Container Name: Enter the name of the container (for example "sql-backups")
LocalDirectory: Enter the path to the local backup directory (for example: "o:/sqlbackups")

Step 4:
Set up a windows scheduler task that executes the SqlAzureBackupFileSyncTool.exe (time it according to your SQL Azure backup schedule).





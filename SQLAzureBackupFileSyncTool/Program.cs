// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Selectron Systems AG">
//   Copyright (c) Selectron Selectron Systems AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SQLAzureBackupFileSyncTool
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using log4net;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// This program synchronizes backup files from blob storage to the local file system.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger("root");

            try
            {
                logger.Info("Sync Started.");
                logger.InfoFormat("Account: {0}", ConfigurationManager.AppSettings["AccountName"]);
                logger.InfoFormat("Container: {0}", ConfigurationManager.AppSettings["ContainerName"]);
                logger.InfoFormat("Local Drive: {0}", ConfigurationManager.AppSettings["LocalDirectory"]);

                var storageCredentials = new StorageCredentials(ConfigurationManager.AppSettings["AccountName"], ConfigurationManager.AppSettings["AccountKey"]);

                var storageAccount = new CloudStorageAccount(storageCredentials, false);
                var blobClient = storageAccount.CreateCloudBlobClient();

                var blobContainer = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);
                var localDirectory = new DirectoryInfo(ConfigurationManager.AppSettings["LocalDirectory"]);

                var blobList = blobContainer.ListBlobs(null, true).ToList();
                var localList = localDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories).ToList();

                foreach (var localFile in localList)
                {
                    bool delete = true;

                    foreach (var blob in blobList.Select(b => b as ICloudBlob))
                    {
                        if (blob.Name.Equals(localFile.Name, StringComparison.Ordinal))
                        {
                            delete = false;
                            break;
                        }
                    }

                    if (delete)
                    {
                        logger.InfoFormat("Delete Local File: {0}", localFile.Name);
                        localFile.Delete();
                    }
                }

                foreach (var blob in blobList.Select(b => b as ICloudBlob))
                {
                    bool download = true;

                    foreach (var localFile in localList)
                    {
                        if (blob.Name.Equals(localFile.Name, StringComparison.Ordinal))
                        {
                            download = false;
                            break;
                        }
                    }

                    if (download)
                    {
                        logger.InfoFormat("Download Blob: {0}", blob.Name);
                        blob.DownloadToFile(Path.Combine(localDirectory.FullName, blob.Name), FileMode.Create);
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp);
                Console.WriteLine(exp);
            }
            finally
            {
                logger.Info("Sync Finished.");
            }
        }
    }
}
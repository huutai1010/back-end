using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Common.AppConfiguration;
using Common.Interfaces;
using Common.Models.RabbitMq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureStorageService(IConfiguration configuration)
        {
            var azureSettings = configuration.GetSection("AzureStorageSettings").Get<AzureStorageSettings>();
            if (azureSettings.ConnectionString == null)
            {
                throw new Exception("connection string is empty!");
            }
            string connectionString = azureSettings.ConnectionString;

            // Create a BlobServiceClient object 
            var blobServiceClient = new BlobServiceClient(connectionString);
            _blobServiceClient = blobServiceClient;
        }

        public bool ContainerNameIsExist(string containerName)
        {
            bool check;
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            Response<bool> containerExists = containerClient.Exists();

            if (containerExists.Value)
            {
                check = true;
            }
            else
            {
                check = false;
            }
            return check;
        }

        public bool DeleteContainer(string containerName)
        {
            bool check;
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            Response<bool> containerExists = containerClient.Exists();

            if (containerExists.Value)
            {
                containerClient.Delete();
                check = true;
            }
            else
            {
                check = false;
            }
            return check;
        }

        public async Task<List<FileMessageModel>> UploadFileToContainer(List<IFormFile> voiceFiles)
        {
            List<FileMessageModel> fileMessageModels = new();
            string containerName = "originalmp3file";
            try
            {
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                if (!await containerClient.ExistsAsync())
                {
                    // Create the container
                    await containerClient.CreateAsync();
                }

                foreach (var voiceFile in voiceFiles)
                {
                    // Generate a unique name for the blob
                    var blobName = voiceFile.FileName;

                    // Get a reference to a blob
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    blobClient.DeleteIfExistsAsync().Wait();

                    // Upload the file to the blob
                    using (var stream = voiceFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    FileMessageModel fileMessage = new();
                    fileMessage.FileName = voiceFile.FileName;
                    fileMessage.BlobName = blobName;

                    fileMessageModels.Add(fileMessage);
                }

                return fileMessageModels;
            }
            catch (Exception ex)
            {
                // Get a reference to the blob container
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                foreach(var item in fileMessageModels)
                {
                    // Get a reference to the blob
                    var blobClient = containerClient.GetBlobClient(item.BlobName);

                    // Delete the blob
                    await blobClient.DeleteIfExistsAsync();
                }

                return null;
            }
        }

        public async Task<bool> DeleteBlob(string containerName, string blobName)
        {
            try
            {
                // Get a reference to the blob container
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                // Get a reference to the blob
                var blobClient = containerClient.GetBlobClient(blobName);

                // Delete the blob
                await blobClient.DeleteIfExistsAsync();

                Console.WriteLine($"Blob {blobName} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting blob: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LeaseContainer(string containerName)
        {
            // Metadata key to represent lease status
            string leaseMetadataKey = "LeaseStatus";
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            // Try to lease the container
            if (await TryAcquireLeaseAsync(blobContainerClient, leaseMetadataKey))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Container is already leased by another process.");
                return false;
            }
        }
        static async Task<bool> TryAcquireLeaseAsync(BlobContainerClient containerClient, string leaseMetadataKey)
        {
            var leaseStatus = await GetLeaseStatusAsync(containerClient, leaseMetadataKey);
            // Try to lease if the container is not leased
            if (leaseStatus == null || leaseStatus.ToLower() == "unleased")
            {
                // Set metadata to indicate the container is leased
                var metadata = new Dictionary<string, string>
            {
                { leaseMetadataKey, "Leased" }
            };
                await containerClient.SetMetadataAsync(metadata);
                return true;
            }
            return false;
        }

        static async Task<string> GetLeaseStatusAsync(BlobContainerClient containerClient, string leaseMetadataKey)
        {
            // Retrieve container metadata
            var properties = await containerClient.GetPropertiesAsync();
            // Check if the lease status metadata exists
            if (properties.Value.Metadata.TryGetValue(leaseMetadataKey, out var leaseStatus))
            {
                return leaseStatus;
            }
            return null;
        }

        public async Task<bool> IsContainerLease(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            // Retrieve container metadata
            var properties = await containerClient.GetPropertiesAsync();
            // Check if the lease status metadata exists
            if (properties.Value.Metadata.TryGetValue("LeaseStatus", out var leaseStatus))
            {
                if (leaseStatus == "Leased")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> CreateLeaseContainer(List<string> containerNames)
        {
            List<string> newContainer = new();
            List<string> modifyContainer = new();
            try
            {
                foreach (var containerName in containerNames)
                {

                    // Create a new container client
                    var voiceContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                    if (await voiceContainerClient.ExistsAsync())
                    {
                        var check = await IsContainerLease(containerName);
                        if (check)
                        {
                            throw new Exception($"Container {containerName} is lease on process!");
                        }
                    }
                    else
                    {
                        // Create the container
                        await voiceContainerClient.CreateAsync();
                        newContainer.Add(containerName);
                    }

                    //lease the container
                    await LeaseContainer(containerName);
                    modifyContainer.Add(containerName);
                }

            }
            catch (Exception ex)
            {
                foreach (var container in modifyContainer)
                {
                    await ReleaseLeaseAsync(container);
                }
                foreach (var item in newContainer)
                {
                    DeleteContainer(item);
                }
                throw new Exception(ex.Message);
            }
            return true;
        }

        public async Task<bool> CreateLeaseContainer(string containerName)
        {
            bool isNewContainer = false;
            try
            {
                // Create a new container client
                var voiceContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                if (await voiceContainerClient.ExistsAsync())
                {
                    var check = await IsContainerLease(containerName);
                    if (check)
                    {
                        throw new Exception($"Container {containerName} is lease on process!");
                    }
                }
                else
                {
                    // Create the container
                    await voiceContainerClient.CreateAsync();
                    isNewContainer = true;
                }

                //lease the container
                await LeaseContainer(containerName);

            }
            catch (Exception ex)
            {
                if (isNewContainer)
                {
                    DeleteContainer(containerName);
                }
                else
                {
                    await ReleaseLeaseAsync(containerName);

                }

                throw new Exception(ex.Message);
            }
            return true;
        }

        public async Task DeleteAllBlobsAsync(string containerName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                await foreach (var blobItem in containerClient.GetBlobsByHierarchyAsync())
                {
                    await containerClient.DeleteBlobIfExistsAsync(blobItem.Blob.Name);
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task ReleaseLeaseAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            // Reset metadata to indicate the container is unleased
            var metadata = new Dictionary<string, string>
        {
            { "LeaseStatus", "Unleased" }
        };

            await containerClient.SetMetadataAsync(metadata);
        }

    }
}

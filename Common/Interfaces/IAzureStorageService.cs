using Azure.Storage.Blobs;
using Common.Models.RabbitMq;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAzureStorageService
    {
        public bool ContainerNameIsExist(string containerName);
        public bool DeleteContainer(string containerName);
        public Task<List<FileMessageModel>> UploadFileToContainer(List<IFormFile> voiceFiles);
        public Task<bool> CreateLeaseContainer(List<string> containerNames);
        public Task<bool> IsContainerLease(string containerName);
        public Task DeleteAllBlobsAsync(string containerName);
        public Task ReleaseLeaseAsync(string containerName);
    }
}

using Common.AppConfiguration;
using Common.Interfaces;
using Common.Models;
using Firebase.Auth;
using Firebase.Storage;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Extensions.Msal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly FirebaseStorageSettings _settings;
        private readonly FirebaseAuthProvider _authProvider;
        private readonly StorageClient _storage;

        public FirebaseStorageService(IOptions<FirebaseStorageSettings> settings, IConfiguration configuration)
        {
            _settings = settings.Value;
            _authProvider = new FirebaseAuthProvider(new FirebaseConfig(_settings.API_KEY));
            var firebaseCreds = JsonConvert.SerializeObject(configuration.GetSection("FirebaseSettings").Get<FirebaseSettings>());
            GoogleCredential credential = GoogleCredential.FromJson(firebaseCreds);
            _storage = StorageClient.Create(credential);
        }

        public async Task<List<ResponseFileImageDto>> UploadImageList(List<IFormFile> files, string imagePath)
        {
            string link = "";
            var a = await _authProvider.SignInWithEmailAndPasswordAsync(_settings.AuthEmail, _settings.AuthPassword);
            var cancellation = new CancellationTokenSource();

            List<ResponseFileImageDto> imageListDto = new();
            foreach (var file in files)
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;
                ResponseFileImageDto imageDto = new();

                var task = new FirebaseStorage(_settings.Bucket, new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true,
                })
                .Child(imagePath)
                .Child(file.FileName)
                .PutAsync(stream, cancellation.Token);

                try
                {
                    imageDto.FileName = file.FileName;
                    imageDto.FileLink = await task;
                    imageListDto.Add(imageDto);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return imageListDto;
        }

        public async Task<string> UploadImageFirebase(MemoryStream stream, string path ,string fileName)
        {
            string link = "";
            var a = await _authProvider.SignInWithEmailAndPasswordAsync(_settings.AuthEmail, _settings.AuthPassword);

            var cancellation = new CancellationTokenSource();
            string uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var task = new FirebaseStorage(_settings.Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true,
            })
                .Child(path)
                .Child(uniqueFileName)
                .PutAsync(stream, cancellation.Token);

            try
            {
                link = await task;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return link;
        }

        public async Task<bool> DeleteImageFirebase(string path, string fileName)
        {
            var a = await _authProvider.SignInWithEmailAndPasswordAsync(_settings.AuthEmail, _settings.AuthPassword);
            
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(_settings.Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true,
            })
                .Child(path)
                .Child(fileName)
                .DeleteAsync();

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ImageIsExist(string filePath)
        {
            bool fileExists = await DoesFileExist(_storage, _settings.Bucket, filePath);
            if (fileExists)
            {
                return true;
            }
            return false;
        }

        static async Task<bool> DoesFileExist(StorageClient storage, string bucketName, string fileName)
        {
            try
            {
                var storageObject = await storage.GetObjectAsync(bucketName, fileName);
                return storageObject != null;
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                // The file does not exist
                return false;
            }
        }

        public async Task<bool> DeleteImageExist(string fileName)
        {
            try
            {
                await _storage.DeleteObjectAsync(_settings.Bucket, fileName);
                return true;
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                // The file does not exist
                return false;
            }
        }
    }
}

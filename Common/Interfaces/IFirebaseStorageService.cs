using Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadImageFirebase(MemoryStream stream,string placeName ,string fileName);
        Task<bool> DeleteImageFirebase(string path, string fileName);
        Task<bool> ImageIsExist(string filePath);
        Task<bool> DeleteImageExist(string fileName);
        Task<List<ResponseFileImageDto>> UploadImageList(List<IFormFile> files, string imagePath);
    }
}

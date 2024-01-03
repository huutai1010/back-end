using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Interfaces;
using Common.Models;
using Common.Services;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;

namespace API.Controllers.Portal
{
    [Authorize(Roles = "1,2")]
    [Route("api/portal/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IMailService _mailService;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly IRabbitmqService _rabbitmqService;
        private readonly IPlaceService _placeService;

        public AssetsController(IAzureStorageService azureStorageService, IFirebaseStorageService firebaseStorageService, IMailService mailService, IRabbitmqService rabbitmqService, IPlaceService placeService)
        {
            _azureStorageService = azureStorageService;
            _firebaseStorageService = firebaseStorageService;
            _rabbitmqService = rabbitmqService;
            _mailService = mailService;
            _placeService = placeService;
        }

        [HttpPost("convert/mp3")]
        [SwaggerOperation(Summary = "[Operator] Convert mp3 to m3u8 for create place")]
        public async Task<IActionResult> ConvertMp3ToM3u8(List<IFormFile> listMp3)
        {
            var response = await _placeService.ConvertMp3ToM3u8V2(listMp3);
            return Ok(response);
        }

        [HttpGet("fileName")]
        [SwaggerOperation(Summary = "[Operator] Check voice file is exist!")]
        public IActionResult ContainerIsExist([FromQuery] string fileName)
        {
            string pattern = "^[a-z0-9]+$";
            var isValid = Regex.IsMatch(fileName, pattern);
            if(!isValid)
            {
                throw new BadRequestException($"{fileName} is invalid!");
            }

            var check = _azureStorageService.ContainerNameIsExist(fileName);
            if (check)
            {
                throw new BadRequestException($"{fileName} is exist!");
            }
            var message = $"{fileName} is not exist!";
            var response = new FileStatusResponse(message);
            return Ok(response);
        }

        [HttpDelete("fileName")]
        [SwaggerOperation(Summary = "[Operator] delete voice file exist!")]
        public IActionResult DeleteContainer([FromQuery] List<string> fileNames)
        {
            string pattern = "^[a-z0-9]+$";

            foreach (var fileName in fileNames)
            {
                var isValid = Regex.IsMatch(fileName, pattern);
                if (!isValid)
                {
                    throw new BadRequestException($"{fileName} is invalid!");
                }

                var check = _azureStorageService.DeleteContainer(fileName);
                if (!check)
                {
                    throw new BadRequestException($"{fileName} is not exist!");
                }
            }

            var response = new FileStatusResponse("Voice file delete successfully!");
            return Ok(response);
        }

        [HttpPost("image")]
        [SwaggerOperation(Summary = "[Operator, Admin] upload image to firebase")]
        public async Task<IActionResult> UploadImage(string imagePath, List<IFormFile> file)
        {
            var imageListDto = await _firebaseStorageService.UploadImageList(file, imagePath);
            var response = new UploadResponse(imageListDto);
            return Ok(response);
        }

        [HttpDelete("image")]
        [SwaggerOperation(Summary = "[Operator, Admin] delete image to firebase")]
        public async Task<IActionResult> DeleteImage([FromQuery] string imagePath, [FromQuery] List<string> imageNames)
        {
            foreach (var imageName in imageNames)
            {
                var check = await _firebaseStorageService.DeleteImageExist($"{imagePath}/{imageName}");
                if (!check)
                {
                    continue;
                }
            }
            return Ok("Delete Successfully!");
        }        

        [HttpGet("image/exist")]
        [SwaggerOperation(Summary = "[Operator, Admin] check image is exist")]
        public async Task<IActionResult> ImageExist(string imagePath)
        {
            var check = await _firebaseStorageService.ImageIsExist(imagePath);
            if (check)
            {
                throw new BadRequestException("image is exist");
            }
            return Ok("Image is not exist!");
        }

        [HttpGet("sendEmail")]
        public IActionResult SendEmail(string email)
        {
            MailData mail = new MailData {
                EmailBody = "test mail body",
                EmailSubject = "hello admin",
                EmailToId = email,
                EmailToName = "Quan Le"
            };

            var check = _mailService.CreateTestMessage3();
            if (!check)
            {
                return BadRequest("send false");
            }

            return Ok("send sucessfully!");
        }
    }
}

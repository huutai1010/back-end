using AgoraIO.Media;

using Common.AgoraIO.Common;
using Common.AppConfiguration;
using Common.Constants;
using Common.Interfaces;
using Common.Models;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System.Net.Http.Headers;

namespace Common.Services
{
    public class AgoraService : IAgoraService
    {
        private readonly AgoraSettings _agoraSettings;
        private readonly IRedisCacheService _redisCacheService;
        private readonly HttpClient _client;

        public AgoraService(IRedisCacheService redisCacheService, IOptions<AgoraSettings> agoraSettings)
        {
            var agoraSettingsValue = agoraSettings.Value;
            _agoraSettings = agoraSettingsValue;
            _redisCacheService = redisCacheService;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
        }

        private async Task<string> BuildChatAppToken()
        {
            ChatTokenBuilder2 tokenBuilder = new ChatTokenBuilder2();
            var tokenAgoraApp = tokenBuilder.buildAppToken(_agoraSettings.AppId, _agoraSettings.AppCertificate, 86400); // 24 hours
            return tokenAgoraApp;
        }

        public async Task CreateAgoraUser(AddUserAgora addUserAgora)
        {
            string? tokenAgoraApp = await BuildChatAppToken();

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenAgoraApp}");
            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(addUserAgora));
            string url = $"{_agoraSettings.BaseUrl}/{_agoraSettings.OrgName}/{_agoraSettings.AppName}/users";

            var httpResponse = await _client.PostAsync(url, httpContent);
            httpResponse.EnsureSuccessStatusCode();
        }

        public Task<string> GenerateCallingToken(int userId, string channelName, RtcTokenBuilder2.Role role)
        {
            RtcTokenBuilder2 tokenBuilder = new RtcTokenBuilder2();

            string token = tokenBuilder.buildTokenWithUid(_agoraSettings.AppId, _agoraSettings.AppCertificate, channelName, userId, role, 60000, 60000);

            return Task.FromResult(token);
        }

        public async Task SendChatMessage(string fromUsername, string toUsername, string content)
        {
            string? tokenAgoraApp = await BuildChatAppToken();

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenAgoraApp}");
            var request = new AgoraChatMessageRequest
            {
                From = fromUsername,
                To = new List<string> { toUsername },
                Type = "txt",
                Body = new AgoraChatMessageTextBody
                {
                    Content = content
                }
            };
            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(request));
            string url = $"{_agoraSettings.BaseUrl}/{_agoraSettings.OrgName}/{_agoraSettings.AppName}/messages/users";

            var httpResponse = await _client.PostAsync(url, httpContent);
            var response = await httpResponse.Content.ReadAsStringAsync();
            httpResponse.EnsureSuccessStatusCode();

        }
    }
}

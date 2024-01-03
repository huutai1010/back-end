using AgoraIO.Media;

using Common.AgoraIO.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAgoraService
    {
        Task CreateAgoraUser(AddUserAgora addUserAgora);
        Task<string> GenerateCallingToken(int userId, string channelName, RtcTokenBuilder2.Role role);
        Task SendChatMessage(string fromUsername, string toUsername, string content);
    }
}

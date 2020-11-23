using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models.OneSignalr;

namespace Kztek_Service.OneSignalr.Interfaces
{
    public interface IOS_PlayerService
    {
        Task<OS_Player> GetByPlayerId(string id);

        Task<OS_Player> GetByPlayerId_UserId(string id, string userid);

        Task<MessageReport> Create(OS_Player model);

        Task<MessageReport> Update(OS_Player model);

        Task<MessageReport> Delete(string id);

        Task<MessageReport> SendMessage(OneSignalrMessage model);

        Task<MessageReport> SendNotification(OneSignalrMessage model);

        Task<List<OS_Player>> GetPlayerIdsByUserIds(List<string> ids);

        Task<MessageReport> SendVoip(string fromUser, string toUser, List<string> playerids);
    }
}

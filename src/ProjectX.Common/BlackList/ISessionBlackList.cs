using System;
using System.Threading.Tasks;

namespace ProjectX.Common.BlackList
{
    public interface ISessionBlackList
    {
        Task AddToBlackListAsync(string sessionId, TimeSpan timeSpan);
        Task<bool> HasSessionAsync(string sessionId);
    }
}

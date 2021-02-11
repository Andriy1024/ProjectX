using System;
using System.Threading.Tasks;

namespace ProjectX.Core.BlackList
{
    public interface ISessionBlackList
    {
        Task AddToBlackListAsync(string sessionId, TimeSpan timeSpan);
        Task<bool> HasSessionAsync(string sessionId);
    }
}

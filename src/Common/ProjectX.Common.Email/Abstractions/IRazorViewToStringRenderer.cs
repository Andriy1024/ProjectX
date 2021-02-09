using System.Threading.Tasks;

namespace ProjectX.Common.Email
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}

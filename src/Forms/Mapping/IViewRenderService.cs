#if NETCOREAPP
using System.Threading.Tasks;

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
#endif

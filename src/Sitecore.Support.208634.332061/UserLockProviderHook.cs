using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Data.Locking;
using Sitecore.DependencyInjection;
using Sitecore.Events.Hooks;
using Sitecore.Support.Data.Locking;

namespace Sitecore.Support
{
    [UsedImplicitly]
    public class UserLockProviderHook : IHook
    {
        public void Initialize()
        {
            UserLockProvider.Instance = new DirectSqlQueryLockProvider(ServiceLocator.ServiceProvider.GetRequiredService<BaseItemManager>());
        }
    }
}
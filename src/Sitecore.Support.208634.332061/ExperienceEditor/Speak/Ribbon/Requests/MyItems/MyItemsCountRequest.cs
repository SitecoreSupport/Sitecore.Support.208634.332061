using Microsoft.Extensions.DependencyInjection;
using Sitecore.Configuration;
using Sitecore.Data.Locking;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.MyItems
{
    public class MyItemsCountRequest : Sitecore.ExperienceEditor.Speak.Ribbon.Requests.MyItems.MyItemsCountRequest
    {
        private readonly UserLockProvider _lockProvider;

        public MyItemsCountRequest() : this(ServiceLocator.ServiceProvider.GetRequiredService<UserLockProvider>())
        {
        }
        public MyItemsCountRequest(UserLockProvider lockProvider)
        {
            _lockProvider = lockProvider;
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            Assert.IsNotNull(this.RequestContext.Database, $"Could not get context.Database for requestArgs:{this.Args.Data}");

            var database = Factory.GetDatabase(this.RequestContext.Database);
            Assert.IsNotNull(database, $"Could not get database, with name:{this.RequestContext.Database}");

            if (!Sitecore.ExperienceEditor.Settings.WebEdit.ShowNumberOfLockedItemsOnButton)
            {
                return new PipelineProcessorResponseValue
                {
                    Value = string.Empty
                };
            }

            var userLockedItemArgs = new UserLockedItemArgs(Context.User.Identity, database);
            _lockProvider.GetItemsLockedByUser(userLockedItemArgs);
            var items = userLockedItemArgs.Result;
            return new PipelineProcessorResponseValue
            {
                Value = items.Count
            };
        }
    }
}
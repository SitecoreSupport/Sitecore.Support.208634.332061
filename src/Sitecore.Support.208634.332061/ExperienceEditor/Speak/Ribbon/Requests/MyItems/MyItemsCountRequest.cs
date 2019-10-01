using Sitecore.Configuration;
using Sitecore.Data.Locking;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.MyItems
{
    public class MyItemsCountRequest : Sitecore.ExperienceEditor.Speak.Ribbon.Requests.MyItems.MyItemsCountRequest
    {
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
            UserLockProvider.Instance.GetItemsLockedByUser(userLockedItemArgs);
            var items = userLockedItemArgs.Result;
            return new PipelineProcessorResponseValue
            {
                Value = items.Count
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Support.Shell.Applications.WebEdit.Commands
{
    /// <summary>
    /// Represents the Unlock command.
    /// </summary>
    [Obsolete("This method is obsolete and will be removed in the next product version. Please use SPEAK JS approach instead.")]
    [Serializable]
    public class UnlockAll : Sitecore.Shell.Applications.WebEdit.Commands.UnlockAll
    {
        #region Public methods

        /// <summary>
        /// Executes the specified page designer.
        /// </summary>
        public override void Execute([NotNull] CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            ContinuationManager.Current.Start(this, "OverriddenRun");
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Runs the pipeline.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected static void OverriddenRun([NotNull] ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var tempItems = new List<Item>();

            Item[] items = Client.ContentDatabase.SelectItems(@"search://*[@__lock='%""" + Context.User.Name + @"""%']");

            if (items == null || !items.Any())
            {
                SheerResponse.Alert(Texts.YOU_HAVE_NO_LOCKED_ITEMS);
                return;
            }

            if (args.IsPostBack)
            {
                if (args.Result == "yes")
                {
                    foreach (var item in items)
                    {
                        foreach (var tempItem in item.Versions.GetVersions(true))
                        {
                            if (string.Compare(tempItem.Locking.GetOwner(), Context.User.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                tempItems.Add(tempItem);
                            }
                        }
                    }
                    ProgressBox.Execute("UnlockAll", Texts.UNLOCKING_ITEMS, "Network/16x16/lock.png", UnlockAllItems, "lockeditems:refresh", Context.User, tempItems);
                }
            }
            else
            {
                if (items.Count() == 1)
                {
                    SheerResponse.Confirm(Texts.ARE_YOU_SURE_YOU_WANT_TO_UNLOCK_THIS_ITEM);
                }
                else
                {
                    SheerResponse.Confirm(Translate.Text(Texts.ARE_YOU_SURE_YOU_WANT_TO_UNLOCK_THESE_0_ITEMS, items.Count()));
                }

                args.WaitForPostBack();
            }
        }

        #endregion

        #region Private methods

        private static readonly MethodInfo UnlockAllItemsMethodInfo =
            typeof(Sitecore.Shell.Applications.WebEdit.Commands.UnlockAll).GetMethod("UnlockAllItems",
                BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Unlocks all.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        static void UnlockAllItems([NotNull] params object[] parameters)
        {
            UnlockAllItemsMethodInfo.Invoke(null, parameters);
        }

        #endregion
    }
}
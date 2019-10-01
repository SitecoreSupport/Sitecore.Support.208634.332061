using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Locking;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Sitecore.Support.Data.Locking
{
    public class DirectSqlQueryLockProvider : UserLockProvider
    {
        private readonly string sqlBody =
            $"SELECT [ItemId], [Version], [Language] FROM [VersionedFields] WHERE [FieldId]='{FieldIDs.Lock}' AND CHARINDEX(@lockOwner, [Value])=1";

        private readonly BaseItemManager manager;

        public DirectSqlQueryLockProvider(BaseItemManager manager)
        {
            Assert.ArgumentNotNull(manager, "manager");
            this.manager = manager;
        }

        public override void GetItemsLockedByUser(UserLockedItemArgs args)
        {
            Database contentDatabase = args.ContentDatabase;
            string connectionStringName = contentDatabase.ConnectionStringName;
            string connectionString = Assert.ResultNotNull(ConfigurationManager.ConnectionStrings[connectionStringName])
                .ConnectionString;
            List<DataUri> lockedItemUris = new List<DataUri>();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(sqlBody, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@lockOwner", $"<r owner=\"{args.User.Name}\"");
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    while (sqlDataReader.Read())
                    {
                        var itemId = new ID((Guid) sqlDataReader[0]);
                        var version = Sitecore.Data.Version.Parse(sqlDataReader[1]);
                        var language = Language.Parse((string) sqlDataReader[2]);
                        lockedItemUris.Add(new DataUri(itemId, language, version));
                    }
                }
                finally
                {
                    sqlDataReader.Close();
                }
            }

            foreach (DataUri lockedItemUri in lockedItemUris)
            {
                Item lockedItem = manager.GetItem(lockedItemUri.ItemID, lockedItemUri.Language, lockedItemUri.Version, contentDatabase);
                if (lockedItem != null)
                {
                    args.Result.Add(lockedItem);
                }
            }
        }
    }
}
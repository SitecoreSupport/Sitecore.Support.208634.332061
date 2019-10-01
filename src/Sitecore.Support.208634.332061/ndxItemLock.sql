/* Sitecore.Support.208634.332061
   The script should be executed against the master database
 */
CREATE NONCLUSTERED INDEX ndxItemLock
ON [VersionedFields] ([ItemId])
INCLUDE ([Language], [Version], [Value])
WHERE FieldId='{001DD393-96C5-490B-924A-B0F25CD9EFD8}'
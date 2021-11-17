using System;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace WorkshopWorkingWithData.Files.DataOperations
{
    internal sealed class UpdatingData
    {
        readonly UserConnection UserConnection;
        public UpdatingData(UserConnection userConnection)
        {
            UserConnection = userConnection;
        }

        internal string UpdateContactUpdate(Guid ContactId, string NewName)
        {
            const string tableName = "Contact";

            Select select = new Select(UserConnection)
                .Column("Name").From(tableName)
                .Where("Id").IsEqual(Column.Parameter(ContactId)) 
                as Select;

            string OldName = select.ExecuteScalar<string>();

            Update update = new Update(UserConnection, tableName)
                .Set("Name", Column.Parameter($"{OldName} {NewName}"))
                .Where("Id").IsEqual(Column.Parameter(ContactId)) as Update;
            return (update.Execute() == 1) ? "Updated":"Failed";
        }

        internal string UpdateContactEsq(Guid ContactId, string NewName)
        {
            const string tableName = "Contact";
            EntitySchema contactSchema = UserConnection.EntitySchemaManager.GetInstanceByName(tableName);
            Entity contact = contactSchema.CreateEntity(UserConnection);
            contact.FetchFromDB(ContactId);
            contact.SetColumnValue("Name", contact.GetTypedColumnValue<string>("Name")+" "+NewName);


            if (contact.UpdateInDB())
                return "Updated";
            return "Failed";
        }
    }
}

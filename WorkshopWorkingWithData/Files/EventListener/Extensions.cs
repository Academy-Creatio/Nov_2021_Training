using System;
using System.Collections.Generic;
using Terrasoft.Core.Entities;
using Terrasoft.Core.DB;
using Terrasoft.Common;

namespace WorkshopWorkingWithData.Files.EventListener
{
    public static class Extensions
    {
        public static bool IsChangeInteresting<Entity>(this Entity entity, string[] InterestingColumns, EntityColumnValueCollection entityColumnValues = null)
            where Entity : Terrasoft.Core.Entities.Entity
        {
            bool result = false;
            IEnumerable<EntityColumnValue> changedColumns;

            if (entityColumnValues != null)
            {
                changedColumns = entityColumnValues;
            }
            else
            {
                changedColumns = entity.GetChangedColumnValues();
            }

            foreach (EntityColumnValue column in changedColumns)
            {
                if (Array.IndexOf(InterestingColumns, column.Name) > -1)
                    return true;
            }
            return result;
        }

        public static bool DoesExist<Entity>(this Entity entity, string RootSchemName, object searchValue, string searchColumn)
            where Entity : Terrasoft.Core.Entities.Entity
        {
            Select select = new Select(entity.UserConnection)
                .Column(Func.Count("Id"))
                .From(RootSchemName)
                .Where(searchColumn).IsEqual(Column.Parameter(searchValue)) as Select;
            int count = select.ExecuteScalar<int>();

            return (count == 0) ? false : true;
        }

        public static Guid FindIdByValue<Entity>(this Entity entity, string RootSchemName, object searchValue, string searchColumn)
            where Entity : Terrasoft.Core.Entities.Entity
        {
            Select select = new Select(entity.UserConnection)
                .Top(1)
                .Column("Id")
                .From(RootSchemName)
                .Where(searchColumn).IsEqual(Column.Parameter(searchValue)) as Select;
            Guid Id = select.ExecuteScalar<Guid>();

            return Id;
        }

        public static string GetLocalizableString<Entity>(this Entity entity, string resourceName, string schemaName) where Entity : Terrasoft.Core.Entities.Entity
        {
            var localizableString = string.Format("LocalizableStrings.{0}.Value", resourceName);
            string result = new LocalizableString(entity.UserConnection.ResourceStorage,
                schemaName, localizableString);
            return result;
        }
    }
}
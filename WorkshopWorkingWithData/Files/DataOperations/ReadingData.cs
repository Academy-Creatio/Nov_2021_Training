using System;
using System.Data;
using Terrasoft.Common;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace WorkshopWorkingWithData.Files.DataOperations
{
    internal sealed class ReadingData
    {
        readonly UserConnection UserConnection;
        public ReadingData(UserConnection userConnection)
        {
            UserConnection = userConnection;
        }

        #region Example: GetAllData
        internal Tuple<DataTable, string> GetAllContactsEsq()
        {
            const string tableName = "Contact";
            EntitySchemaQuery esqResult = new EntitySchemaQuery(UserConnection.EntitySchemaManager, tableName);
            esqResult.PrimaryQueryColumn.IsVisible = true;
            
            esqResult.AddColumn("Name");
            esqResult.AddColumn("Email");
            esqResult.AddColumn("Phone");

            Select select = esqResult.GetSelectQuery(UserConnection);
            //select.BuildParametersAsValue = true;

            var entities = esqResult.GetEntityCollection(UserConnection);
            return new Tuple<DataTable, string>(entities.ConvertToDataTable(), select.GetSqlText());
        }
        internal Tuple<DataTable, string> GetAllContactsCustomQuery()
        {
            var type = UserConnection.DBEngine.DBEngineType;
            CustomQuery custom = new CustomQuery(UserConnection, "Select \"Id\", \"Name\", \"Phone\", \"Email\" from public.\"Contact\"");

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = custom.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            custom.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, custom.GetSqlText());
        }
        internal Tuple<DataTable, string> GetAllContactsSelect()
        {
            Select select = new Select(UserConnection)
                .Column("Id").As("Id")
                .Column("Contact", "Name").As("ContactName")
                .Column("Email")
                .Column("Phone")
                .From("Contact");


            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = select.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            //select.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, select.GetSqlText());
        }
        #endregion

        #region Example: GetFilteredData
        internal Tuple<DataTable, string> GetFilteredContactsSelect(string email)
        {
            Select select = new Select(UserConnection)
                .Column("Id").As("Id")
                .Column("Contact", "Name").As("ContactName")
                .Column("Email")
                .Column("Phone")
                .From("Contact")
                .Where("Email").IsEqual(Column.Parameter(email)) as Select;

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = select.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            select.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, select.GetSqlText());
        }
        internal Tuple<DataTable, string> GetFilteredContactsEsq(string email)
        {
            const string tableName = "Contact";
            EntitySchemaQuery esqResult = new EntitySchemaQuery(UserConnection.EntitySchemaManager, tableName);
            esqResult.PrimaryQueryColumn.IsVisible = true;

            esqResult.AddColumn("Name");
            esqResult.AddColumn("Email");
            esqResult.AddColumn("Phone");

            IEntitySchemaQueryFilterItem filterByEmail = esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "Email", email);
            esqResult.Filters.Add(filterByEmail);

            Select select = esqResult.GetSelectQuery(UserConnection);
            select.BuildParametersAsValue = true;

            var entities = esqResult.GetEntityCollection(UserConnection);
            return new Tuple<DataTable, string>(entities.ConvertToDataTable(), select.GetSqlText());
        }
        internal Tuple<DataTable, string> GetFilteredContactsCustomQuery(string email)
        {
        CustomQuery custom = new CustomQuery(UserConnection, $@"
            Select Id, Name, Phone, Email 
            from Contact
            where Email ='{email}'");

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = custom.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            custom.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, custom.GetSqlText());
        }
        #endregion

        #region Example: GetAllDataReverseJoin
        internal Tuple<DataTable, string> GetAllDataReverseJoinSelect()
        {
            Select select = new Select(UserConnection)
                .Column("Contact", "Name").As("ContactName")
                .Column("SysAdminUnit", "Name").As("SysAdminUnitName")
                .From("Contact")
                .LeftOuterJoin("SysAdminUnit").On("Contact", "Id").IsEqual("SysAdminUnit", "ContactId")
                as Select;
            
            //select.Where("SysAdminUnit", "Name").IsNotEqual(Column.Parameter(""));
            select.Where("SysAdminUnit", "Name").Not().IsNull();

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = select.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            select.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, select.GetSqlText());
        }
        internal Tuple<DataTable, string> GetAllDataReverseJoinEsq()
        {
            var esqResult = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
            var contactName = esqResult.AddColumn("Name");
            contactName.Name = "ContactName";

            // read Name of SysAdminUnit connected to Contact
            // https://academy.creatio.com/documents/technic-sdk/7-16/entityschemaquery-class-building-paths-columns
            // [Name_of_ joinable_schema:Name_of_column_for_linking_of_joinable_schema:Name_of_column_for_linking_of_current_schema].
            var sysAdminUnitName = esqResult.AddColumn("[SysAdminUnit:Contact:Id].Name"); 
            sysAdminUnitName.Name = "sysAdminUnitName";

            Select select = esqResult.GetSelectQuery(UserConnection);
            select.BuildParametersAsValue = true;

            var entities = esqResult.GetEntityCollection(UserConnection);
            return new Tuple<DataTable, string>(entities.ConvertToDataTable(), select.GetSqlText());
        }
        #endregion

        #region GetContactsWithMinutes
        internal Tuple<DataTable, string> GetContactsWithMinutesEsq(Guid ContactId)
        {
            const string tableName = "Activity";
            EntitySchemaQuery esqResult = new EntitySchemaQuery(UserConnection.EntitySchemaManager, tableName);
            esqResult.AddColumn("Owner.Id");
            esqResult.AddColumn("Owner.Name");
			
            
            EntitySchemaAggregationQueryFunction durationInMinutes = esqResult.CreateAggregationFunction(AggregationTypeStrict.Sum, "DurationInMinutes");
            durationInMinutes.Name = "DurationInMinutes";
            esqResult.AddColumn(durationInMinutes);
            
            
            IEntitySchemaQueryFilterItem filterByContactId = esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "Owner.Id", ContactId);
            esqResult.Filters.Add(filterByContactId);

            Select select = esqResult.GetSelectQuery(UserConnection);
            select.BuildParametersAsValue = true;

            var entities = esqResult.GetEntityCollection(UserConnection);
            return new Tuple<DataTable, string>(entities.ConvertToDataTable(), select.GetSqlText());
        }
        internal Tuple<DataTable, string> GetContactsWithMinutesSelect(Guid ContactId)
        {
            Select select = new Select(UserConnection)
                .Column("Activity", "OwnerId").As("OwnerId")
                .Column(Func.Max("Contact", "Name")).As("ContactName")
                .Column(Func.Sum("Activity", "DurationInMinutes")).As("Total Duration")
                .From("Activity")
                .LeftOuterJoin("Contact").On("Contact", "Id").IsEqual("Activity", "OwnerId")
                .GroupBy("Activity", "OwnerId")
                .Having(Func.Sum("Activity", "DurationInMinutes")).IsGreater(Column.Parameter(150))
                as Select;
            select.Where("Activity", "OwnerId").IsEqual(Column.Parameter(ContactId));

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = select.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Activity");
                }
            }
            select.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, select.GetSqlText());
        }
        #endregion
    }
}

using System;
using Terrasoft.Common;
using Terrasoft.Core;

using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;
using Terrasoft.Core.Entities.Events;
using WorkshopWorkingWithData.Files.WebServices;

namespace WorkshopWorkingWithData.Files.EventListener
{

    [EntityEventListener(SchemaName = "Activity")]
    class ActivityEventListener : BaseEntityEventListener
    {
        private static readonly Guid activityType = Guid.Parse("FBE0ACDC-CFC0-DF11-B00F-001D60E938C6");//Task
        private static readonly Guid activityCategory = Guid.Parse("F51C4643-58E6-DF11-971B-001D60E938C6"); //Activity Category: ToDo
        private static readonly Guid activityStatusNotStarted = Guid.Parse("384D4B84-58E6-DF11-971B-001D60E938C6"); // Not Started Status
        private static readonly Guid activityStatusInProgress = Guid.Parse("394D4B84-58E6-DF11-971B-001D60E938C6"); // InProgress Status
        private static readonly string[] InterestingColumns = { "OwnerId", "TypeId", "ActivityCategoryId", "StatusId", "StartDate", "DueDate" };
        private UserConnection UserConnection;

        public override void OnSaving(object sender, EntityBeforeEventArgs e)
        {
            base.OnSaving(sender, e);
            Entity entity = (Entity)sender;
            UserConnection = entity.UserConnection;

            //Check if columns changed are the ones I care about
            if (!entity.IsChangeInteresting(InterestingColumns)) return;

            //if (CountOverlappingActivitiesEsq(entity) != 0)
            if (CountOverlappingActivitiesSelect(entity) != 0)
            {
                e.IsCanceled = true;
                string message = entity.GetLocalizableString("OverlappingActivitiesFoundMessage", GetType().Name);
                MsgChannelUtilities.PostMessage(UserConnection, GetType().Name, message);
            }
        }


        public override void OnSaved(object sender, EntityAfterEventArgs e)
        {
            base.OnSaved(sender, e);
            Entity entity = (Entity)sender;
            
            if(entity.IsChangeInteresting(InterestingColumns, e.ModifiedColumnValues))
            {
                //TODO: do something interesting
            }
            else
            {
                //TODO: do nothing since the change IS NOT interesting
            }
        }

        private static int CountOverlappingActivitiesSelect(Entity activity)
        {
            int result = 0;
            UserConnection userConnection = activity.UserConnection;

            if (activity.GetTypedColumnValue<Guid>("TypeId") != activityType) return result;

            if (activity.GetTypedColumnValue<Guid>("ActivityCategoryId") != activityCategory) return result;

            if (activity.GetTypedColumnValue<Guid>("StatusId") != activityStatusInProgress &&
                activity.GetTypedColumnValue<Guid>("StatusId") != activityStatusNotStarted)
                return result;

            Guid activityId = activity.GetTypedColumnValue<Guid>("Id");
            Guid ownerId = activity.GetTypedColumnValue<Guid>("OwnerId");

            Select select = new Select(userConnection)
                .Column(Func.Count("Id"))
                .From("Activity")
                .Where("TypeId").IsEqual(Column.Parameter(activityType))
                    .And("ActivityCategoryId").IsEqual(Column.Parameter(activityCategory))
                    .And("OwnerId").IsEqual(Column.Parameter(ownerId))
                    .And("Id").IsNotEqual(Column.Parameter(activityId))
                as Select;

            TimeZoneInfo userTimeZoneInfo = userConnection.CurrentUser.TimeZone;
            DateTime start = TimeZoneInfo.ConvertTimeToUtc(activity.GetTypedColumnValue<DateTime>("StartDate"), userTimeZoneInfo);
            DateTime due = TimeZoneInfo.ConvertTimeToUtc(activity.GetTypedColumnValue<DateTime>("DueDate"), userTimeZoneInfo);

            select.And()
                //Case 1: Search target starts during new activity
                .OpenBlock("StartDate").IsBetween(Column.Parameter(start)).And(Column.Parameter(due))

                //Case2: Search target ends during new Activity
                .Or("DueDate").IsBetween(Column.Parameter(start)).And(Column.Parameter(due))

                //Case 3: activity inside search target;
                .Or()
                    .OpenBlock("StartDate").IsLessOrEqual(Column.Parameter(start))
                    .And("DueDate").IsGreaterOrEqual(Column.Parameter(due))
                .CloseBlock();

            select.BuildParametersAsValue = true;
            result = select.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// Example using Uses Entity Schema Query
        /// </summary>
        /// <param name="activity">Activity Entity</param>
        /// <returns>Return number of records found</returns>
        /// <remarks> <see cref="https://academy.creatio.com/documents/technic-sdk/7-15/introduction-13" /></remarks>
        private static int CountOverlappingActivityEsq(Entity activity)
        {
            int result = 0;
            UserConnection userConnection = activity.UserConnection;

            if (activity.GetTypedColumnValue<Guid>("TypeId") != activityType) return result;
            if (activity.GetTypedColumnValue<Guid>("ActivityCategoryId") != activityCategory) return result;
            if (activity.GetTypedColumnValue<Guid>("StatusId") != activityStatusInProgress &&
                activity.GetTypedColumnValue<Guid>("StatusId") != activityStatusNotStarted)
                return result;

            TimeZoneInfo userTimeZonInfo = userConnection.CurrentUser.TimeZone;

            DateTime start = TimeZoneInfo.ConvertTimeToUtc(activity.GetTypedColumnValue<DateTime>("StartDate").AddSeconds(1), userTimeZonInfo);
            DateTime due = TimeZoneInfo.ConvertTimeToUtc(activity.GetTypedColumnValue<DateTime>("DueDate").AddSeconds(-1), userTimeZonInfo);
            Guid ownerId = activity.GetTypedColumnValue<Guid>("OwnerId");

            EntitySchemaQuery esqResult = new EntitySchemaQuery(userConnection.EntitySchemaManager, "Activity");
            esqResult.AddColumn("Title");
            esqResult.AddColumn("StartDate");
            esqResult.AddColumn("DueDate");
            esqResult.AddColumn("Type");
            esqResult.AddColumn("ActivityCategory");
            esqResult.AddColumn("Owner");
            esqResult.AddColumn("Status");

            //Add Filters
            esqResult.Filters.LogicalOperation = Terrasoft.Common.LogicalOperationStrict.And;
            IEntitySchemaQueryFilterItem activityIdFilter =
                esqResult.CreateFilterWithParameters(FilterComparisonType.NotEqual, "Id", activity.GetTypedColumnValue<Guid>("Id"));
            esqResult.Filters.Add(activityIdFilter);

            esqResult.Filters.LogicalOperation = Terrasoft.Common.LogicalOperationStrict.And;
            IEntitySchemaQueryFilterItem activityTypeFilter =
                esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "Type", activityType);
            esqResult.Filters.Add(activityTypeFilter);

            IEntitySchemaQueryFilterItem activityCategoryFilter =
                esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "ActivityCategory", activityCategory);
            esqResult.Filters.Add(activityCategoryFilter);

            IEntitySchemaQueryFilterItem activityOwnerFilter =
                esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "Owner", ownerId);
            esqResult.Filters.Add(activityOwnerFilter);

            //Set Statuses
            esqResult.Filters.Add(
                new EntitySchemaQueryFilterCollection(
                    esqResult,
                    Terrasoft.Common.LogicalOperationStrict.Or,
                    esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "Status", activityStatusInProgress),
                    esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "Status", activityStatusNotStarted)
                    )
                );

            //Add Dates
            var dateFilter1 = new EntitySchemaQueryFilterCollection(
                esqResult,
                Terrasoft.Common.LogicalOperationStrict.Or,
                esqResult.CreateFilterWithParameters(FilterComparisonType.Between, "StartDate", start, due),
                esqResult.CreateFilterWithParameters(FilterComparisonType.Between, "DueDate", start, due)
            );

            var dateFilter2 = new EntitySchemaQueryFilterCollection(
                esqResult,
                Terrasoft.Common.LogicalOperationStrict.And,
                esqResult.CreateFilterWithParameters(FilterComparisonType.LessOrEqual, "StartDate", start),
                esqResult.CreateFilterWithParameters(FilterComparisonType.GreaterOrEqual, "DueDate", due)
            );

            esqResult.Filters.Add(
               new EntitySchemaQueryFilterCollection(
                       esqResult,
                       Terrasoft.Common.LogicalOperationStrict.Or,
                       dateFilter1,
                       dateFilter2
                   )
              );

            EntityCollection activities = esqResult.GetEntityCollection(userConnection);

            result = activities.Count;
            return result;
        }
    }
}

using System.Linq;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using Terrasoft.Core.Entities.Events;
using global::Common.Logging;

namespace EventsDemo.Files.cs.EventListener
{
	/// <summary>
	/// Listener for Contact entity events.
	/// </summary>
	/// <seealso cref="Terrasoft.Core.Entities.Events.BaseEntityEventListener" />
	/// <remarks>
	/// See <see href="https://academy.creatio.com/docs/developer/back-end_development/entity_event_layer/entity_event_layer">academny documentation</see>
	/// </remarks>
	[EntityEventListener(SchemaName = "Contact")]
	internal class ContactEventListener : BaseEntityEventListener
	{
		#region Enum
		#endregion

		#region Delegates
		#endregion

		#region Constants
		#endregion

		#region Fields

		#region Fileds : Private
		private ILog _log = LogManager.GetLogger("GuidedLearningLogger");
		#endregion

		#region Fileds : Protected
		#endregion

		#region Fileds : Internal
		#endregion

		#region Fileds : Protected Internal
		#endregion

		#region Fileds : Public
		#endregion

		#endregion

		#region Properties

		#region Properties : Private
		#endregion

		#region Properties : Protected
		#endregion

		#region Properties : Internal
		#endregion

		#region Properties : Protected Internal
		#endregion

		#region Properties : Public
		#endregion

		#endregion

		#region Events
		#endregion

		#region Methods

		#region Methods : Private

		#endregion

		#region Methods : Public

		#region Methods : Public : OnSave
		public override void OnSaving(object sender, EntityBeforeEventArgs e)
		{
			base.OnSaving(sender, e);
			Entity entity = (Entity)sender;
			UserConnection userConnection = entity.UserConnection;

			e.IsCanceled = true;
			entity.Validating += Entity_Validating;
		}


		private void Entity_Validating(object sender, EntityValidationEventArgs e)
		{

			Entity entity = (Entity)sender;
			string newName = entity.GetTypedColumnValue<string>("Name");
			string oldName = entity.GetTypedOldColumnValue<string>("Name");

			if(newName != oldName)
			{
				var evm = new EntityValidationMessage
				{
					Text = "Cannot save Andrew",
					MassageType = Terrasoft.Common.MessageType.Error,
					Column = entity.Schema.Columns.FindByName("Name")
				};

				entity.ValidationMessages.Add(evm);
			}
		}



		public override void OnSaved(object sender, EntityAfterEventArgs e)
		{
			base.OnSaved(sender, e);
			Entity entity = (Entity)sender;
			UserConnection userConnection = entity.UserConnection;
			//ILog _log = LogManager.GetLogger("GuidedLearningLogger");
			_log.Info("ContactEventListener  Cantact OnSaved");
		}
		#endregion


		#endregion

		#endregion
	}
}

 define("ActivitySectionV2", [], function() {
	return {
		entitySchemaName: "Activity",
		messages:{},
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[]/**SCHEMA_DIFF*/,
		methods: {

			/**
			* @inheritdoc Terrasoft.BasePageV2#init
			* @overridden
			*/
			init: function(){
				this.callParent(arguments);
				
				//WebSocket Subscribe
				this.Terrasoft.ServerChannel.on(Terrasoft.EventName.ON_MESSAGE, this.onOurMessage, this);
			},
			
			/**
			 * @inheritdoc Terrasoft.core.BaseObject#destroy
			 * @override
			 */
			destroy: function() {
				if (this.messages) {
					var messages = this.Terrasoft.keys(this.messages);
					this.sandbox.unRegisterMessages(messages);
				}
				this.Terrasoft.ServerChannel.un(this.Terrasoft.EventName.ON_MESSAGE, this.onOurMessage, this);
				this.callParent(arguments);
			},
				
			/**
			 * Our handler for when a websocket message arrives.
			 * Please make sure to encode message as proper json object
			 * @param {*} scope 
			 * @param {object} message sent from the backend
			 */
			 onOurMessage: function(scope, message){
				if(message.Header.Sender === "ActivityEventListener" && JSON.parse(message.Body).event === "Overlapping activities detected"){
					var event = JSON.parse(message.Body).event;
					this.showInformationDialog(event);
					this.updateSection();
				}
			}
		}
	};
});

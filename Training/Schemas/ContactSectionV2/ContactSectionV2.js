define("ContactSectionV2", [], function() {
	return {
		entitySchemaName: "Contact",
		messages:{

			/**
			 * Subscribed  on: ContactPageV2
			 * @tutorial https://academy.creatio.com/docs/developer/front-end_development/sandbox_component/module_message_exchange
			 */
			"SectionActionClicked":{
				mode: this.Terrasoft.MessageMode.PTP,
				direction: this.Terrasoft.MessageDirectionType.PUBLISH
			}
		},
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "PrimaryContactButtonRed",
				"parentName": "CombinedModeActionButtonsCardLeftContainer", //INVISIBLE in section, visible on the page
				"propertyName": "items",
				"values":{
					itemType: this.Terrasoft.ViewItemType.BUTTON,
					style: Terrasoft.controls.ButtonEnums.style.RED,
					classes: {
						"textClass": ["actions-button-margin-right"],
						"wrapperClass": ["actions-button-margin-right"]
					},
					caption: "Section Red Button",
					hint: "Section red button hint",
					click: {"bindTo": "onMyMainButtonClick"},
					tag: "CombinedModeActionButtonsCardLeftContainer_Red"
				}
			},
			{
				"operation": "insert",
				"name": "PrimaryContactButtonGreen",
				"parentName": "ActionButtonsContainer", //visible in section and on a page
				"propertyName": "items",
				"values":{
					itemType: this.Terrasoft.ViewItemType.BUTTON,
					style: Terrasoft.controls.ButtonEnums.style.GREEN,
					classes: {
						"textClass": ["actions-button-margin-right"],
						"wrapperClass": ["actions-button-margin-right"]
					},
					caption: "Section Green Button",
					hint: "Section red button hint",
					click: {"bindTo": "onMyMainButtonClick"},
					tag: "ActionButtonsContainer_Red"
				}
			}
			
		]/**SCHEMA_DIFF*/,
		methods: {
			getSectionActions: function() {
				var actionMenuItems = this.callParent(arguments);
				actionMenuItems.addItem(this.getButtonMenuSeparator());
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Tag": "action1",
					"Caption": "Section Action One",
					"Click": {"bindTo": "onActionClick"},
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Tag": "action2",
					"Caption": "Section Action Two",
					"Click": {"bindTo": "onActionClick"}
				}));
				return actionMenuItems;
			},
			onActionClick: function(tag){
				this.showInformationDialog("Section Action Clicked with tag: "+ tag);
				// this.sandbox.publish(
				// 	"SectionActionClicked", //Message name
				// 	null, 
				// 	["1234567890"] // TAG
				// );
			},
			onMyMainButtonClick: function(){
				var tag = arguments[3]; //identifies button
				//this.showInformationDialog("Button clicked: "+ tag);

				this.sandbox.publish(
					"SectionActionClicked", //Message name
					{arg1: 5, arg2: "arg2"}, 
					[this.sandbox.id+"_CardModuleV2"] // TAG
				);
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
				this.callParent(arguments);
			},
		}
	};
});

define("ContactPageV2", [], function() {
	return {
		entitySchemaName: "Contact",
		attributes: {

			IsVisible:{
				dataValueType: Terrasoft.DataValueType.BOOLEAN,
				value : false
			},

			"MyAttribute": {
				dependencies: [
					{
						columns: ["Email", "Name"],
						methodName: "onEmailChanged"
					},
					{
						columns: ["Name"],
						methodName: "onNameChanged"
					}
				]
			},

			"Account": {
				lookupListConfig: {
					columns: ["Web", "Code", "Owner", "Owner.Name"]
				}
			},

		},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {

			/**
			 * @inheritdoc Terrasoft.BasePageV2#onEntityInitialized
			 * @overridden
			 * @protected
			 */
			init: function() {
				this.callParent(arguments);
			},

			/**
			 * @inheritdoc Terrasoft.BasePageV2#onEntityInitialized
			 * @overridden
			 * @protected
			 */
			 onEntityInitialized: function() {
				this.callParent(arguments);
				this.setIsVisible();
			},

			onEmailChanged: function(){
				let colUnderChange = arguments[1];

				this.showInformationDialog(colUnderChange+" "+ "new value"+this.get(colUnderChange));
			},
			onNameChanged: function(){
				this.showInformationDialog("name has changed");
			},

			setIsVisible: function(){
				debugger;
				if(this.$Account["Owner.Name"]=== "Supervisor Kirill"){
					this.set("IsVisible", true)
				}else{
					this.set("IsVisible", false)
				}
			},

			/**
			 * @inheritdoc Terrasoft.BasePageV2#getActions
			 * @overridden
			 */
			 getActions: function() {
				var actionMenuItems = this.callParent(arguments);
				actionMenuItems.addItem(this.getButtonMenuSeparator());
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Tag": "action1",
					"Caption": this.get("Resources.Strings.ActionOneCaption"),
					"Click": {"bindTo": "onActionClick"},
					ImageConfig: this.get("Resources.Images.CreatioSquare"),
				}));

				actionMenuItems.addItem(this.getButtonMenuItem({
					"Tag": "action2",
					"Caption": this.get("Resources.Strings.ActionTwoCaption"),
					"Click": {"bindTo": "onActionClick"},
					"Items": this.addSubItems()
				}));

				return actionMenuItems;
			 },

			 onActionClick: function(){
				 this.showInformationDialog("Action clicked");
			 },
			
			addSubItems: function(){
				var collection = this.Ext.create("Terrasoft.BaseViewModelCollection");
				collection.addItem(this.getButtonMenuItem({
					"Caption": this.get("Resources.Strings.SubActionOneCaption"),
					"Click": {"bindTo": "onActionClick"},
					"Tag": "sub1"
				}));
				collection.addItem(this.getButtonMenuItem({
					"Caption": this.get("Resources.Strings.SubActionTwoCaption"),
					"Click": {"bindTo": "onActionClick"},
					"Tag": "sub2"
				}));
				return collection;
			},
			onMyMainButtonClick: function(){
				var tag = arguments[3];
				this.showInformationDialog(tag+" clicked");
			}

		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "MyName",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "ContactGeneralInfoBlock"
					},
					//"visible": {"bindTo": "IsVisible"},
					"enabled": {"bindTo": "IsVisible"},
					"bindTo": "Name"
				},
				"parentName": "ContactGeneralInfoBlock",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "PrimaryContactButtonRed",
				"parentName": "LeftContainer",
				"propertyName": "items",
				"values":{
					itemType: this.Terrasoft.ViewItemType.BUTTON,
					style: Terrasoft.controls.ButtonEnums.style.RED,
					classes: {
						"textClass": ["actions-button-margin-right"],
						"wrapperClass": ["actions-button-margin-right"]
					},
					caption: {bindTo: "Resources.Strings.MyRedBtnCaption"},
					hint: {bindTo:"Resources.Strings.MyRedBtnHint"},
					click: {"bindTo": "onMyMainButtonClick"},
					tag: "LeftContainer_Red"
				}
			},
			{
				"operation": "insert",
				"name": "MyGreenButton",
				"parentName": "LeftContainer",
				"propertyName": "items",
				"values":{
					"itemType": this.Terrasoft.ViewItemType.BUTTON,
					"style": Terrasoft.controls.ButtonEnums.style.GREEN,
					 classes: {
					 	"textClass": ["actions-button-margin-right"],
					 	"wrapperClass": ["actions-button-margin-right"]
					},
					"caption": "Page Green button",
					"hint": "Page green button hint <a href=\"https://google.ca\"> Link to help",
					"click": {"bindTo": "onMyMainButtonClick"},
					tag: "LeftContainer_Green",
					"menu":{
						"items": [
							{
								caption: "Sub Item 1",
								click: {bindTo: "onMySubButtonClick"},
								visible: true,
								hint: "Sub item 1 hint",
								tag: "subItem1"
							},
							{
								caption: "Sub Item 2",
								click: {bindTo: "onMySubButtonClick"},
								visible: true,
								hint: "Sub item 2 hint",
								tag: "subItem2"
							}
						]
					}
				}
			}


		]/**SCHEMA_DIFF*/
	};
});

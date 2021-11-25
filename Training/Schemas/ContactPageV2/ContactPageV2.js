define("ContactPageV2", ["ServiceHelper", "DevTrainingMixin", "css!DevTrainingMixin", "css!MyCustomCss"], function(ServiceHelper, resources) {
	return {
		entitySchemaName: "Contact",
		mixins: {
			"DevTrainingMixin": "Terrasoft.DevTrainingMixin"
		},
		messages:{

			/**
			 * Published on: ContactSectionV2
			 * @tutorial https://academy.creatio.com/docs/developer/front-end_development/sandbox_component/module_message_exchange
			 */
			"SectionActionClicked":{
				mode: this.Terrasoft.MessageMode.PTP,
				direction: this.Terrasoft.MessageDirectionType.SUBSCRIBE
			}
		},
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
				this.subscribeToMessages();
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
					"Click": {"bindTo": "onActionOneClick"},
					ImageConfig: this.get("Resources.Images.CreatioSquare"),
				}));

				actionMenuItems.addItem(this.getButtonMenuItem({
					"Tag": "action2",
					"Caption": this.get("Resources.Strings.ActionTwoCaption"),
					"Click": {"bindTo": "onActionTwoClick"},
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
			},
			
			//OnSect: this.sandbox.id = SectionModuleV2_ContactSectionV2
			//OnPage: this.sandbox.id = SectionModuleV2_ContactSectionV2_CardModuleV2
			subscribeToMessages: function(){
				this.sandbox.subscribe(
					"SectionActionClicked",
					function(args){this.onSectionMessageReceived(args);},
					this,
					[this.sandbox.id]
				)
			},

			onSectionMessageReceived: function(args){

				this.showInformationDialog("Message received");
			},


			/** Sets up synchronous validation, not suitable for async methods such as database requests or webservice calls
			 * @inheritdoc BaseSchemaViewModel#setValidationConfig
			 * @override
			 */
			 setValidationConfig: function() {
				this.callParent(arguments);
				this.addColumnValidator("Email", this.emailValidator);
			 },

			 emailValidator: function() {
				let invalidMessage= "";
				let newValue = this.$Email;
				let corpDomain = this.$Account.Web

				if (newValue.split("@")[1] !== corpDomain) {
					invalidMessage = "Primary email has to match to corporate domain.";
				}
				else {
					invalidMessage = "";
				}
				return {
					invalidMessage: invalidMessage
				};
			 },


			 /**
			 * Creation of query instance with "Contact" root schema. 
			 * @tutorial https://academy.creatio.com/docs/developer/front-end_development/crud_operations_in_configuration_schema/filters_handling
			 */
			onActionTwoClick: function(){

				this.doESQ();
				// var esq = Ext.create("Terrasoft.EntitySchemaQuery", {
				// 	rootSchemaName: "Contact"
				// });
				// esq.addColumn("Name");
				// esq.addColumn("Country.Name", "CountryName");

				// // Select all contacts where country is not specified.
				// var esqFirstFilter = esq.createColumnIsNullFilter("Country");
				// esq.filters.add("esqFirstFilter", esqFirstFilter);


				// // Select all contacts, date of birth of which fall at the period from 1.01.1970 to 1.01.1980.
				// var dateFrom = new Date(1970, 0, 1, 0, 0, 0, 0);
				// var dateTo = new Date(1980, 0, 1, 0, 0, 0, 0);
				// var esqSecondFilter = esq.createColumnBetweenFilterWithParameters("BirthDate", dateFrom, dateTo);
				
				// // Add created filters to query collection. 
				// esq.filters.add("esqSecondFilter", esqSecondFilter);

				// // This collection will include objects, i.e. query results, filtered by two filters.
				// esq.getEntityCollection(
				// 	function (result) 
				// 	{
				// 		if (result.success) {
				// 			result.collection.each(function (item) {
				// 				// Processing of collection items.
				// 				var message = item.$Name+" "+item.$CountryName;
				// 				this.showInformationDialog(message);
				// 			});
				// 		}
				// 	}, 
				// 	this
				// );
			},


			/**
			 * Call Custom Configuration WebService
			 * @tutorial https://academy.creatio.com/docs/developer/back-end_development/configuration_web_service/configuration_web_service#case-1241
			 */
			 onActionOneClick: function(){
				
				//Payload
				var serviceData = {
					"person":{
						"email": "andrew@domain.com",
						"name": this.$Name,
						"age": 0
					}	
				};

				// Calling the web service and processing the results.
				// Can only execute/send POST requests
				//https://baseUrl/0/rest/CustomExample/PostMethodName
				ServiceHelper.callService(
					"CreatioWsTemplate",  //CS - ClassName
					"Test2", //CS Method
					function(response) 
					{
						var result = response.Test2Result;
						if(result.name){
							//var name = result[0].name;
							this.showInformationDialog(result.name);
						}
					}, 
					serviceData, 
					this
				);
			},

		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "MyName",
				"values": {
					classes: {
						"wrapperClass": ["btn-orange"]
					},
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
					//style: Terrasoft.controls.ButtonEnums.style.RED,
					classes: {
						"textClass": ["actions-button-margin-right"],
						"wrapperClass": ["actions-button-margin-right", "btn-orange"]
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

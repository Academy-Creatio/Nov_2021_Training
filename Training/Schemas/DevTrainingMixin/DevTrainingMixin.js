define("DevTrainingMixin", [], function() {
	Ext.define("Terrasoft.DevTrainingMixin", {
		alternateClassName: "Terrasoft.configuration.mixins.DevTrainingMixin",

		test: function(){
			return "Ok";
		},

		doESQ: function() {
			var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {rootSchemaName: "Contact"});

			esq.addColumn("Id");
			esq.addColumn("Name");
			esq.addColumn("Email");
			// esq.addColumn("Industry");
			// esq.addColumn("AlternativeName");

			var esqFirstFilter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL, "Country.Name", "Canada");
			var esqSecondFilter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL, "Country.Id", "e0be1264-f36b-1410-fa98-00155d043204");

			esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.OR;
			esq.filters.add("esqFirstFilter", esqFirstFilter);
			esq.filters.add("esqSecondFilter", esqSecondFilter);

			var i = 0;

			esq.getEntityCollection(
				function (result) {
					if (!result.success) {
						// error processing/logging, for example
						this.showInformationDialog("Data query error");
						return;
					}
					result.collection.each(
						function (item) {
							i++;
							var name = name + " "+item.$Name;
							window.console.log(name)
					});
					this.showInformationDialog("Total Accounts: " + i);
				},
				this
			);
		},

	});
});
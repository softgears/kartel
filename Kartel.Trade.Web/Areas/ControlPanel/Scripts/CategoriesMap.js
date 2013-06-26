/**
 * Скрипт отображающий панель управления статистическими страницами
 */
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
	var mapGrid = {
		xtype            : 'grid',
		frame            : true,
		store            : new Ext.data.JsonStore({
			root       : 'data',
			idProperty : 'name',
			storeId    : 'mapStore',
			fields     : [
				'id',
				'categoryId',
				'categoryName',
				'displayName',
				'image',
				'sortOrder',
				'dateCreated',
				'categories'
			]
		}),
		height           : 400,
		autoExpandColumn : 'mapCategoryName',
		id               : 'mapGrid',
		selModel         : new Ext.grid.RowSelectionModel({

		}),
		colModel         : new Ext.grid.ColumnModel({
			columns : [
				{
					header    : 'ИД',
					dataIndex : 'id',
					sortable  : true,
					width     : 50,
					align     : 'left',
					id        : 'mapIdColumn'
				},
				{
					header    : 'Категория',
					dataIndex : 'categoryName',
					sortable  : true,
					width     : 250,
					align     : 'left',
					id        : 'mapCategoryName'
				},
				{
					header    : 'Отображаемое имя',
					dataIndex : 'displayName',
					sortable  : true,
					width     : 200,
					align     : 'left',
					id        : 'mapDisplayNameColumn'
				}
			]
		}),
		tbar             : [
			{
				text    : 'Обновить список',
				handler : function () {
					reloadMaps();
				}
			},
			'-',
			{
				text    : 'Добавить',
				tooltip : 'Отображает диалог добавления новой карты подкатегорий',
				handler : function () {
					showMapDialog();
				}
			},
			{
				text    : 'Редактировать',
				tooltip : 'Отображает диалог редактирования выбранной карты подкатегорий',
				handler : function () {
					var selected = Ext.getCmp('mapGrid').getSelectionModel().getSelected();
					if (selected != null) {
						showMapDialog(selected);
					}
				}
			},
			{
				text    : 'Удалить',
				tooltip : 'Отображает диалог удаления выбранной карты подкатегорий',
				handler : function () {
					var selectedRecord = Ext.getCmp('mapGrid').getSelectionModel().getSelected();
					if (selectedRecord == null) {
						return;
					}
					Ext.Msg.show({
						title   : 'Удаление карты категорий',
						msg     : 'Вы действительно хотите удалить выбранную карту подкатегорий?',
						buttons : Ext.Msg.YESNO,
						icon    : Ext.Msg.QUESTION,
						fn      : function (txt) {
							if (txt == "yes") {
								var id = selectedRecord.data.id;
								global.Ajax({
									url     : '/ControlPanel/ManageCategoriesMap/Delete',
									params  : {
										id : id
									},
									maskEl  : Ext.getCmp('mapGrid'),
									maskMsg : 'Идет удаление карты категорий',
									success : function (data) {
										reloadMaps();
									}
								});
							}
						}
					});
				}
			}
		],
		listeners        : {
			rowdblclick : function (number, e) {
				var selected = Ext.getCmp('mapGrid').getSelectionModel().getSelected();
				if (selected != null) {
					showMapDialog(selected);
				}
			}
		}
	};

	container.add(mapGrid);
	container.doLayout();

	/**
	 * Перезагружает список страниц с сервера
	 */
	function reloadMaps() {
		global.Ajax({
			url     : '/ControlPanel/ManageCategoriesMap/GetCategoryMaps',
			maskEl  : Ext.getCmp('mapGrid'),
			maskMsg : 'Идет загрузка списка карт категорий',
			success : function (data) {
				Ext.getCmp('mapGrid').getStore().loadData(data);
			}
		});
	}

	reloadMaps();

	/**
	 * Отображает диалог создания или редактирования страницы
	 * @param page
	 */
	var selModel = new Ext.grid.CheckboxSelectionModel({
		checkOnly    : true,
		singleSelect : false,
		sortable     : false,
		width        : 20
	});

	function showMapDialog(map) {
		var wnd = new Ext.Window({
			title     : map != undefined ? "Редактирование карты " + map.data.categoryName : "Создание карты",
			resizable : false,
			width     : 550,
			modal     : true,
			items     : [
				{
					xtype          : 'form',
					bodyStyle      : 'padding: 5px',
					name           : 'categoriesMapForm',
					id             : 'categoriesMapForm',
					labelAlign     : 'top',
					standartSubmit : true,
					fileUpload     : true,
					items          : [
						{
							xtype          : 'tabpanel',
							activeTab      : 0,
							border         : false,

							// Без этой строчки в CheckboxSelectionModel grid будет = undefined
							// а следовательно, ничего не будет работать
							// (на поиск решения этой проблемы потратил 3 часа)...
							// Оно того стоило!
							deferredRender : false,

							items : [
								{
									title  : 'Осн. характеристики',
									layout : 'form',
									margin : 10,
									items  : [
										{
											xtype : 'hidden',
											name  : 'id',
											id    : 'id',
											value : map != undefined ? map.data.id : -1
										},
										{
											xtype : 'hidden',
											name  : 'parent',
											id    : 'parent',
											value : map != undefined ? map.data.id : -1
										},
										{
											xtype : 'hidden',
											name  : 'categoryIds',
											id    : 'categoryIds',
											value : map != undefined ? map.data.id : -1
										},
										{
											xtype      : 'textfield',
											marginTop  : 10,
											allowBlank : false,
											fieldLabel : 'Заголовок',
											name       : 'displayName',
											id         : 'displayName',
											anchor     : '100%',
											value      : map != undefined ? map.data.displayName : ""
										},
										{
											xtype  : 'panel',
											id     : 'panel',
											html   : map != undefined ? '<img src="/files/' + map.data.image + '" style="width: 72px; height: 72px">' : '',
											border : false
										},
										{
											xtype      : 'fileuploadfield',
											anchor     : '100%',
											name       : 'file',
											id         : 'file',
											fieldLabel : 'Выберите изображение. Макс. размер - 2 Мб. Форматы: JPEG, PNG, GIF',
											buttonText : 'Выбрать',
											allowBlank : true
										},
										{
											xtype      : 'numberfield',
											allowBlank : false,
											fieldLabel : 'Индекс для сортировки',
											id         : 'sortOrder',
											name       : "sortOrder",
											anchor     : '100%',
											value      : map != undefined ? map.data.sortOrder : 1
										}
									]
								},
								{
									title  : 'Категории',
									layout : 'form',
									margin : 10,
									items  : [
										{
											xtype         : 'combo',
											id            : 'parentsCombo',
											name          : 'parentsCombo',
											valueField    : 'id',
											displayField  : 'title',
											width         : 250,
											mode          : 'local',
											triggerAction : 'all',
											editable      : false,
											lazyInit      : false,
											fieldLabel    : 'Выберите раздел',
											store         : new Ext.data.Store({
												url        : '/ControlPanel/ManageCategories/GetParentCategories',
												root       : 'data',
												idProperty : 'id',
												baseParams : {'categoryId' : map != undefined ? map.get('categoryId') : ''},
												autoLoad   : 'true',
												sortInfo   : {field : 'id', direction : "ASC"},
												reader     : new Ext.data.JsonReader({
													root   : 'data',
													fields : ['id', 'title']
												}),
												listeners  : {
													load : function () {
														Ext.getCmp('parentsCombo').setValue(map != undefined ? map.data.categoryId : 1);
													}
												}
											})
										},
										{
											xtype    : 'grid',
											selModel : selModel,
											loadMask : {msg : 'Идёт загрузка данных. Пожалуйста, подождите...'},
											store    : new Ext.data.GroupingStore({
												url        : '/ControlPanel/ManageCategories/GetFirstLevelSubCategories',
												root       : 'data',
												idProperty : 'parentId',
												autoLoad   : 'false',
												baseParams : {'mapId' : map != undefined ? map.get('id') : ''},
												sortInfo   : {field : 'id', direction : "ASC"},
												groupField : 'parentTitle',
												reader     : new Ext.data.JsonReader({
													root   : 'data',
													fields : ['id', 'title', 'parentId', 'parentTitle', 'selected']
												}),
												listeners  : {
													load : function () {
														var selected = [];
														this.each(function (e) {
															if (e.get('selected') === true) {
																selected.push(e);
															}
														});
														selModel.grid.getView().collapseAllGroups();
														selModel.clearSelections();
														selModel.selectRecords(selected)
													}
												}
											}),
											columns  : [
												selModel,
												{id : 'title', dataIndex : 'title', header : "Категория"},
												{id : 'parentTitle', dataIndex : 'parentTitle', header : "Раздел", hidden : true}
											],

											view : new Ext.grid.GroupingView({
												forceFit       : true,
												groupTextTpl   : '{text}',
												showGroupName  : false,
												startCollapsed : true
											}),

											width       : '100%',
											height      : 400,
											collapsible : false
										}
									]
								}
							]
						}
					]
				}
			],
			buttons   : [
				{
					text    : 'Сохранить',
					handler : function () {
						// Валидируем
						if (!Ext.getCmp('categoriesMapForm').getForm().isValid()) {
							Ext.Msg.alert('Ошибка', 'Пожалуйста, правильно заполните все поля формы');
							return;
						}

						var parentsCombo = Ext.getCmp('parentsCombo');

						var selectedIds = [];
						var selected = selModel.getSelections();
						Ext.each(selected, function (e) {
							selectedIds.push(e.get('id'));
						});

						Ext.getCmp('categoryIds').setValue(selectedIds.toString());
						Ext.getCmp('parent').setValue(parentsCombo.getValue());

						Ext.getCmp('categoriesMapForm').getForm().submit({
							url              : '/ControlPanel/ManageCategoriesMap/Save',
							clientValidation : true,
							isUpload         : true,
							success          : function (form, action) {
								// Успешно сохранили
								selModel.clearSelections();
								wnd.close();
								reloadMaps();
							},
							failure          : function (form, action) {
								wnd.close();
								reloadMaps();
							}
						});
					}
				},
				{
					text    : 'Закрыть',
					handler : function () {
						wnd.close();
					}
				}
			]
		});
		wnd.show();
	}
});
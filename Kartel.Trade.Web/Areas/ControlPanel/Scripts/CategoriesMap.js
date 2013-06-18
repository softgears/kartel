/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var mapGrid = {
        xtype: 'grid',
        frame: true,
        store: new Ext.data.JsonStore({
            root: 'data',
            idProperty: 'name',
            storeId: 'mapStore',
            fields: [
                'id',
                'categoryId',
                'categoryName',
                'displayName',
                'logoUrl',
                'sort'
            ]
        }),
        height: 400,
        autoExpandColumn: 'mapCategoryName',
        id: 'mapGrid',
        selModel: new Ext.grid.RowSelectionModel({

        }),
        colModel: new Ext.grid.ColumnModel({
            columns: [
                {
                    header: 'ИД',
                    dataIndex: 'id',
                    sortable: true,
                    width: 50,
                    align: 'left',
                    id: 'mapIdColumn'
                },
                {
                    header: 'Категория',
                    dataIndex: 'categoryName',
                    sortable: true,
                    width: 250,
                    align: 'left',
                    id: 'mapCategoryName'
                },
                {
                    header: 'Отображаемое имя',
                    dataIndex: 'displayName',
                    sortable: true,
                    width: 200,
                    align: 'left',
                    id: 'mapDisplayNameColumn'
                }
            ]
        }),
        tbar: [
            {
                text: 'Обновить список',
                handler: function () {
                    reloadMaps();
                }
            },
            '-',
            {
                text: 'Добавить',
                tooltip: 'Отображает диалог добавления новой карты подкатегорий',
                handler: function () {
                    showMapDialog();
                }
            },
            {
                text: 'Редактировать',
                tooltip: 'Отображает диалог редактирования выбранной карты подкатегорий',
                handler: function () {
                    var selected = Ext.getCmp('mapGrid').getSelectionModel().getSelected();
                    if (selected != null) {
                        showMapDialog(selected);
                    }
                }
            },
            {
                text: 'Удалить',
                tooltip: 'Отображает диалог удаления выбранной карты подкатегорий',
                handler: function () {
                    var selectedRecord = Ext.getCmp('mapGrid').getSelectionModel().getSelected();
                    if (selectedRecord == null) {
                        return;
                    }
                    Ext.Msg.show({
                        title: 'Удаление карты категорий',
                        msg: 'Вы действительно хотите удалить выбранную карту подкатегорий?',
                        buttons: Ext.Msg.YESNO,
                        icon: Ext.Msg.QUESTION,
                        fn: function (txt) {
                            if (txt == "yes") {
                                var id = selectedRecord.data.id;
                                global.Ajax({
                                    url: '/ControlPanel/ManageCategoriesMap/Delete',
                                    params: {
                                        id: id
                                    },
                                    maskEl: Ext.getCmp('mapGrid'),
                                    maskMsg: 'Идет удаление карты категорий',
                                    success: function (data) {
                                        reloadMaps();
                                    }
                                });
                            }
                        }
                    });
                }
            }
        ],
        listeners: {
            rowdblclick: function (number, e) {
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
            url: '/ControlPanel/ManageCategoriesMap/GetItems',
            maskEl: Ext.getCmp('mapGrid'),
            maskMsg: 'Идет загрузка списка карт категорий',
            success: function (data) {
                Ext.getCmp('mapGrid').getStore().loadData(data);
            }
        });
    }
    reloadMaps();

    /**
     * Отображает диалог создания или редактирования страницы
     * @param page
     */
    function showMapDialog(map) {
        var wnd = new Ext.Window({
            title: map != undefined ? "Редактирование карты " + map.data.categoryName : "Создание карты",
            resizable: true,
            width: 550,
            height: 700,
            modal: true,
            layout: 'fit',
            items: [
                {
                    xtype: 'form',
                    bodyStyle: 'padding: 5px',
                    id: 'bannerForm',
                    labelAlign: 'top',
                    standartSubmit: true,
                    fileUpload: true,
                    items: [
                        {
                            xtype: 'hidden',
                            id: 'bannerIdField',
                            value: map != undefined ? map.data.id : -1
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Заголовок',
                            id: 'bannerTitleField',
                            name: "title",
                            anchor: '100%',
                            value: map != undefined ? map.data.title : ""
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: true,
                            fieldLabel: 'Имя файла',
                            id: 'bannerImgField',
                            name: "img",
                            anchor: '100%',
                            value: map != undefined ? map.data.img : ""
                        },
                        new Ext.form.FileUploadField({
                            anchor: '100%',
                            id: 'fileField',
                            name: 'file',
                            fieldLabel: 'Выберите файл для загрузки. Максимальный размер - 2 мегабайта',
                            buttonText: 'Выбрать',
                            allowBlank: false
                        }),
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Ссылка',
                            id: 'bannerHrefField',
                            name: "href",
                            anchor: '100%',
                            value: map != undefined ? map.data.href : ""
                        },
                        {
                            xtype: 'numberfield',
                            allowBlank: false,
                            fieldLabel: 'Индекс для сортировки',
                            id: 'bannerSortField',
                            name: "sort",
                            anchor: '100%',
                            value: map != undefined ? map.data.sort : 1
                        },
                        {
                            xtype: 'textarea',
                            allowBlank: true,
                            fieldLabel: 'HTML',
                            id: 'bannerHtmlField',
                            name: "html",
                            anchor: '100%',
                            value: map != undefined ? map.data.html : ""
                        },
                        {
                            xtype: 'combo',
                            fieldLabel: 'Расположение',
                            id: 'bannerExtraField',
                            mode: 'local',
                            triggerAction: 'all',
                            lazyRender: true,
                            readOnly: false,
                            anchor: '100%',
                            typeAhead: true,
                            store: new Ext.data.ArrayStore({
                                fields: ['id', 'name'],
                                data: [
                                    [1, "Меняющийся баннер на главной"],
                                    [2, "Категории товаров"],
                                    [3, "Категории тендеров"],
                                    [4, "Список товаров в категории"],
                                    [5, "Список тендеров в категории"],
                                    [6, "Страница товара"],
                                    [7, "Страница тендера"]
                                ]
                            }),
                            displayField: 'name',
                            valueField: 'id',
                            name: 'extra',
                            value: map != undefined ? map.data.extra : 1
                        },
                        {
                            xtype: 'textarea',
                            allowBlank: true,
                            fieldLabel: 'Идентификаторы товаров',
                            id: 'bannerObjectsField',
                            name: "objects",
                            anchor: '100%',
                            value: map != undefined ? map.data.objects : ""
                        },
                        {
                            xtype: 'textarea',
                            allowBlank: true,
                            fieldLabel: 'Идентификаторы категорий',
                            id: 'bannerCategoriesField',
                            name: "categories",
                            anchor: '100%',
                            value: map != undefined ? map.data.categories : ""
                        }
                    ]
                }
            ],
            buttons: [
                {
                    text: 'Сохранить',
                    handler: function () {
                        // Валидируем
                        if (!Ext.getCmp('bannerForm').getForm().isValid()) {
                            Ext.Msg.alert('Ошибка', 'Пожалуйста, правильно заполните все поля формы');
                            return;
                        }

                        Ext.getCmp('bannerForm').getForm().submit({
                            url: '/ControlPanel/ManageBanners/Save',
                            clientValidation: true,
                            success: function (form, action) {
                                // Успешно сохранили
                                wnd.close();
                                reloadMaps();
                            },
                            failure: function (form, action) {
                                wnd.close();
                                reloadMaps();
                            }
                        });
                    }
                },
                {
                    text: 'Закрыть',
                    handler: function () {
                        wnd.close();
                    }
                }
            ]
        });
        wnd.show();
    }
});
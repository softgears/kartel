/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var bannerTemplatesGrid = {
        xtype: 'grid',
        frame: true,
        store: new Ext.data.GroupingStore({
            root: 'data',
            idProperty: 'id',
            groupField: 'category',
            storeId: 'billsStore',
            sortInfo: { field: 'id', direction: "ASC" },
            reader: new Ext.data.JsonReader({
                root: 'data',
                fields: [
                    'id',
                    'filename',
                    'category'
                ]
            })
        }),
        height: 400,
        autoExpandColumn: 'templateFilenameColumn',
        id: 'bannerTemplatesGrid',
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
                    id: 'templateIdColumn'
                },
                {
                    header: 'Имя файла',
                    dataIndex: 'filename',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'templateFilenameColumn'
                },
                {
                    header: 'Категория',
                    dataIndex: 'category',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'templateColumnColumn'
                }
            ]
        }),
        tbar: [
            {
                text: 'Обновить список',
                handler: function () {
                    reloadTemplates();
                }
            },
            '-',
            {
                text: 'Добавить',
                tooltip: 'Отображает диалог добавления нового шаблона баннера',
                handler: function () {
                    showBannerDialog();
                }
            },
            {
                text: 'Редактировать',
                tooltip: 'Отображает диалог редактирования выбранного шаблона баннера',
                handler: function () {
                    var selected = Ext.getCmp('bannerTemplatesGrid').getSelectionModel().getSelected();
                    if (selected != null) {
                        showBannerDialog(selected);
                    }
                }
            },
            {
                text: 'Удалить',
                tooltip: 'Отображает диалог удаления выбранного шаблона баннера',
                handler: function () {
                    var selectedRecord = Ext.getCmp('bannerTemplatesGrid').getSelectionModel().getSelected();
                    if (selectedRecord == null) {
                        return;
                    }
                    Ext.Msg.show({
                        title: 'Удаление баннера',
                        msg: 'Вы действительно хотите удалить выбранный шаблон баннера?',
                        buttons: Ext.Msg.YESNO,
                        icon: Ext.Msg.QUESTION,
                        fn: function (txt) {
                            if (txt == "yes") {
                                var id = selectedRecord.data.id;
                                global.Ajax({
                                    url: '/ControlPanel/ManageBannersTemplate/Delete',
                                    params: {
                                        id: id
                                    },
                                    maskEl: Ext.getCmp('bannerTemplatesGrid'),
                                    maskMsg: 'Идет удаление шаблона баннера',
                                    success: function (data) {
                                        reloadTemplates();
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
                var selected = Ext.getCmp('bannerTemplatesGrid').getSelectionModel().getSelected();
                if (selected != null) {
                    showBannerDialog(selected);
                }
            }
        },
        view: new Ext.grid.GroupingView({
            forceFit: true,
            groupTextTpl: '{text} ({[values.rs.length]} штук)',
            showGroupName: true,
            startCollapsed: true
        }),
    };

    container.add(bannerTemplatesGrid);
    container.doLayout();

    /**
     * Перезагружает список счетов
     */
    function reloadTemplates() {
        global.Ajax({
            url: '/ControlPanel/ManageBannerTemplates/GetBanners',
            maskEl: Ext.getCmp('bannerTemplatesGrid'),
            maskMsg: 'Идет загрузка списка шаблонов баннеров',
            success: function (data) {
                Ext.getCmp('bannerTemplatesGrid').getStore().loadData(data);
            }
        });
    }
    reloadTemplates();

    /**
     * Отображает диалог создания или редактирования страницы
     * @param page
     */
    function showBannerDialog(page) {
        var wnd = new Ext.Window({
            title: page != undefined ? "Редактирование шаблона баннера " + page.data.filename : "Создание шаблона баннера",
            resizable: true,
            width: 550,
            modal: true,
            items: [
                {
                    xtype: 'form',
                    bodyStyle: 'padding: 5px',
                    id: 'bannerTemplateForm',
                    labelAlign: 'top',
                    standartSubmit: true,
                    fileUpload: true,
                    items: [
                        {
                            xtype: 'hidden',
                            name: 'id',
                            value: page != undefined ? page.data.id : -1
                        },
                        {
                            xtype: 'panel',
                            id: 'panel',
                            height: 120,
                            html: page != undefined ? '<img src="/files/bannertemplates/' + page.data.filename + '" style="width: 120px; height: 120px">' : '',
                            border: false
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: true,
                            fieldLabel: 'Имя файла',
                            name: "filename",
                            anchor: '100%',
                            value: page != undefined ? page.data.filename : ""
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Категория',
                            name: "category",
                            anchor: '100%',
                            value: page != undefined ? page.data.category : "Общие"
                        },
                        new Ext.form.FileUploadField({
                            anchor: '100%',
                            id: 'fileField',
                            name: 'file',
                            fieldLabel: 'Выберите файл для загрузки. Максимальный размер - 2 мегабайта',
                            buttonText: 'Выбрать',
                            allowBlank: true
                        })
                    ]
                }
            ],
            buttons: [
                {
                    text: 'Сохранить',
                    handler: function () {
                        // Валидируем
                        if (!Ext.getCmp('bannerTemplateForm').getForm().isValid()) {
                            Ext.Msg.alert('Ошибка', 'Пожалуйста, правильно заполните все поля формы');
                            return;
                        }

                        Ext.getCmp('bannerTemplateForm').getForm().submit({
                            url: '/ControlPanel/ManageBannerTemplates/Save',
                            clientValidation: true,
                            success: function (form, action) {
                                // Успешно сохранили
                                wnd.close();
                                reloadTemplates();
                            },
                            failure: function (form, action) {
                                wnd.close();
                                reloadTemplates();
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
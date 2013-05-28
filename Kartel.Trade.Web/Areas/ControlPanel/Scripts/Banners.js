/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var bannersGrid = {
        xtype: 'grid',
        frame: true,
        store: new Ext.data.JsonStore({
            root: 'data',
            idProperty: 'name',
            storeId: 'bannersStore',
            fields: [
                'id',
                'title',
                'img',
                'href',
                'html',
                'sort',
                'objects',
                'categories',
                'extra'
            ]
        }),
        height: 400,
        autoExpandColumn: 'bannerHrefColumn',
        id: 'bannersGrid',
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
                    id: 'bannerIdColumn'
                },
                {
                    header: 'Заголовок',
                    dataIndex: 'title',
                    sortable: true,
                    width: 250,
                    align: 'left',
                    id: 'bannerTitleColumn'
                },
                {
                    header: 'Ссылка',
                    dataIndex: 'href',
                    sortable: true,
                    width: 200,
                    align: 'left',
                    id: 'bannerHrefColumn'
                }
            ]
        }),
        tbar: [
            {
                text: 'Обновить список',
                handler: function () {
                    reloadBanners();
                }
            },
            '-',
            {
                text: 'Добавить',
                tooltip: 'Отображает диалог добавления нового баннера',
                handler: function () {
                    showBannerDialog();
                }
            },
            {
                text: 'Редактировать',
                tooltip: 'Отображает диалог редактирования выбранного баннера',
                handler: function () {
                    var selected = Ext.getCmp('bannersGrid').getSelectionModel().getSelected();
                    if (selected != null) {
                        showBannerDialog(selected);
                    }
                }
            },
            {
                text: 'Удалить',
                tooltip: 'Отображает диалог удаления выбранного баннера',
                handler: function () {
                    var selectedRecord = Ext.getCmp('bannersGrid').getSelectionModel().getSelected();
                    if (selectedRecord == null) {
                        return;
                    }
                    Ext.Msg.show({
                        title: 'Удаление баннера',
                        msg: 'Вы действительно хотите удалить выбранный баннер?',
                        buttons: Ext.Msg.YESNO,
                        icon: Ext.Msg.QUESTION,
                        fn: function (txt) {
                            if (txt == "yes") {
                                var id = selectedRecord.data.id;
                                global.Ajax({
                                    url: '/ControlPanel/ManageBanners/Delete',
                                    params: {
                                        id: id
                                    },
                                    maskEl: Ext.getCmp('bannersGrid'),
                                    maskMsg: 'Идет удаление баннера',
                                    success: function (data) {
                                        reloadBanners();
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
                var selected = Ext.getCmp('bannersGrid').getSelectionModel().getSelected();
                if (selected != null) {
                    showBannerDialog(selected);
                }
            }
        }
    };

    container.add(bannersGrid);
    container.doLayout();

    /**
     * Перезагружает список страниц с сервера
     */
    function reloadBanners() {
        global.Ajax({
            url: '/ControlPanel/ManageBanners/GetBanners',
            maskEl: Ext.getCmp('bannersGrid'),
            maskMsg: 'Идет загрузка списка баннеров',
            success: function (data) {
                Ext.getCmp('bannersGrid').getStore().loadData(data);
            }
        });
    }
    reloadBanners();

    /**
     * Отображает диалог создания или редактирования страницы
     * @param page
     */
    function showBannerDialog(page) {
        var wnd = new Ext.Window({
            title: page != undefined ? "Редактирование баннера " + page.data.title : "Создание баннера",
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
                            value: page != undefined ? page.data.id : -1
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Заголовок',
                            id: 'bannerTitleField',
                            name: "title",
                            anchor: '100%',
                            value: page != undefined ? page.data.title : ""
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: true,
                            fieldLabel: 'Имя файла',
                            id: 'bannerImgField',
                            name: "img",
                            anchor: '100%',
                            value: page != undefined ? page.data.img : ""
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
                            value: page != undefined ? page.data.href : ""
                        },
                        {
                            xtype: 'numberfield',
                            allowBlank: false,
                            fieldLabel: 'Индекс для сортировки',
                            id: 'bannerSortField',
                            name: "sort",
                            anchor: '100%',
                            value: page != undefined ? page.data.sort : 1
                        },
                        {
                            xtype: 'textarea',
                            allowBlank: true,
                            fieldLabel: 'HTML',
                            id: 'bannerHtmlField',
                            name: "html",
                            anchor: '100%',
                            value: page != undefined ? page.data.html : ""
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
                            value: page != undefined ? page.data.extra : 1
                        },
                        {
                            xtype: 'textarea',
                            allowBlank: true,
                            fieldLabel: 'Идентификаторы товаров',
                            id: 'bannerObjectsField',
                            name: "objects",
                            anchor: '100%',
                            value: page != undefined ? page.data.objects : ""
                        },
                        {
                            xtype: 'textarea',
                            allowBlank: true,
                            fieldLabel: 'Идентификаторы категорий',
                            id: 'bannerCategoriesField',
                            name: "categories",
                            anchor: '100%',
                            value: page != undefined ? page.data.categories : ""
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
                                reloadBanners();
                            },
                            failure: function (form, action) {
                                wnd.close();
                                reloadBanners();
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
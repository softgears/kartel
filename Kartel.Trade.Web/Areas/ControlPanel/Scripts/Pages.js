/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var pagesGrid = {
        xtype: 'grid',
        frame: true,
        store: new Ext.data.JsonStore({
            root: 'data',
            idProperty: 'name',
            storeId: 'pagesStore',
            fields: [
                'id',
                'title',
                'route',
                'content',
                'views',
                'dateCreated',
                'dateModified'
            ]
        }),
        height: 400,
        autoExpandColumn: 'pageTitleColumn',
        id: 'pagesGrid',
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
                    id: 'pageIdColumn'
                },
                {
                    header: 'Заголовок',
                    dataIndex: 'title',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'pageTitleColumn'
                },
                {
                    header: 'Путь',
                    dataIndex: 'route',
                    sortable: true,
                    width: 300,
                    align: 'left',
                    id: 'pageRouteColumn'
                },
                {
                    header: 'Просмотров',
                    dataIndex: 'views',
                    sortable: true,
                    width: 120,
                    align: 'left',
                    id: 'pageViewsColumn'
                },
                {
                    header: 'Дата создания',
                    dataIndex: 'dateCreated',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'pageDateCreatedColumn'
                },
                {
                    header: 'Дата редактирования',
                    dataIndex: 'dateModified',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'pageDateModifiedColumn'
                }
            ]
        }),
        tbar: [
            {
                text: 'Обновить список',
                handler: function() {
                    reloadPages();
                }
            },
            '-',
            {
                text: 'Добавить',
                tooltip: 'Отображает диалог добавления новой страницы',
                handler: function(){
                    showPageDialog();
                }
            },
            {
                text: 'Редактировать',
                tooltip: 'Отображает диалог редактирования выбранной страницы',
                handler: function(){
                    var selected = Ext.getCmp('pagesGrid').getSelectionModel().getSelected();
                    if (selected != null){
                        showPageDialog(selected);
                    }
                }
            },
            {
                text: 'Удалить',
                tooltip: 'Отображает диалог удаления выбранной страницы',
                handler: function(){
                    var selectedRecord = Ext.getCmp('pagesGrid').getSelectionModel().getSelected();
                    if (selectedRecord == null){
                        return;
                    }
                    Ext.Msg.show({
                        title: 'Удаление страницы',
                        msg: 'Вы действительно хотите удалить выбранную страницу?',
                        buttons: Ext.Msg.YESNO,
                        icon: Ext.Msg.QUESTION,
                        fn: function(txt){
                            if (txt == "yes"){
                                var id = selectedRecord.data.id;
                                global.Ajax({
                                    url: '/ControlPanel/ManagePages/Delete',
                                    params: {
                                        id: id
                                    },
                                    maskEl: Ext.getCmp('pagesGrid'),
                                    maskMsg: 'Идет удаление страницы',
                                    success: function(data){
                                        reloadPages();
                                    }
                                });
                            }
                        }
                    });
                }
            }
        ],
        listeners: {
            rowdblclick: function(number,e){
                var selected = Ext.getCmp('pagesGrid').getSelectionModel().getSelected();
                if (selected != null){
                    showPageDialog(selected);
                }
            }
        }
    };

    container.add(pagesGrid);
    container.doLayout();

    /**
     * Перезагружает список страниц с сервера
     */
    function reloadPages(){
        global.Ajax({
            url: '/ControlPanel/ManagePages/GetPages',
            maskEl: Ext.getCmp('pagesGrid'),
            maskMsg: 'Идет загрузка списка страниц',
            success: function(data){
                Ext.getCmp('pagesGrid').getStore().loadData(data);
            }
        });
    }
    reloadPages();

    /**
     * Отображает диалог создания или редактирования страницы
     * @param page
     */
    function showPageDialog(page){
        var wnd = new Ext.Window({
            title: page != undefined ? "Редактирование страницы "+page.data.title : "Создание страницы",
            resizable: true,
            width: 550,
            height: 700,
            modal: true,
            layout: 'fit',
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 0,
                    defferedRender: false,
                    items: [
                        {
                            title: 'Свойства страницы',
                            items: [
                                {
                                    xtype: 'form',
                                    bodyStyle: 'padding: 5px',
                                    id: 'pageForm',
                                    labelAlign: 'top',
                                    items: [
                                        {
                                            xtype: 'hidden',
                                            id: 'pageIdField',
                                            value: page != undefined ? page.data.id : -1
                                        },
                                        {
                                            xtype: 'textfield',
                                            allowBlank: false,
                                            fieldLabel: 'Заголовок',
                                            id: 'pageTitleField',
                                            anchor: '100%',
                                            value: page != undefined ? page.data.title : ""
                                        },
                                        {
                                            xtype: 'textfield',
                                            allowBlank: false,
                                            fieldLabel: 'Путь, URL к странице',
                                            id: 'pageRouteField',
                                            anchor: '100%',
                                            value: page != undefined ? page.data.route : ""
                                        }
                                    ]
                                }
                            ]
                        },
                        {
                            title: 'Содержимое страницы (HTML)',
                            layout: 'fit',
                            items: [
                                {
                                    xtype: 'htmleditor',
                                    allowBlank: true,
                                    id: 'pageContentField',
                                    value: page != undefined ? page.data.content : ""
                                }
                            ]
                        }
                    ]
                }
            ],
            buttons: [
                {
                    text: 'Сохранить',
                    handler: function(){
                        // Валидируем
                        if (!Ext.getCmp('pageForm').getForm().isValid()){
                            Ext.Msg.alert('Ошибка','Пожалуйста, правильно заполните все поля формы');
                            return;
                        }

                        // Подготавливаем
                        var params = {
                            id: Ext.getCmp('pageIdField').getValue(),
                            title: Ext.getCmp('pageTitleField').getValue(),
                            route: Ext.getCmp('pageRouteField').getValue(),
                            content: Ext.getCmp('pageContentField').getValue()
                        };

                        // Отправляем
                        global.Ajax({
                            url: '/ControlPanel/ManagePages/Save',
                            params: params,
                            maskEl: Ext.getCmp('pagesGrid'),
                            maskMsg: 'Идет сохранение страницы',
                            success: function(data){
                                wnd.close();
                                reloadPages();
                            }
                        });
                    }
                },
                {
                    text: 'Закрыть',
                    handler: function(){
                        wnd.close();
                    }
                }
            ]
        });
        wnd.show();
    }
});
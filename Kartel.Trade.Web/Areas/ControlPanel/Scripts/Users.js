/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var store = new Ext.data.JsonStore({
        root: 'data',
        idProperty: 'name',
        url: '/ControlPanel/ManageUsers/GetUsers',
        storeId: 'usersStore',
        totalProperty: "results",
        fields: [
            'id',
            'login',
            'subdomain',
            'company',
            'brand',
            'fio',
            'email',
            'icq',
            'skype',
            'url',
            'country',
            'region',
            'city',
            'about',
            'address',
            'zip',
            'postcode',
            'importer',
            'oem',
            'whoseller',
            'singleSeller',
            'developer',
            'agent',
            'distributor',
            'ogrn',
            'inn',
            'kpp',
            'accountRnumber',
            'accountKNumber',
            'bank',
            'bankBik',
            'dealer',
            'tariff',
            'tariffExpiration',
            'regDate'
        ]
    });
    var usersGrid = {
        xtype: 'grid',
        frame: true,
        store: store,
        height: 400,
        autoExpandColumn: 'userCompanyColumn',
        id: 'usersGrid',
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
                    id: 'userIdColumn'
                },
                {
                    header: 'Компания',
                    dataIndex: 'company',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'userCompanyColumn'
                },
                {
                    header: 'Email',
                    dataIndex: 'email',
                    sortable: true,
                    width: 180,
                    align: 'left',
                    id: 'userEmailColumn'
                },
                {
                    header: 'Тариф',
                    dataIndex: 'tariff',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'userTariffColumn'
                },
                {
                    header: 'Тариф кончается',
                    dataIndex: 'tariffExpiration',
                    sortable: true,
                    width: 120,
                    align: 'left',
                    id: 'tariffExpirationColumn'
                },
                {
                    header: 'Контактное лицо',
                    dataIndex: 'fio',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'userFioColumn'
                }
            ]
        }),
        tbar: [
            {
                text: 'Все пользователи',
                handler: function () {
                    loadAllUsers();
                }
            },
            {
                text: 'Премиальные пользователи',
                handler: function () {
                    loadPremiumUsers();
                }
            },
            '-',
            {
                text: 'Редактировать информацию',
                tooltip: 'Отображает диалог редактирования выбранного пользователя',
                handler: function () {
                    var selected = Ext.getCmp('usersGrid').getSelectionModel().getSelected();
                    if (selected != null) {
                        showUserDialog(selected);
                    }
                }
            },
            {
                text: 'Редактировать товары',
                tooltip: 'Отображает диалог редактирования товаров пользователя',
                handler: function () {
                    var selected = Ext.getCmp('usersGrid').getSelectionModel().getSelected();
                    if (selected != null) {
                        showProductsDialog(selected);
                    }
                }
            },
            {
                text: 'Редактировать тендеры',
                tooltip: 'Отображает диалог редактирования тендеров пользователя',
                handler: function () {
                    var selected = Ext.getCmp('usersGrid').getSelectionModel().getSelected();
                    if (selected != null) {
                        showTendersDialog(selected);
                    }
                }
            },
        ],
        bbar: new Ext.PagingToolbar({
            pageSize: 100,
            displayInfo: true,
            store: store,
            displayMsg: 'Отображает пользователя {0} - {1} из {2}',
            emptyMsg: "Нет пользователей для отображения",
        }),
        listeners: {
            rowdblclick: function (number, e) {
                var selected = Ext.getCmp('usersGrid').getSelectionModel().getSelected();
                if (selected != null) {
                    showUserDialog(selected);
                }
            }
        }
    };

    container.add(usersGrid);
    container.doLayout();
    
    function loadAllUsers() {
        store.setBaseParam("type", "all");
        store.load({
            params: {
                start: 0,
                limit: 100
            }
        });
    }
    
    function loadPremiumUsers() {
        store.setBaseParam("type", "gold");
        store.load({
            params: {
                start: 0,
                limit: 100
            }
        });
    }

    loadAllUsers();

    /**
     * Отображает диалог создания или редактирования страницы
     * @param page
     */
    function showUserDialog(user) {
        var wnd = new Ext.Window({
            title: "Редактирование пользователя " + user.data.login,
            resizable: true,
            width: 550,
            height: 700,
            modal: true,
            autoScroll: true,
            items: [
                {
                    xtype: 'form',
                    bodyStyle: 'padding: 5px',
                    id: 'userForm',
                    labelAlign: 'top',
                    items: [
                        {
                            xtype: 'hidden',
                            name: 'id',
                            value: user.data.id
                        },
                        {
                            xtype: 'fieldset',
                            defaultType: 'textfield',
                            title: 'Основная информация',
                            anchor: '100%',
                            items: [
                                {
                                    fieldLabel: 'Логин',
                                    name: 'login',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: user.data.login
                                },
                                {
                                    fieldLabel: 'Компания',
                                    name: 'company',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: user.data.company
                                },
                                {
                                    fieldLabel: 'Домен',
                                    name: 'subdomain',
                                    anchor: '100%',
                                    allowBlank: true,
                                    value: user.data.subdomain
                                },
                                {
                                    fieldLabel: 'Бренд',
                                    name: 'brand',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: user.data.brand
                                },
                                {
                                    fieldLabel: 'ФИО',
                                    name: 'fio',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.fio
                                },
                                {
                                    fieldLabel: 'Email',
                                    name: 'email',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.email
                                },
                                {
                                    fieldLabel: 'ICQ',
                                    name: 'ICQ',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.icq
                                },
                                {
                                    fieldLabel: 'Skype',
                                    name: 'skype',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.skype
                                },
                                {
                                    fieldLabel: 'Url',
                                    name: 'url',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.url
                                }
                            ]
                        },
                        {
                            xtype: 'fieldset',
                            defaultType: 'textfield',
                            title: 'Местоположение',
                            anchor: '100%',
                            items: [
                                {
                                    fieldLabel: 'Страна',
                                    name: 'country',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: user.data.country
                                },
                                {
                                    fieldLabel: 'Регион',
                                    name: 'region',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.region
                                },
                                {
                                    fieldLabel: 'Город',
                                    name: 'city',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.city
                                },
                                {
                                    fieldLabel: 'Адрес',
                                    name: 'address',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.address
                                },
                                {
                                    fieldLabel: 'Почтовый индекс',
                                    name: 'zip',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.zip
                                }
                            ]
                        },
                        {
                            xtype: 'textarea',
                            name: 'about',
                            fieldLabel: 'О компании',
                            allowBlank: true,
                            anchor: "100%",
                            value: user.data.about
                        },
                        {
                            xtype: 'fieldset',
                            title: 'Род деятельности',
                            anchor: '100%',
                            items: [
                                {
                                    xtype: 'compositefield',
                                    hideLabel: true,
                                    anchor: '100%',
                                    items: [
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'Импортер',
                                            name: 'importer',
                                            checked: user.data.importer,
                                            flex: 1
                                        },
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'OEM',
                                            name: 'oem',
                                            checked: user.data.oem,
                                            flex: 1
                                        },
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'Оптовик',
                                            name: 'whoseller',
                                            checked: user.data.whoseller,
                                            flex: 1
                                        },
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'ODM',
                                            name: 'ODM',
                                            checked: user.data.ODM,
                                            flex: 1
                                        }
                                    ]
                                },
                                {
                                    xtype: 'compositefield',
                                    hideLabel: true,
                                    anchor: '100%',
                                    items: [
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'Розничный продавец',
                                            name: 'singleSeller',
                                            checked: user.data.singleSeller,
                                            flex: 1
                                        },
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'Производитель',
                                            name: 'developer',
                                            checked: user.data.developer,
                                            flex: 1
                                        },
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'Агент',
                                            name: 'agent',
                                            checked: user.data.agent,
                                            flex: 1
                                        },
                                        {
                                            xtype: 'checkbox',
                                            hideLabel: true,
                                            boxLabel: 'Дистрибьютор',
                                            name: 'distributor',
                                            checked: user.data.distributor,
                                            flex: 1
                                        }
                                    ]
                                }
                            ]
                        },
                        {
                            xtype: 'fieldset',
                            defaultType: 'textfield',
                            title: 'Реквизиты',
                            anchor: '100%',
                            items: [
                                {
                                    fieldLabel: 'ИНН',
                                    name: 'inn',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.inn
                                },
                                {
                                    fieldLabel: 'ОГРН',
                                    name: 'ogrn',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.ogrn
                                },
                                {
                                    fieldLabel: 'КПП',
                                    name: 'kpp',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.kpp
                                },
                                {
                                    fieldLabel: 'Номер рассчетного счета',
                                    name: 'accountRnumber',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.accountRnumber
                                },
                                {
                                    fieldLabel: 'Корреспонденсткий счет',
                                    name: 'accountKnumber',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.accountKnumber
                                },
                                {
                                    fieldLabel: 'Банк',
                                    name: 'bank',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.bank
                                },
                                {
                                    fieldLabel: 'БИК Банка',
                                    name: 'bankBik',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: user.data.bankBik
                                }
                            ]
                        },
                        {
                            xtype: 'textarea',
                            name: 'dealer',
                            fieldLabel: 'Дилер',
                            allowBlank: true,
                            anchor: "100%",
                            value: user.data.dealer
                        }
                    ]
                }
                
            ],
            buttons: [
                {
                    text: 'Сохранить',
                    handler: function () {
                        // Валидируем
                        if (!Ext.getCmp('userForm').getForm().isValid()) {
                            Ext.Msg.alert('Ошибка', 'Пожалуйста, правильно заполните все поля формы');
                            return;
                        }

                        Ext.getCmp('userForm').getForm().submit({
                            url: '/ControlPanel/ManageUsers/SaveUser',
                            success: function () {
                                var lastOptions = store.lastOptions;
                                store.reload(lastOptions);
                                wnd.close();
                            },
                            failure: function() {
                                var lastOptions = store.lastOptions;
                                store.reload(lastOptions);
                                wnd.close();
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
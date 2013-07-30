/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var allCategoriesStore = new Ext.data.Store({
        url: '/ControlPanel/ManageCategories/GetAllCategories',
        root: 'data',
        idProperty: 'id',
        autoLoad: 'true',
        sortInfo: { field: 'id', direction: "ASC" },
        reader: new Ext.data.JsonReader({
            root: 'data',
            fields: ['id', 'title']
        }),
        listeners: {
                                                        
        }
    });
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
        ],
        listeners: {
            beforeload: function () {
                Ext.getCmp("usersGrid").el.mask("Идет загрузка","x-mask-loading");
            },
            load: function () {
                Ext.getCmp("usersGrid").el.unmask();
            }
        }
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
    
    function showProductsDialog(user) {
        var wnd = new Ext.Window({
            title: "Товары пользователя " + user.data.login,
            resizable: true,
            width: 550,
            height: 700,
            modal: true,
            autoScroll: true,
            layout: 'fit',
            items: [
                {
                    xtype: 'grid',
                    frame: true,
                    store: new Ext.data.JsonStore({
                        root: 'data',
                        idProperty: 'id',
                        fields: [
                            'id',
                            'title',
                            'keywords',
                            'description',
                            'img',
                            'date',
                            'field1',
                            'field2',
                            'field3',
                            'field4',
                            'field5',
                            'field6',
                            'field7',
                            'field8',
                            'field9',
                            'price',
                            'currency',
                            'measure',
                            'minimumLotSize',
                            'minimumLotMeasure',
                            'vendorCountry',
                            'deliveryTime',
                            'deliveryPossibilityDay',
                            'deliveryPossibilityMeasure',
                            'deliveryPossibilityTime',
                            'productCode',
                            'productBox',
                            'userCategoryId',
                            'categoryId',
                            'userId',
                            'categoryName',
                            'userCategoryName'
                        ]
                    }),
                    height: 400,
                    autoExpandColumn: 'productTitleColumn',
                    id: 'userProductsGrid',
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
                                id: 'productIdColumn'
                            },
                            {
                                header: 'Название',
                                dataIndex: 'title',
                                sortable: true,
                                width: 150,
                                align: 'left',
                                id: 'productTitleColumn'
                            },
                            {
                                header: 'Категория',
                                dataIndex: 'categoryName',
                                sortable: true,
                                width: 100,
                                align: 'left',
                                id: 'productCategoryColumn'
                            },
                            {
                                header: 'Пользовательская категоря',
                                dataIndex: 'userCategoryName',
                                sortable: true,
                                width: 100,
                                align: 'left',
                                id: 'productUserCategoryColumn'
                            }
                        ]
                    }),
                    tbar: [
                        {
                            text: 'Добавить товар',
                            handler: function () {
                                showProductDialog(user.data.id);
                            }
                        },
                        {
                            text: 'Редактировать товар',
                            handler: function () {
                                var selected = Ext.getCmp('userProductsGrid').getSelectionModel().getSelected();
                                if (selected != null) {
                                    showProductDialog(user.data.id,selected);
                                }
                            }
                        },
                        {
                            text: 'Удалить товар',
                            handler: function () {
                                var selectedRecord = Ext.getCmp('userProductsGrid').getSelectionModel().getSelected();
                                if (selectedRecord == null) {
                                    return;
                                }
                                Ext.Msg.show({
                                    title: 'Удаление товара',
                                    msg: 'Вы действительно хотите удалить выбранный товар?',
                                    buttons: Ext.Msg.YESNO,
                                    icon: Ext.Msg.QUESTION,
                                    fn: function (txt) {
                                        if (txt == "yes") {
                                            var id = selectedRecord.data.id;
                                            global.Ajax({
                                                url: '/ControlPanel/ManageUsers/DeleteProduct',
                                                params: {
                                                    id: id
                                                },
                                                maskEl: Ext.getCmp('usersProductsGrid'),
                                                maskMsg: 'Идет удаление товара',
                                                success: function (data) {
                                                    reloadProducts(user.data.id);
                                                }
                                            });
                                        }
                                    }
                                });
                            }
                        },
                    ],
                    listeners: {
                        rowdblclick: function (number, e) {
                            var selected = Ext.getCmp('userProductsGrid').getSelectionModel().getSelected();
                            if (selected != null) {
                                showProductDialog(user.data.id, selected);
                            }
                        }
                    }
                }
            ],
            buttons: [
                {
                    text: 'Закрыть',
                    handler: function () {
                        wnd.close();
                    }
                }
            ],
            listeners: {
                show: function() {
                    reloadProducts(user.data.id);
                }
            }
        });
        wnd.show();
    }
    
    function reloadProducts(userId) {
        global.Ajax({
            url: '/ControlPanel/ManageUsers/GetProducts',
            maskEl: Ext.getCmp('userProductsGrid'),
            params: {
                userId: userId
            },
            maskMsg: 'Идет загрузка списка товаров',
            success: function (data) {
                Ext.getCmp('userProductsGrid').getStore().loadData(data);
            }
        });
    }
    
    function showProductDialog(userId,product) {
        var wnd = new Ext.Window({
            title: product != null ? "Редактирование товара "+product.data.title:"Создание товара",
            resizable: true,
            width: 550,
            height: 700,
            modal: true,
            autoScroll: true,
            items: [
                {
                    xtype: 'form',
                    bodyStyle: 'padding: 5px',
                    id: 'productForm',
                    labelAlign: 'top',
                    items: [
                        {
                            xtype: 'hidden',
                            name: 'id',
                            value: product != null ? product.data.id : -1
                        },
                        {
                            xtype: 'hidden',
                            name: 'userId',
                            value: product != null ? product.data.userId : userId
                        },
                        {
                            xtype: 'hidden',
                            name: 'userCategoryId',
                            value: product != null ? product.data.userCategoryId : -1
                        },
                        {
                            xtype: 'fieldset',
                            defaultType: 'textfield',
                            title: 'Характеристики',
                            anchor: '100%',
                            items: [
                                {
                                    fieldLabel: 'Наименование',
                                    name: 'title',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: product != null ? product.data.title : ""
                                },
                                {
                                    fieldLabel: 'Ключевые слова',
                                    name: 'keywords',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: product != null ? product.data.keywords : ""
                                },
                                {
                                    xtype: 'textarea',
                                    fieldLabel: 'Описание',
                                    name: 'description',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: product != null ? product.data.description : ""
                                },
                                {
                                    xtype         : 'combo',
                                    valueField    : 'id',
                                    displayField  : 'title',
                                    width         : 250,
                                    mode          : 'local',
                                    triggerAction : 'all',
                                    editable      : false,
                                    lazyInit: false,
                                    anchor: "100%",
                                    fieldLabel: 'Категория',
                                    hiddenName: 'categoryId',
                                    store         : allCategoriesStore,
                                    value: product != null ? product.data.id : 1
                                },
                                {
                                    fieldLabel: 'Цена',
                                    name: 'price',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: product != null ? product.data.price : ""
                                },
                                {
                                    fieldLabel: 'Валюта',
                                    name: 'currency',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: product != null ? product.data.currency : "RUB"
                                },
                                {
                                    fieldLabel: 'Мера',
                                    name: 'measure',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.currency : "шт"
                                },
                                {
                                    fieldLabel: 'Минимальная партия',
                                    name: 'minimumLotSize',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.minimumLotSize : ""
                                },
                                {
                                    fieldLabel: 'Размер минимальной партия',
                                    name: 'minimumLotMeasure',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.minimumLotMeasure : ""
                                },
                                {
                                    fieldLabel: 'Страна производитель',
                                    name: 'vendorCountry',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.vendorCountry : ""
                                },
                                {
                                    fieldLabel: 'Срок поставки',
                                    name: 'deliveryTime',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.deliveryTime : ""
                                },
                                {
                                    fieldLabel: 'Возможность поставки - за день',
                                    name: 'deliveryPossibilityDay',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.deliveryPossibilityDay : ""
                                },
                                {
                                    fieldLabel: 'Возможность поставки - мера',
                                    name: 'deliveryPossibilityMeasure',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.deliveryPossibilityMeasure : ""
                                },
                                {
                                    fieldLabel: 'Возможность поставки - время',
                                    name: 'deliveryPossibilityTime',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.deliveryPossibilityTime : ""
                                },
                                {
                                    fieldLabel: 'Артикул товара',
                                    name: 'productCode',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.productCode : ""
                                },
                                {
                                    fieldLabel: 'Упаковка',
                                    name: 'productBox',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.productBox : ""
                                }
                            ]
                        },
                        {
                            xtype: 'fieldset',
                            defaultType: 'textfield',
                            title: 'Старые параметры',
                            anchor: '100%',
                            items: [
                                {
                                    fieldLabel: 'Свойство 1',
                                    name: 'field1',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field1 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 2',
                                    name: 'field2',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field2 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 3',
                                    name: 'field3',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field3 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 4',
                                    name: 'field4',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field4 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 5',
                                    name: 'field5',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field5 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 6',
                                    name: 'field6',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field6 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 7',
                                    name: 'field7',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field7 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 8',
                                    name: 'field2',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field8 : ""
                                },
                                {
                                    fieldLabel: 'Свойство 9',
                                    name: 'field9',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: product != null ? product.data.field9 : ""
                                }
                            ]
                        }
                    ]
                }
            ],
            buttons: [
                {
                    text: 'Сохранить',
                    handler: function () {
                        // Валидируем
                        if (!Ext.getCmp('productForm').getForm().isValid()) {
                            Ext.Msg.alert('Ошибка', 'Пожалуйста, правильно заполните все поля формы');
                            return;
                        }

                        Ext.getCmp('productForm').getForm().submit({
                            url: '/ControlPanel/ManageUsers/SaveProduct',
                            success: function () {
                                wnd.close();
                                reloadProducts(userId);
                            },
                            failure: function () {
                                wnd.close();
                                reloadProducts(userId);
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

    function showTendersDialog(user) {
        var wnd = new Ext.Window({
            title: "Тендеры пользователя " + user.data.login,
            resizable: true,
            width: 550,
            height: 700,
            modal: true,
            autoScroll: true,
            layout: 'fit',
            items: [
                {
                    xtype: 'grid',
                    frame: true,
                    store: new Ext.data.JsonStore({
                        root: 'data',
                        idProperty: 'id',
                        fields: [
                            'id',
                            'userId',
                            'categoryId',
                            'categoryName',
                            'title',
                            'description',
                            'minprice',
                            'maxprice',
                            'currency',
                            'size',
                            'measure',
                            'dateStart',
                            'dateEnd',
                            'period',
                            'image',
                            'keywords'
                        ]
                    }),
                    height: 400,
                    autoExpandColumn: 'tenderTitleColumn',
                    id: 'userTendersGrid',
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
                                id: 'tenderIdColumn'
                            },
                            {
                                header: 'Название',
                                dataIndex: 'title',
                                sortable: true,
                                width: 150,
                                align: 'left',
                                id: 'tenderTitleColumn'
                            },
                            {
                                header: 'Категория',
                                dataIndex: 'categoryName',
                                sortable: true,
                                width: 100,
                                align: 'left',
                                id: 'tenderCategoryColumn'
                            }
                        ]
                    }),
                    tbar: [
                        {
                            text: 'Добавить тендер',
                            handler: function () {
                                showTenderDialog(user.data.id);
                            }
                        },
                        {
                            text: 'Редактировать тендер',
                            handler: function () {
                                var selected = Ext.getCmp('userTendersGrid').getSelectionModel().getSelected();
                                if (selected != null) {
                                    showTenderDialog(user.data.id,selected);
                                }
                            }
                        },
                        {
                            text: 'Удалить тендер',
                            handler: function () {
                                var selectedRecord = Ext.getCmp('userTendersGrid').getSelectionModel().getSelected();
                                if (selectedRecord == null) {
                                    return;
                                }
                                Ext.Msg.show({
                                    title: 'Удаление тендера',
                                    msg: 'Вы действительно хотите удалить выбранный тендер?',
                                    buttons: Ext.Msg.YESNO,
                                    icon: Ext.Msg.QUESTION,
                                    fn: function (txt) {
                                        if (txt == "yes") {
                                            var id = selectedRecord.data.id;
                                            global.Ajax({
                                                url: '/ControlPanel/ManageUsers/DeleteTender',
                                                params: {
                                                    id: id
                                                },
                                                maskEl: Ext.getCmp('userTendersGrid'),
                                                maskMsg: 'Идет удаление тендера',
                                                success: function (data) {
                                                    reloadTenders(user.data.id);
                                                }
                                            });
                                        }
                                    }
                                });
                            }
                        },
                    ],
                    listeners: {
                        rowdblclick: function (number, e) {
                            var selected = Ext.getCmp('userTendersGrid').getSelectionModel().getSelected();
                            if (selected != null) {
                                showTenderDialog(user.data.id, selected);
                            }
                        }
                    }
                }
            ],
            buttons: [
                {
                    text: 'Закрыть',
                    handler: function () {
                        wnd.close();
                    }
                }
            ],
            listeners: {
                show: function() {
                    reloadTenders(user.data.id);
                }
            }
        });
        wnd.show();
    }
    
    function reloadTenders(userId) {
        global.Ajax({
            url: '/ControlPanel/ManageUsers/GetTenders',
            maskEl: Ext.getCmp('userTendersGrid'),
            params: {
                userId: userId
            },
            maskMsg: 'Идет загрузка списка тендеров',
            success: function (data) {
                Ext.getCmp('userTendersGrid').getStore().loadData(data);
            }
        });
    }
    
    function showTenderDialog(userId,tender) {
        var wnd = new Ext.Window({
            title: tender != null ? "Редактирование тендера "+tender.data.title:"Создание тендера",
            resizable: true,
            width: 550,
            height: 700,
            modal: true,
            autoScroll: true,
            items: [
                {
                    xtype: 'form',
                    bodyStyle: 'padding: 5px',
                    id: 'tenderForm',
                    labelAlign: 'top',
                    items: [
                        {
                            xtype: 'hidden',
                            name: 'id',
                            value: tender != null ? tender.data.id : -1
                        },
                        {
                            xtype: 'hidden',
                            name: 'userId',
                            value: tender != null ? tender.data.userId : userId
                        },
                        {
                            xtype: 'fieldset',
                            defaultType: 'textfield',
                            title: 'Характеристики',
                            anchor: '100%',
                            items: [
                                {
                                    fieldLabel: 'Наименование',
                                    name: 'title',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.title : ""
                                },
                                {
                                    fieldLabel: 'Ключевые слова',
                                    name: 'keywords',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.keywords : ""
                                },
                                {
                                    xtype: 'textarea',
                                    fieldLabel: 'Описание',
                                    name: 'description',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.description : ""
                                },
                                {
                                    xtype         : 'combo',
                                    valueField    : 'id',
                                    displayField  : 'title',
                                    width         : 250,
                                    mode          : 'local',
                                    triggerAction : 'all',
                                    editable      : false,
                                    lazyInit: false,
                                    anchor: "100%",
                                    fieldLabel: 'Категория',
                                    hiddenName: 'categoryId',
                                    store         : allCategoriesStore,
                                    value: tender != null ? tender.data.id : 1
                                },
                                {
                                    fieldLabel: 'Цена от',
                                    name: 'minprice',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.minprice : 0
                                },
                                {
                                    fieldLabel: 'Цена до',
                                    name: 'maxprice',
                                    allowBlank: false,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.maxprice : 0
                                },
                                {
                                    fieldLabel: 'Валюта',
                                    name: 'currency',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.currency : "RUB"
                                },
                                {
                                    fieldLabel: 'Объем',
                                    name: 'size',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.size : ""
                                },
                                {
                                    fieldLabel: 'Мера',
                                    name: 'measure',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.measure : ""
                                },
                                {
                                    xtype: 'datefield',
                                    fieldLabel: 'Дата начала',
                                    name: 'dateStart',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.dateStart : null
                                },
                                {
                                    xtype: 'datefield',
                                    fieldLabel: 'Дата конца',
                                    name: 'dateEnd',
                                    allowBlank: true,
                                    anchor: '100%',
                                    value: tender != null ? tender.data.dateEnd : null
                                }
                            ]
                        }
                    ]
                }
            ],
            buttons: [
                {
                    text: 'Сохранить',
                    handler: function () {
                        // Валидируем
                        if (!Ext.getCmp('tenderForm').getForm().isValid()) {
                            Ext.Msg.alert('Ошибка', 'Пожалуйста, правильно заполните все поля формы');
                            return;
                        }

                        Ext.getCmp('tenderForm').getForm().submit({
                            url: '/ControlPanel/ManageUsers/SaveTender',
                            success: function () {
                                wnd.close();
                                reloadTenders(userId);
                            },
                            failure: function () {
                                wnd.close();
                                reloadTenders(userId);
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
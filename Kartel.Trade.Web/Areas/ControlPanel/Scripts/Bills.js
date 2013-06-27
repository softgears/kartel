/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var billsGrid = {
        xtype: 'grid',
        frame: true,
        store: new Ext.data.GroupingStore({
            root: 'data',
            idProperty: 'name',
            groupField: 'payedTitle',
            storeId: 'billsStore',
            sortInfo: { field: 'id', direction: "ASC" },
            reader: new Ext.data.JsonReader({
                root: 'data',
                fields: [
                    'id',
                    'user',
                    'amount',
                    'payed',
                    'payedTitle',
                    'activationTarget',
                    'activationAmount',
                    'activationTargetId',
                    'activated',
                    'dateCreated',
                    'dateModified'
                ]
            })
        }),
        height: 400,
        autoExpandColumn: 'billsUserColumn',
        id: 'billsGrid',
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
                    id: 'billsIdColumn'
                },
                {
                    header: 'Пользователь',
                    dataIndex: 'user',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'billsUserColumn'
                },
                {
                    header: 'Сумма',
                    dataIndex: 'amount',
                    sortable: true,
                    width: 100,
                    align: 'left',
                    id: 'billsAmountColumn'
                },
                {
                    header: 'Оплачен',
                    dataIndex: 'payedTitle',
                    sortable: true,
                    width: 100,
                    align: 'left',
                    id: 'billsPayedTitleColumn',
                    hidden: true
                },
                {
                    header: 'Услуга',
                    dataIndex: 'activationTarget',
                    sortable: true,
                    width: 180,
                    align: 'left',
                    id: 'billsActivationTargetColumn'
                },
                {
                    header: 'Период/Количество',
                    dataIndex: 'activationAmount',
                    sortable: true,
                    width: 180,
                    align: 'left',
                    id: 'billsActivationAmountColumn'
                },
                {
                    header: 'Дата создания',
                    dataIndex: 'dateCreated',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'billsDateCreatedColumn'
                },
                {
                    header: 'Дата редактирования',
                    dataIndex: 'dateModified',
                    sortable: true,
                    width: 150,
                    align: 'left',
                    id: 'billsDateModifiedColumn'
                }
            ]
        }),
        tbar: [
            {
                text: 'Обновить список',
                handler: function () {
                    reloadBills();
                }
            },
            '-',
            {
                text: 'Информация',
                tooltip: 'Отображает диалог просмотра информации о счете',
                handler: function () {
                    var selected = Ext.getCmp('billsGrid').getSelectionModel().getSelected();
                    if (selected != null) {
                        showBillDialog(selected);
                    }
                }
            }
        ],
        listeners: {
            rowdblclick: function (number, e) {
                var selected = Ext.getCmp('billsGrid').getSelectionModel().getSelected();
                if (selected != null) {
                    showBillDialog(selected);
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

    container.add(billsGrid);
    container.doLayout();

    /**
     * Перезагружает список счетов
     */
    function reloadBills() {
        global.Ajax({
            url: '/ControlPanel/ManageBills/GetBills',
            maskEl: Ext.getCmp('billsGrid'),
            maskMsg: 'Идет загрузка списка счетов',
            success: function (data) {
                Ext.getCmp('billsGrid').getStore().loadData(data);
            }
        });
    }
    reloadBills();

    /**
     * Отображает диалог создания или редактирования страницы
     * @param page
     */
    function showBillDialog(bill) {
        var wnd = new Ext.Window({
            title: "Просмотр информации о счете №"+bill.data.id,
            resizable: true,
            width: 550,
            modal: true,
            items: [
                {
                    xtype: 'form',
                    bodyStyle: 'padding: 5px',
                    id: 'billForm',
                    labelAlign: 'top',
                    items: [
                        {
                            xtype: 'hidden',
                            id: 'billIdField',
                            value: bill != undefined ? bill.data.id : -1
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Пользователь',
                            id: 'billUserField',
                            anchor: '100%',
                            value: bill != undefined ? bill.data.user : ""
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Сумма',
                            id: 'billAmountField',
                            anchor: '100%',
                            value: bill != undefined ? bill.data.amount : ""
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Услуга',
                            id: 'billActivationTargetField',
                            anchor: '100%',
                            value: bill != undefined ? bill.data.activationTarget : ""
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Количество',
                            id: 'billActivationAmountField',
                            anchor: '100%',
                            value: bill != undefined ? bill.data.activationAmount : ""
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false,
                            fieldLabel: 'Цель активации услуги',
                            id: 'billActivationTargetIdField',
                            anchor: '100%',
                            value: bill != undefined ? bill.data.activationTargetId : ""
                        }
                    ]
                }
            ],
            buttons: [
                {
                    text: 'Оплатить и активировать',
                    handler: function () {
                        global.Ajax({
                            url: '/ControlPanel/ManageBills/ActivateBill',
                            params: {
                                id: bill.data.id
                            },
                            maskEl: Ext.getCmp('billsGrid'),
                            maskMsg: 'Идет активация счета',
                            success: function (data) {
                                reloadBills();
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
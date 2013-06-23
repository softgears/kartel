/**
* Скрипт отображающий панель управления категориями системы
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var categoriesTree = {
        xtype: 'treepanel',
        frame: false,
        height: 400,
        rootVisible: true,
        id: 'categoriesTree',
        autoScroll: true,
        animate: true,
        loader: new Ext.tree.TreeLoader({
            url: '/ControlPanel/ManageCategories/GetCategories',
            requestMethod: 'GET'
        }),
        root: new Ext.tree.AsyncTreeNode({
            id: -1,
            text: 'Корень',
            expanded: true
        }),
        tbar: [
            {
                text: 'Добавить',
                tooltip: 'Отображает диалог добавления новой категории',
                handler: function () {
                    showCategoryDialog();
                }
            },
            {
                text: 'Редактировать',
                tooltip: 'Отображает диалог редактирования выбранной категории',
                handler: function () {
                    var selectedNode = Ext.getCmp('categoriesTree').getSelectionModel().getSelectedNode();
                    if (selectedNode != null && selectedNode.id != -1) {
                        showCategoryDialog(selectedNode.attributes.category);
                    }
                }
            },
            {
                text: 'Удалить',
                tooltip: 'Удаляет категорию и все ее дочерние категории',
                handler: function () {
                    var selectedNode = Ext.getCmp('categoriesTree').getSelectionModel().getSelectedNode();
                    if (selectedNode == null || selectedNode.id == -1) {
                        return;
                    }
                    Ext.Msg.show({
                        title: 'Удаление категории',
                        msg: 'Вы действительно хотите удалить выбранную категорию и все вложенные в нее подкатегории?',
                        buttons: Ext.Msg.YESNO,
                        icon: Ext.Msg.QUESTION,
                        fn: function (txt) {
                            if (txt == "yes") {
                                var id = selectedNode.id;
                                global.Ajax({
                                    url: '/ControlPanel/ManageCategories/Delete',
                                    maskEl: Ext.getCmp('categoriesTree'),
                                    maskMsg: 'Идет удаление категории',
                                    params: { id: id },
                                    success: function (data) {
                                        selectedNode.remove();
                                    }
                                });
                            }
                        }
                    });
                }
            }
        ],
        enableDD: true,
        listeners: {
            dblclick: function (node, e) {
                if (node.id != -1) {
                    showCategoryDialog(node.attributes.category);
                }
            },
            movenode: function (tree, node, oldParent, newParent, index) {
                if (oldParent.id != newParent.id) {
                    global.Ajax({
                        url: '/ControlPanel/ManageCategories/Move',
                        maskEl: Ext.getCmp('categoriesTree'),
                        maskMsg: 'Идет перемещение категории',
                        params: { id: node.id, newParentId: newParent.id, index: index },
                        success: function (data) {
                            node.attributes.category.parentId = newParent.id;
                        }
                    });
                }
            }
        }
    };


    container.add(categoriesTree);
    container.doLayout();

    /**
     * Отображает диалог создания или редактирования категории
     * @param category - идентификатор категории, для новой = -1
     */
    function showCategoryDialog(category) {
        var wnd = new Ext.Window({
            title: category != undefined ? "Редактирование категории " + category.displayName : "Новая категория",
            resizable: true,
            width: 350,
            layout: 'fit',
            modal: true,
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 0,
                    height: 400,
                    items: [
                        {
                            xtype: 'form',
                            name: 'categoryForm',
                            title: 'Свойства категории',
                            id: 'categoryForm',
                            labelAlign: 'top',
                            bodyStyle: 'padding: 5px',
                            items: [
                                {
                                    xtype: 'hidden',
                                    id: 'categoryIdField',
                                    value: category != undefined ? category.id : -1
                                },
                                {
                                    xtype: 'textfield',
                                    fieldLabel: 'Отображаемое имя',
                                    allowBlank: false,
                                    id: 'categoryDisplayNameField',
                                    value: category != undefined ? category.displayName : "",
                                    anchor: '100%'
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
                        // Валидация формы
                        if (!Ext.getCmp('categoryForm').getForm().isValid()) {
                            Ext.Msg.alert('Ошибка заполнение формы', 'Пожалуйста, правильно заполните форму');
                            return;
                        }
                        // подготавливаем данные к отправке
                        var selectedNode = Ext.getCmp('categoriesTree').getSelectionModel().getSelectedNode();
                        var parentId = selectedNode != null ? selectedNode.id : -1;
                        var params = {
                            id: Ext.getCmp('categoryIdField').getValue(),
                            parentId: parentId,
                            displayName: Ext.getCmp('categoryDisplayNameField').getValue(),
                        };

                        // Отправляем данные
                        global.Ajax({
                            url: '/ControlPanel/ManageCategories/Save',
                            maskEl: Ext.getCmp('categoriesTree'),
                            maskMsg: 'Идет сохранение категории',
                            params: params,
                            success: function (data) {
                                wnd.close();
                                // Добавляем данные в дерево либо обновляем изменившиеся данные
                                if (category == undefined) {
                                    // Создаем новый узел
                                    var parentNode = selectedNode != null ? selectedNode : Ext.getCmp('categoriesTree').root;
                                    var node = {
                                        id: data.data.id,
                                        text: params.displayName,
                                        qtip: params.description,
                                        leaf: true,
                                        category: params
                                    };
                                    parentNode.appendChild(node);
                                    parentNode.leaf = false;
                                }
                                else {
                                    // Обновляем изменения в существующем узле
                                    selectedNode.setText(params.displayName);
                                    selectedNode.setTooltip(params.description);
                                    selectedNode.attributes.category = params;
                                }
                            }
                        });
                    }
                },
                {
                    text: 'Отмена',
                    handler: function () {
                        wnd.close();
                    }
                }
            ]

        });
        wnd.show();
    }
});
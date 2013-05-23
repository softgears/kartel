/**
* Основной корневой файл скриптов админской панели
*/

/**
* Основная панель администрирования в главном пространстве имен
*/
Ext.onReady(function () {
    Ext.QuickTips.init();

    Ext.controlPanel = new Ext.Viewport({
        title: 'Панель управления сайтом',
        id: 'adminPanel',
        layout: 'border',
        items: [
            {
                xtype: 'treepanel',
                region: 'west',
                width: 300,
                title: 'Панель навигации',
                loader: new Ext.tree.TreeLoader({
                    url: '/ControlPanel/Root/GetNavMenu'
                }),
                root: new Ext.tree.AsyncTreeNode({
                    text: 'Корень навигации',
                    id: -1,
                    expanded: true
                }),
                rootVisible: true,
                animate: true,
                autoScroll: true,
                listeners: {
                    click: function(node) {
                        if (node.attributes.objectType != "section") {
                            return;
                        }
                        var tabPanel = Ext.getCmp('tabPanel');
                        var searchTab = null, tabIndex = -1;
                        for (var i = 0; i < tabPanel.items.getCount(); i++) {
                            var tab = tabPanel.items.get(i);
                            if (tab.name == node.id) {
                                searchTab = tab;
                                tabIndex = i;
                                break;
                            }
                        }
                        if (searchTab != null) {
                            tabPanel.setActiveTab(tabIndex);
                            return;
                        }
                        searchTab = {
                            xtype: 'panel',
                            title: node.text,
                            id: node.id,
                            closable: true,
                            tabTip: node.qtip,
                            layout: 'fit',
                            autoLoad: { url: node.attributes.url, scripts: true }
                        };
                        tabPanel.add(searchTab).show();
                    }
                }
            },
            {
                xtype: 'tabpanel',
                id: 'tabPanel',
                region: 'center'
            }
        ],
        renderTo: Ext.getBody()
    });
});
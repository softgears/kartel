/**
* Скрипт отображающий панель управления статистическими страницами
*/
var container = Ext.getCmp(sectionId);
Ext.onReady(function () {
    var settingsPanel = {
        xtype: 'panel',
        frame: true,
        title: 'Настройки сайта',
        autoScroll: true,
        items: [
            {
                xtype: 'form',
                id: 'settingsForm',
                bodyStyle: 'padding: 5px',
                labelAlign: 'top',
                items: []
            }
        ],
        buttons: [
            {
                text: 'Сохранить изменения',
                handler: function () {
                    Ext.getCmp('settingsForm').getForm().submit({
                        url: '/ControlPanel/ManageSettings/Save',
                        clientValidation: true,
                        success: function (form, action) {
                            // Успешно сохранили
                            reloadSettings();
                        },
                        failure: function (form, action) {
                            alert("FUCK");
                        }
                    });
                }
            }
        ]
    };

    container.add(settingsPanel);
    container.doLayout();

    /**
     * Перезагружает список страниц с сервера
     */
    function reloadSettings() {
        global.Ajax({
            url: '/ControlPanel/ManageSettings/GetSettings',
            maskEl: Ext.getCmp('settingsPanel'),
            maskMsg: 'Идет загрузка списка страниц',
            success: function (data) {
                Ext.getCmp('settingsForm').removeAll();
                for (var i =0; i < data.data.components.length; i++) {
                    Ext.getCmp('settingsForm').add(data.data.components[i]);
                }
                Ext.getCmp('settingsForm').doLayout();
            }
        });
    }
    reloadSettings();
});
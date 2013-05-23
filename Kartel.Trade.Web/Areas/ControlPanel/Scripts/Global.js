/**
 * Глобальный объект, имеющий в себе определенные функции доступные во всех модулях панели управления
 */
function Global(){

    /**
     * Выполняет аякс запрос с указанными конфигурационными параметрами
     * @param config - конфигурационный объект, имеющий следующие параметры:
     *          url - url по которому производить запрос
     *          params - параметры запрос
     *          success - callback функция, выполняющаяся в случае успешного запроса на сервер, в которую передаются пришедшие с сервера данные в json формате
     *          failure - callback функция, выполняющаяся в случае ошибки при запросе на сервер. Если не задана, то вызывается стандартный обработчик
     *          common - callback функция, выполняющаяся в любом случае после выполнения аякс запроса,
     *          method - метод аякс запроса, по умолчанию POST
     *          maskEl - указатель на компонент, на который накладывается маска на время выполнения запроса
     *          maskMsg - сообщение, которое устанавливается на маску
     */
    this.Ajax = function (config) {
        // устанавливаем маску на компонент если он задан
        var haveMask = config.maskEl != undefined && config.maskEl.el != undefined;
        if (haveMask) {
            config.maskEl.el.mask(config.maskMsg != undefined ? config.maskMsg : "Идет загрузка", "x-mask-loading");
        }
        // Выполняем запрос
        Ext.Ajax.request({
            method: config.method != undefined ? config.method : 'POST',
            url: config.url,
            params: config.params,
            success: function (response, request) {
                // Преобразуем данные
                var data = Ext.util.JSON.decode(response.responseText);

                // Прячем маску если нужно
                if (haveMask) {
                    config.maskEl.el.unmask();
                }

                // Выполняем общую функцию
                if (config.common != undefined) {
                    config.common(data);
                }

                // Выполняем конкретно успешную функцию
                if (data.success && config.success != undefined) {
                    config.success(data);
                } else if (!config.success) {
                    if (config.failure != undefined) {
                        config.failure(data);
                    } else {
                        Ext.Msg.alert('Ошибка', 'В ходе выполнения AJAX запроса возникла ошибка: ' + data.message);
                    }
                }
            },
            failure: function (response, request) {
                // Преобразуем данные
                var data = Ext.util.JSON.decode(response.responseText);

                // Прячем маску если нужно
                if (haveMask) {
                    config.maskEl.el.unmask();
                }

                // Выполняем общую функцию
                if (config.common != undefined) {
                    config.common(data);
                }

                // Выполняем конкретно неуспешнкю функцию
                if (config.failure != undefined) {
                    config.failure(data);
                }
                else {
                    Ext.Msg.alert('Ошибка на сервере', data.message);
                }
            }
        });
    };

    /**
     * Генерирует случайное число в указанном диапазоне
     * @param min - минимальное значение диапазона
     * @param max - максимальное значение диапазона
     */
    this.random = function random(min,max){
        return Math.floor(Math.random() * (max - min + 1)) + min;
    };
}
var global = new Global();
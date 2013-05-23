using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Classes
{
    /// <summary>
    /// Байндер на основе аттрибутов JSON свойств
    /// </summary>
    public class JsonModelBinder: DefaultModelBinder
    {
        /// <summary>
        /// Binds the specified property by using the specified controller context and binding context and the specified property descriptor.
        /// </summary>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param><param name="propertyDescriptor">Describes a property to be bound. The descriptor provides information such as the component type, property type, and property value. It also provides methods to get or set the property value.</param>
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var jsonAttribute = (JsonPropertyAttribute) propertyDescriptor.Attributes.Cast<Attribute>().FirstOrDefault(a => a is JsonPropertyAttribute);
            if (jsonAttribute != null)
            {
                var propertyName = jsonAttribute.PropertyName;
                var form = controllerContext.HttpContext.Request.Unvalidated().Form;
                if (form.AllKeys.Contains(propertyName) && propertyDescriptor.Converter != null)
                {
                    var propertyValue = form[propertyName];
                    if (propertyDescriptor.PropertyType == typeof(bool) || propertyDescriptor.PropertyType == typeof(bool?))
                    {
                        if (propertyValue != null && propertyValue == "on")
                        {
                            propertyValue = "true";
                        } else
                        {
                            propertyValue = "false";
                        }
                    }
                    propertyDescriptor.SetValue(bindingContext.Model, propertyDescriptor.Converter.ConvertFrom(propertyValue));
                }
            }
            else
            {
                base.BindProperty(controllerContext,bindingContext,propertyDescriptor);
            }
        }
    }

    /// <summary>
    /// Статический класс, содержащий расширения для контекста байндинга модели
    /// </summary>
    public static class ModelBindingContextExstension
    {
        /// <summary>
        /// Метод, получающий значение из провайдера значений
        /// </summary>
        /// <param name="bindingContext">контекст байндинга</param>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="performRequestValidation">Проводить ли валидацию запросов</param>
        /// <returns></returns>
        public static ValueProviderResult GetValueFromValueProvider(this ModelBindingContext bindingContext, string propertyName, bool performRequestValidation)
        {
            var unvalidatedValueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            return (unvalidatedValueProvider != null)
                       ? unvalidatedValueProvider.GetValue(bindingContext.ModelName, !performRequestValidation)
                       : bindingContext.ValueProvider.GetValue(propertyName);
        }
    }
}
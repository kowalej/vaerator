using System;
using System.Linq.Expressions;
using Xamarin.Forms;

namespace Vaerator.Helpers
{
    public static class BindablePropertyExtension
    {
        public static BindableProperty Create<TDeclarer, TPropertyType>(Expression<Func<TDeclarer, TPropertyType>> getter,
            TPropertyType defaultValue,
            BindingMode defaultBindingMode = BindingMode.OneWay,
            BindableProperty.ValidateValueDelegate<TPropertyType> validateValue = null,
            BindableProperty.BindingPropertyChangedDelegate<TPropertyType> propertyChanged = null,
            BindableProperty.BindingPropertyChangingDelegate<TPropertyType> propertyChanging = null,
            BindableProperty.CoerceValueDelegate<TPropertyType> coerceValue = null,
            BindableProperty.CreateDefaultValueDelegate<TDeclarer, TPropertyType> defaultValueCreator = null) where TDeclarer : BindableObject
        {
            return BindableProperty.Create(ExpressionExtension.GetPropertyPath(getter), typeof(TPropertyType), typeof(TDeclarer), defaultValue, defaultBindingMode,
                validateValue: (bindable, value) => { return validateValue != null ? validateValue(bindable, (TPropertyType)value) : true; },
                propertyChanged: (bindable, oldValue, newValue) => { if (propertyChanged != null) propertyChanged(bindable, (TPropertyType)oldValue, (TPropertyType)newValue); },
                propertyChanging: (bindable, oldValue, newValue) => { if (propertyChanging != null) propertyChanging(bindable, (TPropertyType)oldValue, (TPropertyType)newValue); },
                coerceValue: (bindable, value) => { return coerceValue != null ? coerceValue(bindable, (TPropertyType)value) : value; },
                defaultValueCreator: (bindable) => { return defaultValueCreator != null ? defaultValueCreator((TDeclarer)bindable) : defaultValue; });
        }
    }
}

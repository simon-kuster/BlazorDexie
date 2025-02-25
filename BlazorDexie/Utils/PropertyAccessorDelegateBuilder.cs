using System.Linq.Expressions;
using System.Reflection;

namespace BlazorDexie.Utils
{
    public static class PropertyAccessorDelegateBuilder
    {
        public static Func<object, object?> BuildPropertyGetter(PropertyInfo propertyInfo, bool nonPublic = false)
        {
            var itemType = propertyInfo.DeclaringType ?? throw new ArgumentException(
                $"{nameof(PropertyAccessorDelegateBuilder)}: No DeclaringType is set for {propertyInfo.Name}", nameof(propertyInfo));

            var getMethodInfo = propertyInfo.GetGetMethod(nonPublic) ?? throw new ArgumentException(
                $"{nameof(PropertyAccessorDelegateBuilder)}: No GetMethod found for property {itemType.Name}.{propertyInfo.Name}", nameof(propertyInfo));

            var itemParameter = Expression.Parameter(typeof(object), "item");
            var itemConvertExpresion = Expression.Convert(itemParameter, itemType);
            var getExpression = Expression.Call(itemConvertExpresion, getMethodInfo);
            var valueConvertExpression = Expression.Convert(getExpression, typeof(object));
            var lamda = Expression.Lambda(valueConvertExpression, itemParameter);

            return ((Expression<Func<object, object?>>)lamda).Compile();
        }
    }
}

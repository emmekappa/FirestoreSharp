namespace FirestoreSharp.Tests
{
    using System;
    using System.Linq.Expressions;

    public static class SelectorExpressionExtensions
    {
        public static TMember GetValue<TType, TMember>(this Expression<Func<TType, TMember>> selector,
            TType obj)
        {
            return selector.Compile().Invoke(obj);
        }

        public static void SetValue<TType, TMember>(this Expression<Func<TType, TMember>> selector,
            TType obj, TMember value)
        {
            var newValue = Expression.Parameter(selector.Body.Type);
            var assign = Expression.Lambda<Action<TType, TMember>>(
                Expression.Assign(selector.Body, newValue),
                selector.Parameters[0], newValue);
            var setter = assign.Compile();
            try
            {
                setter.Invoke(obj, value);
            }
            catch (NullReferenceException)
            {
                throw new InvalidOperationException(
                    $"Unable to set field value on \"{selector.Body}\": found a null value while traversing expression");
            }
        }
    }
}
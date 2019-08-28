using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Google.Cloud.Firestore;

namespace FirestoreSharp
{
    public static class FieldPathResolver
    {
        private static string GetPropertyName(MemberExpression body)
        {
            var firestorePropertyAttribute = body.Member.GetCustomAttribute<FirestorePropertyAttribute>();
            if (firestorePropertyAttribute == null)
                throw new InvalidOperationException($"Missing {nameof(FirestorePropertyAttribute)} on {body.Member}");
            var propertyName = string.IsNullOrEmpty(firestorePropertyAttribute.Name)
                ? body.Member.Name
                : firestorePropertyAttribute.Name;
            return propertyName;
        }
        
        public static string ResolveExpression(MemberExpression me)
        {
            var properties = new List<string>();
            while (me != null)
            {
                properties.Add(GetPropertyName(me));
                me = me.Expression as MemberExpression;
            }

            properties.Reverse();
            return string.Join(".", properties);
        }
    }
    public static class FieldPathResolver<TEntity>
    {
        public static string Resolve<TMember>(Expression<Func<TEntity, TMember>> selector)
        {
            if (!(selector.Body is MemberExpression))
                throw new InvalidOperationException("Property selector expected: x => x.Foo");

            var me = (MemberExpression) selector.Body;
            return FieldPathResolver.ResolveExpression(me);
        }
    }
}
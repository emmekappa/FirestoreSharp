namespace FirestoreSharp
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Google.Cloud.Firestore;

    public class TypedQueryWrapper<TEntity>
    {
        protected TypedQueryWrapper(Query query)
        {
            Query = query;
        }

        public Query Query { get; }

        public Query WhereEqualTo<TMember>(Expression<Func<TEntity, TMember>> selector, TMember value)
        {
            var propertyName = FieldPathResolver<TEntity>.Resolve(selector);
            return Query.WhereEqualTo(propertyName, value);
        }

        public Query WhereArrayContains<TMember>(Expression<Func<TEntity, TMember[]>> selector, TMember value)
        {
            var propertyName = FieldPathResolver<TEntity>.Resolve(selector);
            return Query.WhereArrayContains(propertyName, value);
        }

        public Query WhereGreaterThan<TMember>(Expression<Func<TEntity, TMember>> selector, TMember value)
        {
            var propertyName = FieldPathResolver<TEntity>.Resolve(selector);
            return Query.WhereGreaterThan(propertyName, value);
        }

        public Query WhereLessThan<TMember>(Expression<Func<TEntity, TMember>> selector, TMember value)
        {
            var propertyName = FieldPathResolver<TEntity>.Resolve(selector);
            return Query.WhereLessThan(propertyName, value);
        }

        public Query WhereGreaterThanOrEqualTo<TMember>(Expression<Func<TEntity, TMember>> selector, TMember value)
        {
            var propertyName = FieldPathResolver<TEntity>.Resolve(selector);
            return Query.WhereGreaterThanOrEqualTo(propertyName, value);
        }

        public Query WhereLessThanOrEqualTo<TMember>(Expression<Func<TEntity, TMember>> selector, TMember value)
        {
            var propertyName = FieldPathResolver<TEntity>.Resolve(selector);
            return Query.WhereLessThanOrEqualTo(propertyName, value);
        }

        public TypedQueryWrapper<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var firestoreQueryConverter = new FirestoreQueryConverter(Query);
            firestoreQueryConverter.Visit(predicate);
            return new TypedQueryWrapper<TEntity>(firestoreQueryConverter.Query);
//            
//            if (predicate.Body is MethodCallExpression)
//            {
//                var methodCallExpression = ((MethodCallExpression) predicate.Body);
//                if (methodCallExpression.Method.Name == "Contains")
//                {
//                    
//                    var property = methodCallExpression.Arguments[0]; // PropertyExpression / MemberExpression
//                    var value = methodCallExpression.Arguments[1]; // ConstantExpression
//                }
//            } 
//                
//            if(!(predicate.Body is BinaryExpression))
//                throw new NotImplementedException("Only binary expression are supported");
//
//            var binaryExpression = (BinaryExpression) predicate.Body;
//
//            var propertySelector = binaryExpression.Left as MemberExpression;
//            
//            if(propertySelector == null)
//                throw new InvalidOperationException("Left operator should be a MemberExpression");
//
//            var propertyName = FieldPathResolver.ResolveExpression(propertySelector);
//            var valueExpression = Evaluate(binaryExpression.Right);
//
//            
//            switch (binaryExpression.NodeType)
//            {
//                case ExpressionType.Equal:
//                    return CollectionReference.WhereEqualTo(propertyName, valueExpression);
//                case ExpressionType.GreaterThan:
//                    return CollectionReference.WhereGreaterThan(propertyName, valueExpression);
//                case ExpressionType.GreaterThanOrEqual:
//                    return CollectionReference.WhereGreaterThanOrEqualTo(propertyName, valueExpression);
//                case ExpressionType.LessThan:
//                    return CollectionReference.WhereLessThan(propertyName, valueExpression);
//                case ExpressionType.LessThanOrEqual:
//                    return CollectionReference.WhereLessThanOrEqualTo(propertyName, valueExpression);
//                default:
//                    throw new NotImplementedException();
//            }
        }

        public async Task<TypedQuerySnapshot<TEntity>> GetSnapshotAsync()
        {
            return new TypedQuerySnapshot<TEntity>(await Query.GetSnapshotAsync());
        }

        public TypedQueryWrapper<TEntity> OrderBy<TMember>(Expression<Func<TEntity, TMember>> selector)
        {
            return new TypedQueryWrapper<TEntity>(Query.OrderBy(FieldPathResolver<TEntity>.Resolve(selector)));
        }

        public TypedQueryWrapper<TEntity> OrderByDescending<TMember>(Expression<Func<TEntity, TMember>> selector)
        {
            return new TypedQueryWrapper<TEntity>(
                Query.OrderByDescending(FieldPathResolver<TEntity>.Resolve(selector)));
        }
    }
}
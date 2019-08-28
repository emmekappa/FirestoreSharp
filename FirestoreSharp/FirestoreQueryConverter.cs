using System;
using System.Linq.Expressions;
using Google.Cloud.Firestore;

namespace FirestoreSharp
{
    public class FirestoreQueryConverter : ExpressionVisitor {
        public Query Query { get; private set; }

        public FirestoreQueryConverter(Query query)
        {
            Query = query;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.OrElse)
            {
                throw new NotSupportedException("OR are not supported in firebase");
            }
            
            if (node.NodeType == ExpressionType.AndAlso)
            {
                //_query.WhereArrayContains()
            }

            if (node.Left is MemberExpression)
            {
                var propertyName = FieldPathResolver.ResolveExpression((MemberExpression) node.Left);
                var valueExpression = TypedCollectionWrapper<object>.Evaluate(node.Right);
                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                        Query = Query.WhereEqualTo(propertyName, valueExpression);
                        break;
                    case ExpressionType.GreaterThan:
                        Query = Query.WhereGreaterThan(propertyName, valueExpression);
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        Query = Query.WhereGreaterThanOrEqualTo(propertyName, valueExpression);
                        break;
                    case ExpressionType.LessThan:
                        Query = Query.WhereLessThan(propertyName, valueExpression);
                        break;
                    case ExpressionType.LessThanOrEqual:
                        Query = Query.WhereLessThanOrEqualTo(propertyName, valueExpression);
                        break;
                    default:
                        throw new NotSupportedException($"Operator {node.NodeType} not supported");
                }
            }

            return base.VisitBinary(node);
        }
        
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Contains":
                {
                    var property = FieldPathResolver.ResolveExpression((MemberExpression) node.Arguments[0]);
                    var value = TypedCollectionWrapper<object>.Evaluate(node.Arguments[1]);
                    Query = Query.WhereArrayContains(property, value);
                    break;
                }

                case "Equals":
                {
                    var property = FieldPathResolver.ResolveExpression((MemberExpression) node.Object);
                    var value = TypedCollectionWrapper<object>.Evaluate(node.Arguments[0]);
                    Query = Query.WhereEqualTo(property, value);
                    break;
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
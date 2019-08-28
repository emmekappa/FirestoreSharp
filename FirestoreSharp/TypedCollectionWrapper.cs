using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace FirestoreSharp
{
    public class TypedCollectionWrapper<TEntity> : TypedQueryWrapper<TEntity>
    {
        public TypedCollectionWrapper(CollectionReference collectionReference) : base(collectionReference)
        {
            CollectionReference = collectionReference;
        }

        public CollectionReference CollectionReference { get; }

     
        public async Task<TypedDocumentReference<TEntity>> AddAsync(TEntity entity)
        {
            return new TypedDocumentReference<TEntity>(await CollectionReference.AddAsync(entity));
        }

        public TypedDocumentReference<TEntity> GetDocument(string path)
        {
            return new TypedDocumentReference<TEntity>(CollectionReference.Document(path));
        }

        public static object Evaluate(Expression e)
        {
            if (e.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)e).Value;
            return Expression.Lambda(e).Compile().DynamicInvoke();
        }

        public IAsyncEnumerable<TypedDocumentReference<TEntity>> ListDocumentsAsync()
        {
            return CollectionReference.ListDocumentsAsync().Select(x => new TypedDocumentReference<TEntity>(x));
        }
    }
}
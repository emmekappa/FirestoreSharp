namespace FirestoreSharp.Tests
{
    using Google.Api.Gax.Grpc;
    using Google.Cloud.Firestore;
    using Google.Cloud.Firestore.V1;
    using Grpc.Core;

    public static class FirestoreDbFactory
    {
        public static FirestoreDb Create()
        {
            var firestoreSettings = new FirestoreSettings
            {
                CallSettings = CallSettings.FromHeaderMutation(x => x.Add("Authorization", "Bearer owner"))
            };
            return FirestoreDb.Create("na",
                FirestoreClient.Create(new Channel("127.0.0.1", 8080, ChannelCredentials.Insecure), firestoreSettings));
        }
    }
}
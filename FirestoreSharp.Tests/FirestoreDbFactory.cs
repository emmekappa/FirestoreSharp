namespace FirestoreSharp.Tests
{
    using System;
    using System.Text.RegularExpressions;
    using Google.Api.Gax.Grpc;
    using Google.Cloud.Firestore;
    using Google.Cloud.Firestore.V1;
    using Grpc.Core;

    public static class FirestoreDbFactory
    {
        public static FirestoreDb Create()
        {
            var firestoreHost = Environment.GetEnvironmentVariable("FIRESTORE_EMULATOR_HOST") ?? "localhost:8080";
            var matches = Regex.Match(firestoreHost, "(?<host>.*):(?<port>\\d+)");
            var host = matches.Groups["host"].Value;
            var port = int.Parse(matches.Groups["port"].Value);
            var firestoreSettings = new FirestoreSettings
            {
                CallSettings = CallSettings.FromHeaderMutation(x => x.Add("Authorization", "Bearer owner"))
            };
            return FirestoreDb.Create("na",
                FirestoreClient.Create(new Channel(host, port, ChannelCredentials.Insecure), firestoreSettings));
        }
    }
}
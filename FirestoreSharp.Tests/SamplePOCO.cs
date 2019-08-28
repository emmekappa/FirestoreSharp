using System;
using Google.Cloud.Firestore;

namespace FirestoreSharp.Tests
{
    [FirestoreData]
    public class SampleData
    {
        [FirestoreProperty] public DateTimeOffset DateTimeOffset1 { get; set; }
        [FirestoreProperty] public string String1 { get; set; }

        [FirestoreProperty("__string2_custom")]
        public string String2WithCustomName { get; set; }

        [FirestoreProperty] public int Int1 { get; set; }

        [FirestoreProperty("__int_second")] public int Int2WithCustomName { get; set; }

        public string StringWithoutFirestoreProperty { get; set; }

        [FirestoreProperty] public double Double1 { get; set; }
        [FirestoreProperty] public float Float1 { get; set; }
        [FirestoreProperty] public long Long1 { get; set; }
        [FirestoreProperty] public NestedData NestedData1 { get; set; }

        [FirestoreProperty("custom_nested_path")]
        public NestedData NestedData2CustomName { get; set; }

        [FirestoreProperty]
        public int[] IntArray1 { get; set; }
    }

    [FirestoreData]
    public class NestedData
    {
        [FirestoreProperty] public string String1 { get; set; }
        public int Int1 { get; set; }
        [FirestoreProperty("__int_second")] public int Int2WithCustomName { get; set; }
        [FirestoreProperty] public NestedNestedData NestedNestedData1 { get; set; }

        [FirestoreProperty] public NestedData RecursiveNestedData1 { get; set; }
    }

    [FirestoreData]
    public class NestedNestedData
    {
        [FirestoreProperty] public float Float1 { get; set; }
        [FirestoreProperty] public string String1 { get; set; }
    }
}
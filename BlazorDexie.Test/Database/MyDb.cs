﻿using BlazorDexie.Database;
using BlazorDexie.Options;
using BlazorDexie.Test.TestItems;
using System;

namespace BlazorDexie.Test.Database
{
    public class MyDb : Db<MyDb>
    {
        public static string CompoundIndex = new CompoundIndex(nameof(TestItemWithCompoundIndex.Firstname), nameof(TestItemWithCompoundIndex.Secondname)).ToString();

        public Store<TestItem, Guid> TestItems { get; set; } = new(nameof(TestItem.Id), nameof(TestItem.Year), nameof(TestItem.Name));

        public Store<TestItemHiddenKey, Guid> TestItemsHiddenKey { get; set; } = new(string.Empty, nameof(TestItemHiddenKey.Year));

        public Store<TestItemWithCompoundIndex, int> TestItemsWithCompoundIndex { get; set; } = new(
             "++" + nameof(TestItemWithCompoundIndex.Id),
             CompoundIndex);

        public Store<byte[], Guid> BlobData { get; set; } = new(string.Empty);
        public Store<byte[], Guid> BlobData2 { get; set; } = new(string.Empty);

        public MyDb(BlazorDexieOptions blazorDexieOptions, string databaseId)
            : base($"MyDatabase_{databaseId}", 1, Array.Empty<IDbVersion>(), blazorDexieOptions)
        {
        }
    }
}

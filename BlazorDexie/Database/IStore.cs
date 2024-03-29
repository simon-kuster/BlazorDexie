﻿using BlazorDexie.JsInterop;

namespace BlazorDexie.Database
{
    public interface IStore
    {
        public string[] SchemaDefinitions { get; }

        public void Init(Db db, string storeName, CollectionCommandExecuterJsInterop commandExecuterJsInterop);
    }
}

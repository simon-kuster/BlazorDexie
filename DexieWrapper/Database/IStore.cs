﻿using Nosthy.Blazor.DexieWrapper.JsInterop;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public interface IStore
    {
        public string[] Indices { get; }

        public void Init(Db db, string storeName, CommandExecuterJsInterop commandExecuterJsInterop);
    }
}

﻿using BlazorDexie.JsModule;
using Microsoft.Extensions.Logging;

namespace BlazorDexie.Options
{
    public class BlazorDexieOptions(IModuleFactory moduleFactory, ILoggerFactory loggerFactory)
    {
        public bool CamelCaseStoreNames { get; set; }
        public IModuleFactory ModuleFactory { get; } = moduleFactory;
        public ILoggerFactory LoggerFactory { get; } = loggerFactory;
    }
}

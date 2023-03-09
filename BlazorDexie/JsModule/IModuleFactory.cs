namespace BlazorDexie.JsModule
{
    public interface IModuleFactory
    {
        IModule CreateModule(string modulePath);
    }
}

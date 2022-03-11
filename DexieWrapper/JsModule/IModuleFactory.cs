namespace Nosthy.Blazor.DexieWrapper.JsModule
{
    public interface IModuleFactory
    {
        IModule CreateModule(string modulePath);
    }
}

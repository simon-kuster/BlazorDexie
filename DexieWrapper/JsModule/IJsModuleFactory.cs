namespace Nosthy.Blazor.DexieWrapper.JsModule
{
    public interface IJsModuleFactory
    {
        IJsModule CreateModule(string modulePath);
    }
}

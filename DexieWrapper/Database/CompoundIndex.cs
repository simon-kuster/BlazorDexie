namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class CompoundIndex
    {
        private string _compoundIndex;

        public CompoundIndex(params string[] indices)
        {
            _compoundIndex = "[" + string.Join("+", indices) + "]";
        }

        public override string ToString()
        {
            return _compoundIndex;
        }
    }
}

namespace Glyphy.Misc
{
    public class AutoDictionaryInitializer<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull where TValue : new()
    {
        public new TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out TValue? value))
                {
                    value = new TValue();
                    Add(key, value);
                }
                return value;
            }
            set => base[key] = value;
        }
    }
}

namespace GodotUtils;

public static class ExtensionsDictionary
{
    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> me, Dictionary<TKey, TValue> merge)
    {
        foreach (KeyValuePair<TKey, TValue> item in merge)
        {
            me[item.Key] = item.Value;
        }
    }
}

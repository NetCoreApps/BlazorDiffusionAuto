using System.Collections.Specialized;
using ServiceStack;

namespace BlazorDiffusion.Client.UI;

public static class UiExtensions
{
    public static int? GetInt(this NameValueCollection query, string name) => 
        X.Map(query[name], x => int.TryParse(x, out var num) ? num : (int?)null);
}

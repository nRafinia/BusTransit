using System.Collections.Generic;
using Common.Containers;

namespace Common.Tools
{
    public interface IAppSetting 
    {
        void ClearCachedItems();
        string Get(string section, bool forceUpdate = false);
        Dictionary<string, string> GetSettings(string section, bool forceUpdate = false);
        T Get<T>(string section, bool forceUpdate = false);
    }
}
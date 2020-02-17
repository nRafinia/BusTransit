using System.Collections.Generic;
using System.Linq;
using Common.QMessageModels;
using Common.Tools;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;

namespace Common.Extensions
{
    public static class SettingExt
    {
        public static QSettingModel GetSetting(this IAppSetting appSetting, string name,
            string section = "QueueSettings")
        {
            return appSetting.Get<List<QSettingModel>>(section)
                .First(s => s.Active && s.Name == name);
        }
    }
}
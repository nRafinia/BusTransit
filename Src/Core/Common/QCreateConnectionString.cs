using Common.QMessageModels;

namespace Common
{
    internal static class QCreateConnectionString
    {
        public static string CreateConnection(QSettingModel setting)
        {
            var res = $"host={setting.ServerAddress}";

            if (setting.Timeout != null)
                res += $";timeout={setting.Timeout.Value}";

            if (!string.IsNullOrWhiteSpace(setting.UserName))
            {
                res += $";username={setting.UserName};password={setting.Password}";
            }

            return res;
        }
    }
}
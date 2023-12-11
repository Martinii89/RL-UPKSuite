using System.Configuration;

namespace RLUpkSuite
{
    public class UserConfiguration : ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue("keys.txt")]
        public string KeysPath
        {
            get => (string)this[nameof(KeysPath)];
            set => this[nameof(KeysPath)] = value;
        }
        
        [UserScopedSetting]
        public string StartPage
        {
            get => (string)this[nameof(StartPage)];
            set => this[nameof(StartPage)] = value;
        }
    }
}
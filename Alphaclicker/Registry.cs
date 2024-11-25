using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AlphaClicker
{
    public class AlphaRegistry
    {
        private static RegistryKey GetRegistryKey(string keyPath)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true);

            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(keyPath);
            }

            return key;
        }

        public static void SetMinecraftOnly(bool minecraftOnly)
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return;
            regKey.SetValue("minecraftOnly", minecraftOnly);
        }

        public static bool GetMinecraftOnly()
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return false;

            try
            {
                return regKey.GetValue("minecraftOnly")?.ToString() == "True";
            }
            catch
            { }

            return false;
        }

        public static void SaveClickerConfig(
            string hours, string mins, string secs, string millis,
            string random1, string random2,
            bool isRandomMode,
            string mouseButton,
            string clickType,
            bool isRepeatTimes, string repeatTimes)
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return;

            regKey.SetValue("hours", hours);
            regKey.SetValue("mins", mins);
            regKey.SetValue("secs", secs);
            regKey.SetValue("millis", millis);
            regKey.SetValue("random1", random1);
            regKey.SetValue("random2", random2);
            regKey.SetValue("isRandomMode", isRandomMode);
            regKey.SetValue("mouseButton", mouseButton ?? "Left");
            regKey.SetValue("clickType", clickType ?? "Single");
            regKey.SetValue("isRepeatTimes", isRepeatTimes);
            regKey.SetValue("repeatTimes", repeatTimes);
        }

        public static (string hours, string mins, string secs, string millis,
            string random1, string random2,
            bool isRandomMode,
            string mouseButton,
            string clickType,
            bool isRepeatTimes, string repeatTimes) LoadClickerConfig()
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return GetDefaultConfig();

            try
            {
                return (
                    regKey.GetValue("hours")?.ToString() ?? "0",
                    regKey.GetValue("mins")?.ToString() ?? "0",
                    regKey.GetValue("secs")?.ToString() ?? "0",
                    regKey.GetValue("millis")?.ToString() ?? "100",
                    regKey.GetValue("random1")?.ToString() ?? "0.1",
                    regKey.GetValue("random2")?.ToString() ?? "0.2",
                    regKey.GetValue("isRandomMode")?.ToString() == "True",
                    regKey.GetValue("mouseButton")?.ToString() ?? "Left",
                    regKey.GetValue("clickType")?.ToString() ?? "Single",
                    regKey.GetValue("isRepeatTimes")?.ToString() == "True",
                    regKey.GetValue("repeatTimes")?.ToString() ?? "0"
                );
            }
            catch
            {
                return GetDefaultConfig();
            }
        }

        private static (string, string, string, string, string, string, bool, string, string, bool, string) GetDefaultConfig()
        {
            return ("0", "0", "0", "100", "0.1", "0.2", false, "Left", "Single", false, "0");
        }

        public static void SetKeybindValues()
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return;

            regKey.SetValue("key1", Keybinds.key1);
            regKey.SetValue("key2", Keybinds.key2);
            regKey.SetValue("keybinding", Keybinds.keyBinding);
        }

        public static void GetKeybindValues()
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return;

            try
            {
                Keybinds.key1 = Int32.Parse(regKey.GetValue("key1").ToString());
                Keybinds.key2 = Int32.Parse(regKey.GetValue("key2").ToString());
                Keybinds.keyBinding = regKey.GetValue("keybinding").ToString();
            }
            catch
            {
                // Ignore this exception
            }
        }

        public static void SetWindowSettings(bool topmostValue)
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return;

            regKey.SetValue("theme", ThemesController.CurrentTheme);
            regKey.SetValue("topmost", topmostValue);
        }

        public static string GetTheme()
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return "";

            try
            {
                return regKey.GetValue("theme").ToString();
            }
            catch
            { }

            return "";
        }

        public static bool GetTopmost()
        {
            RegistryKey regKey = GetRegistryKey("Software\\AlphaClicker");
            if (regKey == null) return true;

            try
            {
                if (regKey.GetValue("topmost").ToString() == "True")
                    return true;
                else
                    return false;
            }
            catch
            { }

            return true;
        }
    }
}

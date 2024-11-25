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

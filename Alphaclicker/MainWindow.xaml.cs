using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;

namespace AlphaClicker
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private bool IsMinecraftActive()
        {
            bool isChecked = false;
            try
            {
                Dispatcher.Invoke(() =>
                {
                    isChecked = (bool)minecraftOnlyCheckbox.IsChecked;
                });

                if (!isChecked)
                    return true;

                const int nChars = 256;
                StringBuilder buff = new StringBuilder(nChars);
                IntPtr handle = GetForegroundWindow();

                if (GetWindowText(handle, buff, nChars) > 0)
                {
                    string windowTitle = buff.ToString();
                    return windowTitle.Contains("Minecraft");
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void LoadKeybind()
        {
            startBtn.Content = $"Start ({Keybinds.keyBinding})";
            stopBtn.Content = $"Stop ({Keybinds.keyBinding})";
        }

        private void Cerror(string errormessage)
        {
            ToggleClick();
            MessageBox.Show(errormessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private int ToInt(string number)
        {
            return Int32.Parse((number == "") ? "0" : number);
        }

        private void FadeButtonColor(Button btn, string hex)
        {
            ColorAnimation animation =
               new ColorAnimation(
                   (Color)ColorConverter.ConvertFromString(hex),
                   new Duration(TimeSpan.FromSeconds(0.2))
               );
            btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void ToggleClick()
        {
            if (!IsMinecraftActive())
            {
                return;
            }

            string startEnabled = "#1494e3";
            string startDisabled = "#084466";
            string stopEnabled = "#FF605C";
            string stopDisabled = "#c43c35";

            if (startBtn.IsEnabled)
            {
                FadeButtonColor(startBtn, startDisabled);
                FadeButtonColor(stopBtn, stopEnabled);
                startBtn.IsEnabled = false;
                stopBtn.IsEnabled = true;

                Thread clickhandler = new Thread(ClickHandler);
                clickhandler.Start();
            }
            else
            {
                FadeButtonColor(startBtn, startEnabled);
                FadeButtonColor(stopBtn, stopDisabled);
                startBtn.IsEnabled = true;
                stopBtn.IsEnabled = false;
            }
        }

        public bool keyEnabled = true;
        void KeyHandler()
        {
            while (true)
            {
                if (keyEnabled && IsMinecraftActive())
                {
                    if (Keybinds.key1 == -1)
                    {
                        if (WinApi.GetAsyncKeyState(Keybinds.key2) > 0)
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                ToggleClick();
                            }));
                        }
                    }
                    else
                    {
                        if ((WinApi.GetAsyncKeyState(Keybinds.key1) & 0x8000) == 0x8000 &&
                            WinApi.GetAsyncKeyState(Keybinds.key2) > 0)
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                ToggleClick();
                            }));
                        }
                    }
                }
                Thread.Sleep(200);
            }
        }

        void ClickHandler()
        {
            if (!IsMinecraftActive())
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    ToggleClick();
                }));
                return;
            }

            int sleep = 0;
            bool useRandomSleep = false;
            int randnum1 = 0;
            int randnum2 = 0;
            string mouseBtn = "";
            string clickType = "";
            bool repeatTimesChecked = false;
            int repeatTimes = 0;
            bool customCoordsChecked = false;
            int customCoordsX = 0, customCoordsY = 0;

            Dispatcher.Invoke((Action)(() =>
            {
                /* Grab Click Interval */
                try
                {
                    useRandomSleep = (bool)randomIntervalMode.IsChecked;
                    if (useRandomSleep)
                    {
                        randnum1 = (int)(float.Parse(randomSecs1Box.Text,
                                                CultureInfo.InvariantCulture.NumberFormat) * 1000);
                        randnum2 = (int)(float.Parse(randomSecs2Box.Text,
                                               CultureInfo.InvariantCulture.NumberFormat) * 1000);
                        randnum1 = (randnum1 == 0) ? 1 : randnum1;
                        randnum2 = (randnum2 == 0) ? 1 : randnum2;
                    }
                    else
                    {
                        if (millisecsBox.Text == "0")
                        {
                            millisecsBox.Text = "1";
                        }
                        sleep = ToInt(millisecsBox.Text)
                        + ToInt(secondsBox.Text) * 1000
                        + ToInt(minsBox.Text) * 60000
                        + ToInt(hoursBox.Text) * 3600000;
                        sleep = (sleep == 0) ? 1 : sleep;
                    }
                }
                catch (FormatException ex)
                {
                    Cerror(ex.ToString());
                    return;
                }

                /* Grab Mousebutton And Clicktype */
                mouseBtn = (mouseBtnCBOX.SelectedItem as ComboBoxItem)?.Content.ToString();
                clickType = (clickTypeCBOX.SelectedItem as ComboBoxItem)?.Content.ToString();

                /* Grab Repeat Stuff */
                repeatTimesChecked = (bool)repeatTimesRBtn.IsChecked;
                if (repeatTimesChecked)
                {
                    try
                    {
                        repeatTimes = Int32.Parse(repeatTimesBox.Text);
                    }
                    catch (FormatException)
                    {
                        Cerror("Invalid Repeat Times Number");
                        return;
                    }
                }

                /* Grab Coords Stuff */
                customCoordsChecked = (bool)coordsCBtn.IsChecked;
                if (customCoordsChecked)
                {
                    try
                    {
                        customCoordsX = Int32.Parse(xBox.Text);
                        customCoordsY = Int32.Parse(yBox.Text);
                    }
                    catch (FormatException)
                    {
                        Cerror("Invalid Coordinates");
                        return;
                    }
                }
            }));

            int repeatCount = 0;
            Random rnd = new Random();

            while (true)
            {
                if (!IsMinecraftActive())
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        ToggleClick();
                    }));
                    break;
                }

                bool doClick = false;
                Dispatcher.Invoke((Action)(() =>
                {
                    doClick = stopBtn.IsEnabled;
                }));

                if (doClick)
                {
                    if (repeatTimesChecked)
                    {
                        if (repeatCount >= repeatTimes)
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                ToggleClick();
                            }));
                            break;
                        }
                        repeatCount += 1;
                    }

                    // Get current cursor position before clicking
                    WinApi.POINT currentPos = new WinApi.POINT();
                    if (!customCoordsChecked)
                    {
                        WinApi.GetCursorPos(out currentPos);
                        customCoordsX = currentPos.X;
                        customCoordsY = currentPos.Y;
                    }

                    if (clickType == "Single")
                    {
                        WinApi.DoClick(mouseBtn, customCoordsChecked, customCoordsX, customCoordsY);
                    }
                    else
                    {
                        WinApi.DoClick(mouseBtn, customCoordsChecked, customCoordsX, customCoordsY);
                        Thread.Sleep(300);
                        WinApi.DoClick(mouseBtn, customCoordsChecked, customCoordsX, customCoordsY);
                    }

                    if (useRandomSleep)
                    {
                        sleep = rnd.Next((randnum1 < randnum2) ? randnum1 : randnum2,
                                       (randnum1 > randnum2) ? randnum1 : randnum2);
                    }

                    Thread.Sleep(sleep);
                }
                else
                {
                    break;
                }
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AlphaRegistry.GetTheme() == "Dark")
                ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
            this.Topmost = AlphaRegistry.GetTopmost();

            // Load minecraft only setting
            minecraftOnlyCheckbox.IsChecked = AlphaRegistry.GetMinecraftOnly();

            // Load saved configuration
            var config = AlphaRegistry.LoadClickerConfig();

            // Apply the loaded configuration
            hoursBox.Text = config.hours;
            minsBox.Text = config.mins;
            secondsBox.Text = config.secs;
            millisecsBox.Text = config.millis;
            randomSecs1Box.Text = config.random1;
            randomSecs2Box.Text = config.random2;

            randomIntervalMode.IsChecked = config.isRandomMode;
            clickIntervalMode.IsChecked = !config.isRandomMode;

            mouseBtnCBOX.SelectedItem = mouseBtnCBOX.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == config.mouseButton);

            clickTypeCBOX.SelectedItem = clickTypeCBOX.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == config.clickType);

            repeatTimesRBtn.IsChecked = config.isRepeatTimes;
            repeatForeverRBtn.IsChecked = !config.isRepeatTimes;
            repeatTimesBox.Text = config.repeatTimes;

            Thread keyhandler = new Thread(KeyHandler);
            keyhandler.Start();

            AlphaRegistry.GetKeybindValues();
            LoadKeybind();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save all configuration
            AlphaRegistry.SaveClickerConfig(
                hoursBox.Text,
                minsBox.Text,
                secondsBox.Text,
                millisecsBox.Text,
                randomSecs1Box.Text,
                randomSecs2Box.Text,
                (bool)randomIntervalMode.IsChecked,
                (mouseBtnCBOX.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (clickTypeCBOX.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (bool)repeatTimesRBtn.IsChecked,
                repeatTimesBox.Text
            );

            AlphaRegistry.SetMinecraftOnly((bool)minecraftOnlyCheckbox.IsChecked);
            Environment.Exit(0);
        }
      

        private void getCoordsBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            GetCursorPos win = new GetCursorPos();
            win.Owner = this;
            win.Show();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleClick();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleClick();
        }

        private void changeHotkeyBtn_Click(object sender, RoutedEventArgs e)
        {
            keyEnabled = false;
            ChangeHotkey win = new ChangeHotkey();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.ShowDialog();
        }

        private void windowSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowSettings win = new WindowSettings();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.ShowDialog();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System)
            {
                e.Handled = true;
            }
        }

        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void minimizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}

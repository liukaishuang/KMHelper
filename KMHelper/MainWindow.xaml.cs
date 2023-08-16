using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace KMHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate int HookHandler(int nCode, Int32 wParam, IntPtr lParam);
        int hv = 0;
        HookHandler hh; 
        const uint MAPVK_VK_TO_VSC_EX = 0x04;

        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookHandler lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        bool keyOrMouse = true;
        bool isContinue = false;
        Thread t1;
        public MainWindow()
        {
            InitializeComponent();
        }
        int startKey = 0;
        string[] ks = new string[0]; 
        string szx = "";
        Dictionary<Key, int> kts = new Dictionary<Key, int>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string str = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "km.txt"));
            string[] strs = str.Split('|');
            ks = strs[0].Split(',');
           
            foreach(string k in ks)
            {
                string[] ktvs = k.Split('、');
                Key kv = (Key)Enum.Parse(typeof(Key), ktvs[0]);
                int kt = Convert.ToInt32(ktvs[1]);
                kts.Add(kv, kt);               

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Height = 40;

                TextBlock tbk = new TextBlock();
                tbk.Text ="Key:";
                tbk.VerticalAlignment = VerticalAlignment.Center;
                sp.Children.Add(tbk);

                TextBox tk = new TextBox();
                tk.Text = kv.ToString();
                tk.IsReadOnly = true;
                tk.Height = 23;
                tk.Width = 100;
                tk.Margin = new Thickness(0,0,20,0);
                tk.VerticalAlignment = VerticalAlignment.Center;
                tk.KeyDown += Tk_KeyDown;
                sp.Children.Add(tk);

                TextBlock tbt = new TextBlock();
                tbt.Text = "Interval:";
                tbt.VerticalAlignment = VerticalAlignment.Center;
                sp.Children.Add(tbt);


                CCIntBox tt = new CCIntBox();
                tt.Text = kt.ToString();
                tt.Height = 23;
                tt.Width = 100;
                tt.Margin = new Thickness(0, 0, 20, 0);
                tt.VerticalAlignment = VerticalAlignment.Center;
                sp.Children.Add(tt);

                Button b = new Button();
                b.Content = "Delete";
                b.VerticalAlignment = VerticalAlignment.Center;
                b.Width = 75;
                b.Click += BD_Click;
                sp.Children.Add(b);

                spContent.Children.Add(sp);
            }

            string[] sks = strs[1].Split('、');
            if (sks.Length > 1)
            {
                rbK.IsChecked = true;
                tbQDJ.Text = sks[1];
                startKey = Convert.ToInt32(Enum.Parse(typeof(Key), sks[1]));
                keyOrMouse = true;
            }
            else
            {
                if (sks[0] == "2")
                {
                    rbML.IsChecked = true;
                    startKey = 513;
                }
                else if (sks[0] == "3")
                {
                    rbMR.IsChecked = true;
                    startKey = 516;
                }
                else if (sks[0] == "4")
                {
                    rbMU.IsChecked = true;
                    startKey = 1;
                }
                else if (sks[0] == "5")
                {
                    rbMD.IsChecked = true;
                    startKey = -1;
                }
                keyOrMouse = false;
            }

            szx = strs[2];
            if (szx == "1")
            {
                rbXH.IsChecked = true;
            }
            else
            {
                rbYC.IsChecked = true;
            }


            if (rbK.IsChecked.HasValue && rbK.IsChecked.Value)
            {
                HookStart(13);
            }
            else
            {
                HookStart(14);
            }

            InitKey();
        }

        public void HookStart(int typeValue)
        {
            if (hv == 0)
            {
                hh = new HookHandler(HookAction);

                hv = SetWindowsHookEx(typeValue, hh, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);

                if (hv == 0)
                {
                    HookStop();
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "km.txt"), "D1、10,D2、10,D3、10,D4、10,D5、10||1");
                    MessageBox.Show("Preprocessing failure");

                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
            }
        }

        public void HookStop()
        {
            bool retKeyboard = true;
            if (hv != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hv);
                hv = 0;
            }
            if (!retKeyboard)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "km.txt"), "D1、10,D2、10,D3、10,D4、10,D5、10||1");
                MessageBox.Show("Preprocessing failure");

                Application.Current.Shutdown();
                Environment.Exit(0);
            }
        }

        private int HookAction(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (keyOrMouse)
                {
                    if (wParam == 257)
                    {
                        KeyMSG m = (KeyMSG)Marshal.PtrToStructure(lParam, typeof(KeyMSG));
                        Key k = KeyInterop.KeyFromVirtualKey(m.vkCode);
                        if (startKey == Convert.ToInt32(k))
                        {
                            isContinue = !isContinue;
                        }
                    }
                }
                else
                {
                    if (wParam == 522)
                    {
                        MouseHookStruct m = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                        if (startKey == 1 && m.hWnd > 0)
                        {
                            isContinue = !isContinue;
                        }
                        else if (startKey == -1 && m.hWnd < 0)
                        {
                            isContinue = !isContinue;
                        }
                    }
                    else if (wParam == startKey)
                    {
                        isContinue = !isContinue;
                    }
                }

                return 0;
            }
            return CallNextHookEx(hv, nCode, wParam, lParam);
        }

        private void InitKey()
        {
            Ols ols = new Ols();
            switch (ols.GetStatus())
            {
                case (uint)Ols.Status.NO_ERROR:
                    break;
                case (uint)Ols.Status.DLL_NOT_FOUND:
                    MessageBox.Show("Status Error!! DLL_NOT_FOUND");
                    Environment.Exit(0);
                    break;
                case (uint)Ols.Status.DLL_INCORRECT_VERSION:
                    MessageBox.Show("Status Error!! DLL_INCORRECT_VERSION");
                    break;
                case (uint)Ols.Status.DLL_INITIALIZE_ERROR:
                    MessageBox.Show("Status Error!! DLL_INITIALIZE_ERROR");
                    break;
            }

            switch (ols.GetDllStatus())
            {
                case (uint)Ols.OlsDllStatus.OLS_DLL_NO_ERROR:
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED:
                    MessageBox.Show("DLL Status Error!! OLS_DRIVER_NOT_LOADED");
                    Environment.Exit(0);
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_UNSUPPORTED_PLATFORM:
                    MessageBox.Show("DLL Status Error!! OLS_UNSUPPORTED_PLATFORM");
                    Environment.Exit(0);
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_FOUND:
                    MessageBox.Show("DLL Status Error!! OLS_DLL_DRIVER_NOT_FOUND");
                    Environment.Exit(0);
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_UNLOADED:
                    MessageBox.Show("DLL Status Error!! OLS_DLL_DRIVER_UNLOADED");
                    Environment.Exit(0);
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK:
                    MessageBox.Show("DLL Status Error!! DRIVER_NOT_LOADED_ON_NETWORK");
                    Environment.Exit(0);
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_UNKNOWN_ERROR:
                    MessageBox.Show("DLL Status Error!! OLS_DLL_UNKNOWN_ERROR");
                    Environment.Exit(0);
                    break;
            }

            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            t1 = new Thread(() =>
            {
                if (szx == "1")
                {
                    while (true)
                    {
                        if (isContinue)
                        {
                            foreach (Key k in kts.Keys)
                            {
                                KeyAction(ols, inputLocaleIdentifier, k);
                                Thread.Sleep(kts[k]);
                            }
                        }
                        Thread.Sleep(1);
                    }
                }
                else
                {
                    while (true)
                    {
                        if (isContinue)
                        {
                            foreach (Key k in kts.Keys)
                            {
                                KeyAction(ols, inputLocaleIdentifier, k);
                                Thread.Sleep(kts[k]);
                            }
                            isContinue = false;
                        }
                        Thread.Sleep(1);
                    }
                }               
            });
            t1.Start();
        }

        private static void KeyAction(Ols ols, IntPtr inputLocaleIdentifier, Key k)
        {
            int vk = KeyInterop.VirtualKeyFromKey(k);
            uint scanCode = MapVirtualKeyEx((uint)vk, MAPVK_VK_TO_VSC_EX, inputLocaleIdentifier);

            WRKeyDown(ols, scanCode);
            WRKeyUp(ols, scanCode);
        }

        private static void WRKeyDown(Ols ols, uint scanCode)
        {
            KBCTillOBF(ols);
            TillIBF(ols);
            ols.WriteIoPortByte.Invoke(0x64, 0xD2);
            TillIBF(ols);
            ols.WriteIoPortByte.Invoke(0x60, (byte)scanCode);
        }

        private static void WRKeyUp(Ols ols, uint scanCode)
        {
            KBCTillOBF(ols);
            TillIBF(ols);
            ols.WriteIoPortByte.Invoke(0x64, 0xD2);
            TillIBF(ols);
            ols.WriteIoPortByte.Invoke(0x60, (byte)(scanCode | 0x80));
        }

        private static void KBCTillOBF(Ols ols)
        {
            byte dwVal = 0;
            do
            {
                ols.ReadIoPortByteEx(0x64, ref dwVal);
            }
            while ((dwVal & 0x01) > 0);
        }

        private static void TillIBF(Ols ols)
        {
            byte dwVal = 0;
            do
            {
                ols.ReadIoPortByteEx(0x64, ref dwVal);
            }
            while ((dwVal & 0x2) > 0);
        }

        [DllImport("user32.dll")]
        static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);
        private static bool IsCapsLockOn()
        {
            byte[] bs = new byte[256];
            GetKeyboardState(bs);
            return (bs[0x14] == 1);
        }

        private void Tk_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = e.Key.ToString();
        }

        private void BD_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            spContent.Children.Remove(b.Parent as UIElement);
        }

        private void rbK_Checked(object sender, RoutedEventArgs e)
        {
            tbQDJ.IsEnabled = true;
        }

        private void rbK_Unchecked(object sender, RoutedEventArgs e)
        {
            tbQDJ.Text = "";
            tbQDJ.IsEnabled = false;
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Height = 40;

            TextBlock tbk = new TextBlock();
            tbk.Text = "Key:";
            tbk.VerticalAlignment = VerticalAlignment.Center;
            sp.Children.Add(tbk);

            TextBox tk = new TextBox();
            tk.Text = "";
            tk.IsReadOnly = true;
            tk.Height = 23;
            tk.Width = 100;
            tk.Margin = new Thickness(0, 0, 20, 0);
            tk.VerticalAlignment = VerticalAlignment.Center;
            tk.KeyDown += Tk_KeyDown;
            sp.Children.Add(tk);

            TextBlock tbt = new TextBlock();
            tbt.Text = "Interval:";
            tbt.VerticalAlignment = VerticalAlignment.Center;
            sp.Children.Add(tbt);


            CCIntBox tt = new CCIntBox();
            tt.Text = "50";
            tt.Height = 23;
            tt.Width = 100;
            tt.Margin = new Thickness(0, 0, 20, 0);
            tt.VerticalAlignment = VerticalAlignment.Center;
            sp.Children.Add(tt);

            Button b = new Button();
            b.Content = "Delete";
            b.VerticalAlignment = VerticalAlignment.Center;
            b.Width = 75;
            b.Click += BD_Click;
            sp.Children.Add(b);

            spContent.Children.Add(sp);
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            string ks = ""; 
            string k = "";
            int c = 0;
            foreach (UIElement ui in spContent.Children)
            {
                if (ui is StackPanel)
                {
                    k = "";
                    c = 0;
                    foreach (UIElement ue in (ui as StackPanel).Children)
                    {
                        if (ue is TextBox)
                        {
                            TextBox tb = ue as TextBox;
                            if (tb.Text != null && tb.Text != "")
                            {
                                if (tb.IsReadOnly)
                                {
                                    k += tb.Text;
                                    c++;
                                }
                                else
                                {
                                    k += "、" + tb.Text + ",";
                                    c++;
                                }
                            }

                        }
                    }

                    if (c == 2)
                    {
                        ks += k;
                    }
                }                
            }

            ks = ks.TrimEnd(',');

            ks += "|";

            if (rbK.IsChecked.HasValue&&rbK.IsChecked.Value)
            {
                if (tbQDJ.Text != null && tbQDJ.Text != "")
                {
                    ks += "1、" + tbQDJ.Text;
                }
                else
                {
                    MessageBox.Show("Please set the start key");
                }               
            }
            else if (rbML.IsChecked.HasValue && rbML.IsChecked.Value)
            {
                ks += "2";
            }
            else if (rbMR.IsChecked.HasValue && rbMR.IsChecked.Value)
            {
                ks += "3";
            }
            else if (rbMU.IsChecked.HasValue && rbMU.IsChecked.Value)
            {
                ks += "4";
            }
            else if (rbMD.IsChecked.HasValue && rbMD.IsChecked.Value)
            {
                ks += "5";
            }

            ks += "|";

            if(rbXH.IsChecked.HasValue && rbXH.IsChecked.Value)
            {
                ks += "1";
            }
            else if (rbYC.IsChecked.HasValue && rbYC.IsChecked.Value)
            {
                ks += "2";
            }

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "km.txt"), ks);

            MessageBox.Show("Save successfully,please restart");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}

using System;
using System.Windows.Forms;
using Ibex.UserActivityMonitorApp;
using System.Security.Principal;
using Ibex.UserActivityMonitorApp.Dtos;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.IO;
using Ibex.UserActivityMonitorApp.Properties;
using Microsoft.Win32;
using Ibex.UserActivityMonitor;

namespace Ibex.UserActivityMonitorApp
{
    public partial class TestFormStatic : Form
    {
        private NotifyIcon trayIcon;
        static System.Windows.Forms.Timer ApiTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer HookTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer AvailabTimer = new System.Windows.Forms.Timer();
        DateTime? lastActivityDate;
        bool checkedIn = false;
        bool onBreak = false;
        bool? checkedOut = null;
        string userName = WindowsIdentity.GetCurrent().Name;
        List<UserActivityDto> listOfItems;
        public TestFormStatic()
        {
            InitializeComponent();
            lastActivityDate = DateTime.Now;
            listOfItems = new List<UserActivityDto>();
            InitializeTimer();
            AddApplicationTrayHideMainWindow();
            RegisterApplicationToAutoStartProcess();
        }
        void AddApplicationTrayHideMainWindow()
        {
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.appIcon,
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipText = "Ibex Activity Monitor",
                BalloonTipTitle = "Ibex",
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
            this.ShowInTaskbar = false;
            Hide();
            this.WindowState = FormWindowState.Minimized;
        }
        void RegisterApplicationToAutoStartProcess()
        {
            //Create a registry key, then save your program path
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg.SetValue("Ibex Activity Monitor", Application.ExecutablePath.ToString());
            MessageBox.Show("Ibex Monitor Registered sucessfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            //trayIcon.Visible = false;
            //Application.Exit();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        //##################################################################
        #region Check boxes to set or remove particular event handlers.

        private void checkBoxOnMouseMove_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.MouseMove += HookManager_MouseMove;
            //if (checkBoxOnMouseMove.Checked)
            //{
            //    HookManager.MouseMove += HookManager_MouseMove;
            //}
            //else
            //{
            //    HookManager.MouseMove -= HookManager_MouseMove;
            //}
        }

        private void checkBoxOnMouseClick_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.MouseClick += HookManager_MouseClick;
            //if (checkBoxOnMouseClick.Checked)
            //{
            //    HookManager.MouseClick += HookManager_MouseClick;
            //}
            //else
            //{
            //    HookManager.MouseClick -= HookManager_MouseClick;
            //}
        }

        private void checkBoxOnMouseUp_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.MouseUp += HookManager_MouseUp;
            //if (checkBoxOnMouseUp.Checked)
            //{
            //    HookManager.MouseUp += HookManager_MouseUp;
            //}
            //else
            //{
            //    HookManager.MouseUp -= HookManager_MouseUp;
            //}
        }

        private void checkBoxOnMouseDown_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.MouseDown += HookManager_MouseDown;
            //if (checkBoxOnMouseDown.Checked)
            //{
            //    HookManager.MouseDown += HookManager_MouseDown;
            //}
            //else
            //{
            //    HookManager.MouseDown -= HookManager_MouseDown;
            //}
        }

        private void checkBoxMouseDoubleClick_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.MouseDoubleClick += HookManager_MouseDoubleClick;
            //if (checkBoxMouseDoubleClick.Checked)
            //{
            //    HookManager.MouseDoubleClick += HookManager_MouseDoubleClick;
            //}
            //else
            //{
            //    HookManager.MouseDoubleClick -= HookManager_MouseDoubleClick;
            //}
        }

        private void checkBoxMouseWheel_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.MouseWheel += HookManager_MouseWheel;
            //if (checkBoxMouseWheel.Checked)
            //{
            //    HookManager.MouseWheel += HookManager_MouseWheel;
            //}
            //else
            //{
            //    HookManager.MouseWheel -= HookManager_MouseWheel;
            //}
        }

        private void checkBoxKeyDown_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.KeyDown += HookManager_KeyDown;
            //if (checkBoxKeyDown.Checked)
            //{
            //    HookManager.KeyDown += HookManager_KeyDown;
            //}
            //else
            //{
            //    HookManager.KeyDown -= HookManager_KeyDown;
            //}
        }


        private void checkBoxKeyUp_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.KeyUp += HookManager_KeyUp;
            //if (checkBoxKeyUp.Checked)
            //{
            //    HookManager.KeyUp += HookManager_KeyUp;
            //}
            //else
            //{
            //    HookManager.KeyUp -= HookManager_KeyUp;
            //}
        }

        private void checkBoxKeyPress_CheckedChanged(object sender, EventArgs e)
        {
            HookManager.KeyPress += HookManager_KeyPress;
            //if (checkBoxKeyPress.Checked)
            //{
            //    HookManager.KeyPress += HookManager_KeyPress;
            //}
            //else
            //{
            //    HookManager.KeyPress -= HookManager_KeyPress;
            //}
        }

        #endregion

        //##################################################################
        #region Event handlers of particular events. They will be activated when an appropriate checkbox is checked.

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            AddRecord(DateTime.Now, e.KeyCode.ToString(), "Key Press");
            textBoxLog.AppendText(string.Format("KeyDown - {0}\n", e.KeyCode));
            textBoxLog.ScrollToCaret();
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            AddRecord(DateTime.Now, e.KeyCode.ToString(), "Key Press");
            textBoxLog.AppendText(string.Format("KeyUp - {0}\n", e.KeyCode));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            AddRecord(DateTime.Now, e.KeyChar.ToString(), "Key Press");
            textBoxLog.AppendText(e.KeyChar.ToString());
            textBoxLog.ScrollToCaret();
        } 


        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            AddRecord(DateTime.Now, "Mouse Move", "Mouse Move");
            labelMousePosition.Text = string.Format("x={0:0000}; y={1:0000}", e.X, e.Y);
        }

        private void HookManager_MouseClick(object sender, MouseEventArgs e)
        {
            AddRecord(DateTime.Now, e.Button.ToString(), "Mouse Click");
            textBoxLog.AppendText(string.Format("MouseClick - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseUp(object sender, MouseEventArgs e)
        {
            AddRecord(DateTime.Now, "Mouse Up", "Mouse Up");
            textBoxLog.AppendText(string.Format("MouseUp - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            AddRecord(DateTime.Now, "Mouse Down", "Mouse Down");
            textBoxLog.AppendText(string.Format("MouseDown - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddRecord(DateTime.Now, "Mouse Click", "Mouse Click");
            textBoxLog.AppendText(string.Format("MouseDoubleClick - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseWheel(object sender, MouseEventArgs e)
        {
            AddRecord(DateTime.Now, "Mouse Wheel", "Mouse Wheel");
            labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
        }

        #endregion
        private void PostActivities()
        {
            try
            {
                if (checkedIn && !onBreak)
                {
                    if (checkedOut == null || checkedOut == false)
                    {
                        if (listOfItems != null && listOfItems.Count == 0)
                        {
                            listOfItems.Add(new UserActivityDto()
                            {
                                ActivityTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                                ActivityType = "No Activity",
                                PressedKey = "",
                                UserName = userName,
                                NetWorkActivity = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()
                            });
                        }

                        var jsonString = JsonConvert.SerializeObject(listOfItems);
                        using (var client = new WebClient())
                        {
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            client.Headers[HttpRequestHeader.Accept] = "application/json";
                            var data = Encoding.UTF8.GetBytes(jsonString);
                            var result = client.UploadData("https://observer.360scrm.com/Home/PostActivity", "POST", data);
                        }
                    }
                }
                listOfItems = new List<UserActivityDto>();
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }
        private void AddRecord(DateTime activityTime, string pressedKey, string eventType)
        {

            var pushItem = true;
            foreach (var item in listOfItems)
            {
                DateTime parseDate = DateTime.Now;
                DateTime.TryParse(item.ActivityTime, out parseDate);
                if (parseDate.Minute == activityTime.Minute)
                {
                    item.PressedKey += " " + pressedKey;
                    pushItem = false;
                }
                else pushItem = true;
            }
            if (pushItem)
                listOfItems.Add(new UserActivityDto()
                {
                    ActivityTime = activityTime.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    UserName = userName,
                    PressedKey = pressedKey,
                    ActivityType = eventType,
                    NetWorkActivity = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()
                });
            UnHook();
        }

        private void InitializeTimer()
        {
            ApiTimer.Interval = 300000;
            ApiTimer.Enabled = true;
            ApiTimer.Tick += new System.EventHandler(this.PushActivityStats);
        }
        private void CheckAvailability()
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers[HttpRequestHeader.Accept] = "application/json";
                    client.QueryString.Add("username", userName);
                    string result = client.DownloadString("https://observer.360scrm.com/home/GetUserAvailability");
                    //string result = client.DownloadString("http://localhost:60034/home/GetUserAvailability");
                    var availibilty = JsonConvert.DeserializeObject<List<UserAvailablitiyDto>>(result);
                    foreach (var item in availibilty)
                    {
                        onBreak = item.IsBreak ?? false;
                        checkedIn = item.IsCheckedIn;
                        checkedOut = item.IsCheckedOut;
                        
                    }
                    if (availibilty == null || availibilty.Count == 0)
                    {
                        checkedIn = false;
                        checkedOut = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private void Log(string msg)
        {
            try
            {
                var _Path = Path.GetDirectoryName(@"C:\logging\");
                using (StreamWriter w = File.AppendText(Path.Combine(_Path, "log.txt")))
                {
                    w.Write(Environment.NewLine);
                    w.Write("[{0} {1} {2}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), userName);
                    w.Write("\t");
                    w.WriteLine(" {0}", msg);
                    w.WriteLine("-----------------------");
                }
                
            }
            catch (Exception e)
            {
                //Handle
            }
        }
        private void RefreshHook()
        {
            HookManager.TryUnsubscribeAndSubscribeGlobalEvents();
        }
        private void UnHook()
        {
            HookManager.TryUnsubscribeGlobalEvents();
        }
        private void PushActivityStats(object sender, System.EventArgs e)
        {
            CheckAvailability();
            PostActivities();
            lastActivityDate = DateTime.Now;
            RefreshHook();
        }
    }
}
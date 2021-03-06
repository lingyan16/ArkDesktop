﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Windows.Forms;
using Chromium;

namespace ArkDesktopCSCefOsr
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string nowDir = AppDomain.CurrentDomain.BaseDirectory;
            if(!System.IO.File.Exists(System.IO.Path.Combine(nowDir, "libcfx.dll")))
            {
                if(MessageBox.Show("并没有在软件目录下找到基本运行库\n您应该在获得这款软件的时候同时获得关于本软件依赖库的信息\n如果没有,您可以到本软件的Github Wiki获得帮助\n是否现在前往?",
                                "未能找到依赖库", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/huix-oldcat/ArkDesktop/wiki");
                }
                return;
            }
            CfxRuntime.LibCefDirPath = System.IO.Path.Combine(nowDir, "cef");
            CfxRuntime.LibCfxDirPath = nowDir;
            int exitCode = CfxRuntime.ExecuteProcess(null);
            if (exitCode >= 0)
            {
                Environment.Exit(exitCode);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var settings = new CfxSettings())
            {
                settings.MultiThreadedMessageLoop = true;
                settings.WindowlessRenderingEnabled = true;
                settings.NoSandbox = true;
                settings.ResourcesDirPath = System.IO.Path.Combine(nowDir, "cef", "Resources");
                settings.LocalesDirPath = System.IO.Path.Combine(nowDir, "cef", "Resources", "locales");

                var app = new CfxApp();
                app.OnBeforeCommandLineProcessing += (s, e) =>
                {
                    // optimizations following recommendations from issue #84
                    e.CommandLine.AppendSwitch("disable-gpu");
                    e.CommandLine.AppendSwitch("disable-gpu-compositing");
                    e.CommandLine.AppendSwitch("disable-gpu-vsync");
                };
                if (!CfxRuntime.Initialize(settings, app))
                    Environment.Exit(-1);
            }

            Manager.layeredWindow = new LayeredWindow();
            Manager.control = new ArkDesktopBrowserControl();
            Manager.Init();

            Application.Run(Manager.layeredWindow);

            // CfxRuntime.QuitMessageLoop();
            CfxRuntime.Shutdown();
        }


        //static void Application_Idle(object sender, EventArgs e)
        //{
        //    CfxRuntime.DoMessageLoopWork();
        //}
    }
}

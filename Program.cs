namespace AnalogClock;

using Microsoft.Win32;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

static class Program
{
    [STAThread]
    static void Main()
    {
        var procMutex = new System.Threading.Mutex(true, "_ANALOG_CLOCK_MUTEX", out var result);
        if (!result)
        {
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.Run(new AnalogClockApplicationContext());

        procMutex.ReleaseMutex();
    }
}

public class AnalogClockApplicationContext : ApplicationContext
{
    private const int ANIMATE_TIMER_DEFAULT_INTERVAL = 1000; // ms
    private readonly ToolStripMenuItem startupMenu;
    private readonly NotifyIcon notifyIcon;
    private readonly Timer animateTimer = new();


    public AnalogClockApplicationContext()
    {
        startupMenu = new ToolStripMenuItem("Startup", null, SetStartup!);
        if (IsStartupEnabled())
        {
            startupMenu.Checked = true;
        }

        ContextMenuStrip contextMenuStrip = new ContextMenuStrip(new Container());
        contextMenuStrip.Items.AddRange(new ToolStripItem[]
        {
            startupMenu,
            new ToolStripSeparator(),
            new ToolStripMenuItem($"{Application.ProductName} v{Application.ProductVersion}")
            {
                Enabled = false
            },
            new ToolStripMenuItem("Exit", null, Exit!)
        });

        notifyIcon = new NotifyIcon()
        {
            Icon = GenerateAnalogClockIcon(DateTime.Now),
            ContextMenuStrip = contextMenuStrip,
            Text = DateTime.Now.ToString("HH:mm"),
            Visible = true
        };

        SetAnimation();
        animateTimer.Start();
    }

    private static bool IsStartupEnabled()
    {
        string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
        using RegistryKey rKey = Registry.CurrentUser.OpenSubKey(keyName)!;
        return rKey.GetValue(Application.ProductName) != null;
    }

    private static Icon GenerateAnalogClockIcon(DateTime time)
    {
        var bitmap = new Bitmap(48, 48);
        var g = Graphics.FromImage(bitmap);
        var hourPen = new Pen(Color.White, 5);
        var minutePen = new Pen(Color.White, 3);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        // 外枠
        g.DrawEllipse(new Pen(Color.White, 1), 0, 0, 48, 48);
        // 長針
        g.DrawLine(hourPen, 24, 24, 24 + (int)(Math.Sin(time.Hour * Math.PI / 6) * 15), 24 - (int)(Math.Cos(time.Hour * Math.PI / 6) * 15));
        // 短針
        g.DrawLine(minutePen, 24, 24, 24 + (int)(Math.Sin(time.Minute * Math.PI / 30) * 24), 24 - (int)(Math.Cos(time.Minute * Math.PI / 30) * 24));
        // 中心点
        g.FillEllipse(Brushes.White, 24 - 5, 24 - 5, 10, 10);
        return Icon.FromHandle(bitmap.GetHicon());
    }

    private void SetStartup(object sender, EventArgs e)
    {
        startupMenu.Checked = !startupMenu.Checked;
        string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
        using (RegistryKey rKey = Registry.CurrentUser.OpenSubKey(keyName, true)!)
        {
            if (startupMenu.Checked)
            {
                rKey.SetValue(Application.ProductName, Process.GetCurrentProcess().MainModule!.FileName);
            }
            else
            {
                rKey.DeleteValue(Application.ProductName, false);
            }
            rKey.Close();
        }
    }

    private void Exit(object sender, EventArgs e)
    {
        animateTimer.Stop();
        notifyIcon.Visible = false;
        Application.Exit();
    }

    private void AnimationTick(object sender, EventArgs e)
    {
        notifyIcon.Icon = GenerateAnalogClockIcon(DateTime.Now);
        notifyIcon.Text = DateTime.Now.ToString("HH:mm");
    }

    private void SetAnimation()
    {
        animateTimer.Interval = ANIMATE_TIMER_DEFAULT_INTERVAL;
        animateTimer.Tick += new EventHandler(AnimationTick!);
    }
}
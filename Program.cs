namespace AnalogClock;

using Microsoft.Win32;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

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
    private const int ANIMATE_TIMER_DEFAULT_INTERVAL = 60000; // ms (秒は描画に使わないため分単位で十分)
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

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr handle);

    private static Icon GenerateAnalogClockIcon(DateTime time)
    {
        // OS 側で縮小されるとにじむため、実際に表示される通知領域アイコンのサイズで直接描画する
        Size iconSize = SystemInformation.SmallIconSize;
        int width = iconSize.Width;
        int height = iconSize.Height;
        float centerX = width / 2f;
        float centerY = height / 2f;
        float radius = Math.Min(width, height) / 2f;

        using var bitmap = new Bitmap(width, height);
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // 文字盤(塗りつぶし円): 細い輪郭線よりも極小サイズで潰れにくい
        using var faceBrush = new SolidBrush(Color.FromArgb(255, 0, 120, 215));
        g.FillEllipse(faceBrush, centerX - radius, centerY - radius, radius * 2, radius * 2);

        float hourLength = radius * 0.65f;
        float minuteLength = radius * 0.95f;
        float hourWidth = Math.Max(2f, radius * 0.28f);
        float minuteWidth = Math.Max(2f, radius * 0.22f);
        float dotRadius = Math.Max(1f, radius * 0.15f);

        using var hourPen = new Pen(Color.White, hourWidth);
        using var minutePen = new Pen(Color.Orange, minuteWidth);
        // 長針
        g.DrawLine(hourPen, centerX, centerY,
            centerX + (float)(Math.Sin(time.Hour * Math.PI / 6) * hourLength),
            centerY - (float)(Math.Cos(time.Hour * Math.PI / 6) * hourLength));
        // 短針
        g.DrawLine(minutePen, centerX, centerY,
            centerX + (float)(Math.Sin(time.Minute * Math.PI / 30) * minuteLength),
            centerY - (float)(Math.Cos(time.Minute * Math.PI / 30) * minuteLength));
        // 中心点
        g.FillEllipse(Brushes.Silver, centerX - dotRadius, centerY - dotRadius, dotRadius * 2, dotRadius * 2);
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
                rKey.DeleteValue(Application.ProductName!, false);
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
        var oldIcon = notifyIcon.Icon;
        notifyIcon.Icon = GenerateAnalogClockIcon(DateTime.Now);
        notifyIcon.Text = DateTime.Now.ToString("HH:mm");

        if (oldIcon != null)
        {
            DestroyIcon(oldIcon.Handle);
            oldIcon.Dispose();
        }
    }

    private void SetAnimation()
    {
        animateTimer.Interval = ANIMATE_TIMER_DEFAULT_INTERVAL;
        animateTimer.Tick += new EventHandler(AnimationTick!);
    }
}
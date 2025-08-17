using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using CrossHairJulio;
public partial class ControllerForm
{
    private void InitializeLogic()
    {
        keyboardHook = new KeyboardHook();
        keyboardHook.F2Pressed += OnF2Pressed;
        keyboardHook.F3Pressed += OnF3Pressed;

        if (settings.IsCrosshairVisible)
            ShowCrosshairButton_Click(this, EventArgs.Empty);
    }

    private void OnF2Pressed()
    {
        Program.controllerForm.Visible = !Program.controllerForm.Visible;
    }

    private void OnF3Pressed()
    {
        if (crosshairForm == null || crosshairForm.IsDisposed)
        {
            ShowCrosshairButton_Click(this, EventArgs.Empty);
        }
        else
        {
            crosshairForm.Visible = !crosshairForm.Visible;
            settings.IsCrosshairVisible = crosshairForm.Visible;
            SettingsManager.Save(settings);
        }
    }

    private void InitNotifyIcon()
    {
        trayIcon = new NotifyIcon
        {
            Text = "Crosshair Julio",
            Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "IconJGMira.ico")),
            Visible = true
        };

        trayIcon.MouseClick += (sender, args) =>
        {
            if (args.Button == MouseButtons.Left)
                ToggleControllerForm();
        };

        var trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Cerrar", null, (s, e) =>
        {
            trayIcon.Visible = false;
            Application.Exit();
        });

        trayIcon.ContextMenuStrip = trayMenu;
    }

    private void ToggleControllerForm()
    {
        if (Program.controllerForm == null || Program.controllerForm.IsDisposed)
        {
            Program.controllerForm = new ControllerForm();
            Program.controllerForm.Show();
        }
        else
        {
            var form = Program.controllerForm;

            if (!form.Visible)
                form.Show();

            if (form.WindowState == FormWindowState.Minimized)
                form.WindowState = FormWindowState.Normal;

            form.TopMost = true;
            form.TopMost = false;
            form.BringToFront();
            form.Focus();
        }
    }

    private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }


}

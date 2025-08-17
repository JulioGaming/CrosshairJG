using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using CrossHairJulio;

public partial class ControllerForm
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;

        int windiowWidth = this.Width;

        int rightMargin = 20;
        int topMargin = 20;

        int x = screenWidth - windiowWidth - rightMargin;
        int y = topMargin;

        this.Location = new Point(x, y); // Esquina superior derecha

        BlurHelper.EnableBlur(this.Handle);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        int borderRadius = 30;
        int borderWidth = 3;
        Color borderColor = Color.White; // Cambia por el color que quieras

        using (GraphicsPath path = GetRoundedRectPath(ClientRectangle, borderRadius))
        using (Pen pen = new Pen(borderColor, borderWidth))
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Region = new Region(path); // Esto recorta la ventana a un borde redondeado
            e.Graphics.DrawPath(pen, path); // Esto dibuja el borde
        }
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;
        const int WM_NCHITTEST = 0x84;
        const int HTCLIENT = 1;

        // Bloquea intento de mover la ventana (por teclado, menú del sistema, etc.)
        if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SC_MOVE)
        {
            return;
        }

        // Rechaza cualquier zona arrastrable
        if (m.Msg == WM_NCHITTEST)
        {
            m.Result = (IntPtr)HTCLIENT;
            return;
        }

        base.WndProc(ref m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.Hide();
        }
        else
        {
            keyboardHook.Dispose();
            base.OnFormClosing(e);
        }

        if (crosshairForm != null && !crosshairForm.IsDisposed)
        {
            settings.CrosshairPosition = crosshairForm.Location;
            settings.CrosshairColor = crosshairForm.CrosshairColor.Name;
            settings.CrosshairThickness = crosshairForm.CrosshairThickness;
            settings.CrosshairScale = crosshairForm.CrosshairScale;
            settings.IsCrosshairVisible = crosshairForm.Visible;
        }

        settings.SelectedShape = SelectedShape;
        SettingsManager.Save(settings);
    }
}
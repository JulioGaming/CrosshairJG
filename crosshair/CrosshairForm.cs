using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class CrosshairForm : Form
{
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

    public Color CrosshairColor { get; set; } = Color.Red;
    public int CrosshairThickness { get; set; } = 2;
    public float CrosshairScale { get; set; } = 1.0f;

    public CrosshairShape Shape { get; set; } = CrosshairShape.Cruz;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
    );

    public CrosshairForm()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.BackColor = Color.Black;
        this.TransparencyKey = Color.Black;
        this.StartPosition = FormStartPosition.Manual;
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        this.UpdateStyles();

        this.Size = new Size(200, 200);
        this.Location = new Point(
            (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2,
            (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2
        );

        this.Load += (sender, e) =>
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        };
    }

    public void UpdateCrosshairShape(CrosshairShape shape)
    {
        Shape = shape;
        Invalidate(); // Fuerza repintado
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        e.Graphics.TranslateTransform(0.5f, 0.5f);

        using (Pen pen = new Pen(CrosshairColor, CrosshairThickness))
        {
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;

            int centerX = this.Width / 2;
            int centerY = this.Height / 2;

            int size = (int)(20 * CrosshairScale);
            int radius = (int)(25 * CrosshairScale);
            int dotSize = (int)(8 * CrosshairScale);

            switch (Shape)
            {
                case CrosshairShape.Cruz:
                    e.Graphics.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                    e.Graphics.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
                    break;

                case CrosshairShape.CruzCircular:
                    e.Graphics.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                    e.Graphics.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
                    e.Graphics.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
                    break;

                case CrosshairShape.Punto:
                    e.Graphics.FillEllipse(new SolidBrush(CrosshairColor),
                        centerX - dotSize / 2, centerY - dotSize / 2, dotSize, dotSize);
                    break;

                case CrosshairShape.MiraCircular:
                    int bigRadius = (int)(30 * CrosshairScale);
                    int innerLine = (int)(15 * CrosshairScale);
                    e.Graphics.DrawEllipse(pen, centerX - bigRadius, centerY - bigRadius, bigRadius * 2, bigRadius * 2);
                    e.Graphics.DrawLine(pen, centerX - innerLine, centerY, centerX + innerLine, centerY);
                    e.Graphics.DrawLine(pen, centerX, centerY - innerLine, centerX, centerY + innerLine);
                    break;

                case CrosshairShape.Tactica:
                    int gap = (int)(8 * CrosshairScale);
                    int lineLength = (int)(15 * CrosshairScale);

                    e.Graphics.DrawLine(pen, centerX - lineLength - gap, centerY, centerX - gap, centerY); // Izquierda
                    e.Graphics.DrawLine(pen, centerX + gap, centerY, centerX + lineLength + gap, centerY); // Derecha
                    e.Graphics.DrawLine(pen, centerX, centerY - lineLength - gap, centerX, centerY - gap); // Arriba
                    e.Graphics.DrawLine(pen, centerX, centerY + gap, centerX, centerY + lineLength + gap); // Abajo
                    break;

                case CrosshairShape.PuntoAnillo:
                    int punto = (int)(6 * CrosshairScale);
                    int anillo = (int)(18 * CrosshairScale);

                    e.Graphics.FillEllipse(new SolidBrush(CrosshairColor),
                        centerX - punto / 2, centerY - punto / 2, punto, punto);

                    e.Graphics.DrawEllipse(pen,
                        centerX - anillo, centerY - anillo, anillo * 2, anillo * 2);
                    break;

                case CrosshairShape.FlechaInversa:
                    Point[] flecha = new Point[]
                    {
                        new Point(centerX, centerY - size),
                        new Point(centerX - size / 2, centerY + size / 2),
                        new Point(centerX + size / 2, centerY + size / 2),
                    };
                    e.Graphics.FillPolygon(new SolidBrush(CrosshairColor), flecha);
                    break;

                case CrosshairShape.DobleCruz:
                    int inner = (int)(10 * CrosshairScale);
                    int outer = (int)(20 * CrosshairScale);

                    // Horizontal doble
                    e.Graphics.DrawLine(pen, centerX - outer, centerY - 2, centerX + outer, centerY - 2);
                    e.Graphics.DrawLine(pen, centerX - outer, centerY + 2, centerX + outer, centerY + 2);

                    // Vertical doble
                    e.Graphics.DrawLine(pen, centerX - 2, centerY - outer, centerX - 2, centerY + outer);
                    e.Graphics.DrawLine(pen, centerX + 2, centerY - outer, centerX + 2, centerY + outer);
                    break;

                default:
                    e.Graphics.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                    e.Graphics.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
                    break;
            }
        }
    }
}

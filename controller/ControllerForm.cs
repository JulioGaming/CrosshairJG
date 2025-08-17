using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using CrossHairJulio;
public partial class ControllerForm : Form
{
    private CrosshairForm crosshairForm;
    public event Action<CrosshairShape> ShapeSelected;
    private Button upButton, downButton, leftButton, rightButton, centerButton;
    private Panel buttonPanel;
    private FlowLayoutPanel flowPanel;
    private TableLayoutPanel settingsLayout;
    private Label titleLabel, showControllerLabel, showCrosshairLabel, scaleLabel;
    private ColorDialog colorDialog;
    private NumericUpDown thicknessUpDown;
    private TrackBar scaleSlider;
    private SettingsModel settings;
    public CrosshairShape SelectedShape { get; private set; } = CrosshairShape.Cruz;
    private NotifyIcon trayIcon;
    private KeyboardHook keyboardHook;

    public ControllerForm()
    {
        ConfigureForm();
        InitializeUI();
        InitNotifyIcon();
        InitializeLogic();
    }

    private void ConfigureForm()
    {
        settings = SettingsManager.Load();
        SelectedShape = settings.SelectedShape;

        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "IconJGMira.ico");
        Icon icon = new Icon(iconPath);

        // 🖼️ Propiedades visuales
        this.BackColor = Color.Black;
        this.ControlBox = false;
        this.FormBorderStyle = FormBorderStyle.None;
        this.Icon = icon;
        this.MaximizeBox = false;
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.Size = new Size(250, 550);
        this.TopMost = true;
    }

    private void InitializeUI()
    {
        titleLabel = new Label
        {
            Text = "Crosshair JG",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 10),
        };

        showControllerLabel = new Label
        {
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        };
        showControllerLabel.Paint += (s, e) =>
        {
            string text = "F2 ";
            string rest = "Ocultar o Mostrar Formulario";
            var f2Size = e.Graphics.MeasureString(text, showControllerLabel.Font);
            e.Graphics.DrawString(text, showControllerLabel.Font, Brushes.Blue, 0, 0);
            e.Graphics.DrawString(rest, showControllerLabel.Font, Brushes.White, f2Size.Width, 0);
        };


        showCrosshairLabel = new Label
        {
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),

        };
        showCrosshairLabel.Paint += (s, e) =>
        {
            string text = "F3 ";
            string rest = "Ocultar o Mostrar Mira";
            var f2Size = e.Graphics.MeasureString(text, showControllerLabel.Font);
            e.Graphics.DrawString(text, showControllerLabel.Font, Brushes.Blue, 0, 0);
            e.Graphics.DrawString(rest, showControllerLabel.Font, Brushes.White, f2Size.Width, 0);
        };

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130)); // Para buttonPanel
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

        buttonPanel = CreateButtonPanel();
        settingsLayout = CreateSettingsPanel();

        mainLayout.Controls.Add(buttonPanel, 0, 0);
        mainLayout.Controls.Add(settingsLayout, 0, 1);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1
        };

        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Alto del título
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Alto del hothey
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Alto del hotCrosshair 
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(showControllerLabel, 0, 1);
        rootLayout.Controls.Add(showCrosshairLabel, 0, 2);
        rootLayout.Controls.Add(mainLayout, 0, 3);

        this.Controls.Add(rootLayout);
    }

    private TableLayoutPanel CreateLeftPanel()
    {
        var leftPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 1
        };

        leftPanel.Controls.Add(buttonPanel, 0, 0);
        return leftPanel;
    }

    private Panel CreateButtonPanel()
    {
        var buttonPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0)
        };
        upButton = CreateArrowButton("↑", (s, e) => MoveCrosshair(0, -10));
        downButton = CreateArrowButton("↓", (s, e) => MoveCrosshair(0, 10));
        leftButton = CreateArrowButton("←", (s, e) => MoveCrosshair(-10, 0));
        rightButton = CreateArrowButton("→", (s, e) => MoveCrosshair(10, 0));
        centerButton = CreateArrowButton("×", (s, e) =>
        centerButton.Click += (s, e) =>
        {
            if (crosshairForm != null && !crosshairForm.IsDisposed)
            {
                var center = new Point((Screen.PrimaryScreen.Bounds.Width - crosshairForm.Width) / 2,
                                        (Screen.PrimaryScreen.Bounds.Height - crosshairForm.Height) / 2);
                crosshairForm.Location = center;
                settings.CrosshairPosition = center;
                SettingsManager.Save(settings);
            }
        }
        );

        buttonPanel.Controls.AddRange(new Control[] { upButton, downButton, leftButton, rightButton, centerButton });
        upButton.Location = new Point(35, 0);
        leftButton.Location = new Point(0, 35);
        centerButton.Location = new Point(35, 35);
        rightButton.Location = new Point(70, 35);
        downButton.Location = new Point(35, 70);

        return buttonPanel;
    }

    private TableLayoutPanel CreateSettingsPanel()
    {
        var settingsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5
        };

        colorDialog = new ColorDialog();
        var colorButton = CreateColorButton();

        thicknessUpDown = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 10,
            Value = 2,
            Size = new Size(60, 30)
        };
        thicknessUpDown.ValueChanged += ThicknessUpDown_ValueChanged;

        int savedScaleValue = (int)(settings.CrosshairScale * 100);
        if (savedScaleValue < 1 || savedScaleValue > 200)
            savedScaleValue = 10;

        scaleSlider = new TrackBar
        {
            Minimum = 1,
            Maximum = 200,
            Value = savedScaleValue,
            TickFrequency = 25,
            SmallChange = 10,
            LargeChange = 50,
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(30, 30, 30),  // Fondo oscuro
            ForeColor = Color.LimeGreen,             // Color del texto y ticks
            Height = 40
        };

        scaleLabel = new Label
        {
            Text = $"Tamaño del crosshair: {scaleSlider.Value / 2}%",
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0, 5, 0, 5)
        };

        scaleSlider.ValueChanged += (s, e) =>
        {
            if (crosshairForm != null && !crosshairForm.IsDisposed)
            {
                crosshairForm.CrosshairScale = scaleSlider.Value / 100f;
                settings.CrosshairScale = crosshairForm.CrosshairScale;
                SettingsManager.Save(settings);
                crosshairForm.Invalidate();
                scaleLabel.Text = $"Tamaño del crosshair: {scaleSlider.Value / 2}%";
            }
        };

        flowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true,
            Padding = new Padding()
        };

        foreach (CrosshairShape shape in Enum.GetValues(typeof(CrosshairShape)))
        {
            var bmp = CrosshairRenderer.GetCrosshairBitmap(shape, 64, Color.Black);

            if (bmp != null)
            {
                Button btn = new Button
                {
                    Tag = shape,
                    Width = 32,
                    Height = 32,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.Blue,
                    BackColor = Color.LightGray,
                    Margin = new Padding(4),
                    Image = bmp,
                    ImageAlign = ContentAlignment.MiddleCenter,
                };

                btn.Click += (s, e) =>
                {
                    SelectedShape = (CrosshairShape)btn.Tag;
                    ShapeSelected?.Invoke(SelectedShape);
                    if (crosshairForm != null && !crosshairForm.IsDisposed)
                    {
                        crosshairForm.Shape = SelectedShape;
                        crosshairForm.Invalidate(); // Redibuja el crosshair
                    }
                };

                flowPanel.Controls.Add(btn);
            }
        }

        settingsLayout.Controls.Add(colorButton, 0, 0);
        settingsLayout.Controls.Add(thicknessUpDown, 0, 1);
        settingsLayout.Controls.Add(scaleLabel, 0, 2);
        settingsLayout.Controls.Add(scaleSlider, 0, 3);
        settingsLayout.Controls.Add(flowPanel, 0, 4);

        return settingsLayout;
    }

    private Button CreateColorButton()
    {
        var colorButton = new Button
        {
            BackColor = Color.White,
            ForeColor = Color.Blue,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Text = "Color",
            Width = 120,
            Height = 35,
        };
        colorButton.Click += ColorButton_Click;
        return colorButton;
    }

    private Button CreateArrowButton(string text, EventHandler click)
    {
        int size = 30;
        return new Button
        {
            Text = text,
            Width = size,
            Height = size,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(FontFamily.GenericSansSerif, 12),
            BackColor = Color.LightGray,
            ForeColor = Color.Blue,
            UseVisualStyleBackColor = true
        }.Apply(b => b.Click += click);
    }

    private void ShowCrosshairButton_Click(object sender, EventArgs e)
    {
        if (crosshairForm == null || crosshairForm.IsDisposed)
        {
            crosshairForm = new CrosshairForm
            {
                CrosshairColor = Color.FromName(settings.CrosshairColor ?? "Red"),
                CrosshairThickness = settings.CrosshairThickness,
                CrosshairScale = settings.CrosshairScale,
                Shape = settings.SelectedShape
            };

            if (settings.CrosshairPosition != Point.Empty)
                crosshairForm.Location = settings.CrosshairPosition;

            crosshairForm.Show();
            crosshairForm.TopMost = true;
            //showCrosshairButton.Text = "Ocultar Crosshair";
        }
        else
        {
            crosshairForm.Close();
            crosshairForm = null;
            //showCrosshairButton.Text = "Mostrar Crosshair";
        }

        settings.IsCrosshairVisible = crosshairForm != null;
        SettingsManager.Save(settings);
    }

    private void MoveCrosshair(int dx, int dy)
    {
        if (crosshairForm != null && !crosshairForm.IsDisposed)
        {
            crosshairForm.Left += dx;
            crosshairForm.Top += dy;
            settings.CrosshairPosition = crosshairForm.Location;
            SettingsManager.Save(settings);
            crosshairForm.Invalidate();
        }
    }

    private void ColorButton_Click(object sender, EventArgs e)
    {
        if (colorDialog.ShowDialog() == DialogResult.OK && crosshairForm != null && !crosshairForm.IsDisposed)
        {
            crosshairForm.CrosshairColor = colorDialog.Color;
            settings.CrosshairColor = colorDialog.Color.Name;
            SettingsManager.Save(settings);
            crosshairForm.Invalidate();
        }
    }

    private void ThicknessUpDown_ValueChanged(object sender, EventArgs e)
    {
        if (crosshairForm != null && !crosshairForm.IsDisposed)
        {
            crosshairForm.CrosshairThickness = (int)thicknessUpDown.Value;
            settings.CrosshairThickness = crosshairForm.CrosshairThickness;
            SettingsManager.Save(settings);
            crosshairForm.Invalidate();
        }
    }
}

public static class ControlExtensions
{
    public static T Apply<T>(this T control, Action<T> action)
    {
        action(control);
        return control;
    }
}

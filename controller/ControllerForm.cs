using System;
using System.Drawing;
using System.Windows.Forms;

public class ControllerForm : Form
{
    private CrosshairForm crosshairForm;
    public event Action<CrosshairShape> ShapeSelected;

    private Button showCrosshairButton;
    private Button upButton, downButton, leftButton, rightButton, centerButton;
    private Panel buttonPanel;
    private FlowLayoutPanel flowPanel;

    private ColorDialog colorDialog;
    private Button exitButton;
    private NumericUpDown thicknessUpDown;
    private TrackBar scaleSlider;
    private SettingsModel settings;
    public CrosshairShape SelectedShape { get; private set; } = CrosshairShape.Cruz;

    public ControllerForm()
    {
        settings = SettingsManager.Load();
        SelectedShape = settings.SelectedShape;

        this.Text = "Crosshair JG";
        this.Icon = new Icon("Assets/IconJGMira.ico");
        this.Size = new Size(500, 300);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        InitializeUI();

        if (settings.IsCrosshairVisible)
            ShowCrosshairButton_Click(this, EventArgs.Empty);
    }

    private void InitializeUI()
    {
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        // Left Panel
        var leftPanel = CreateLeftPanel();

        // Right Panel
        var settingsLayout = CreateSettingsPanel();

        mainLayout.Controls.Add(leftPanel, 0, 0);
        mainLayout.Controls.Add(settingsLayout, 1, 0);
        this.Controls.Add(mainLayout);
    }

    private TableLayoutPanel CreateLeftPanel()
    {
        var leftPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1
        };
        leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        showCrosshairButton = new Button { Text = "Mostrar Crosshair", Width = 150 };
        showCrosshairButton.Click += ShowCrosshairButton_Click;

        var buttonPanel = CreateButtonPanel();

        var exitButton = CreateExitButton();

        leftPanel.Controls.Add(showCrosshairButton, 0, 0);
        leftPanel.Controls.Add(buttonPanel, 0, 1);
        leftPanel.Controls.Add(exitButton, 0, 3);
        return leftPanel;
    }

    private Panel CreateButtonPanel()
    {
        var buttonPanel = new Panel { Width = 100, Height = 100 };
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

    private Button CreateExitButton()
    {
        var exitButton = new Button { Text = "Salir", Width = 100 };
        exitButton.Click += (s, e) => this.Close();
        return exitButton;
    }

    private TableLayoutPanel CreateSettingsPanel()
    {
        var settingsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };

        colorDialog = new ColorDialog();
        var colorButton = CreateColorButton();

        thicknessUpDown = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 10,
            Value = 2,
            Size = new Size(60, 30),
        };
        thicknessUpDown.ValueChanged += ThicknessUpDown_ValueChanged;

        scaleSlider = new TrackBar
        {
            Minimum = 1,
            Maximum = 500,
            Value = 100,
            TickFrequency = 25,
            SmallChange = 10,
            LargeChange = 50,
            Dock = DockStyle.Top
        };
        scaleSlider.ValueChanged += (s, e) =>
        {
            if (crosshairForm != null && !crosshairForm.IsDisposed)
            {
                crosshairForm.CrosshairScale = scaleSlider.Value / 100f;
                settings.CrosshairScale = crosshairForm.CrosshairScale;
                SettingsManager.Save(settings);
                crosshairForm.Invalidate();
            }
        };

        flowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true,
            Padding = new Padding(5)
        };

        foreach (CrosshairShape shape in Enum.GetValues(typeof(CrosshairShape)))
        {
            var bmp = CrosshairRenderer.GetCrosshairBitmap(shape, 64, Color.Red);

            if (bmp != null)
            {
                Button btn = new Button
                {
                    Tag = shape,
                    Width = 32,
                    Height = 32,
                    Margin = new Padding(4),
                    Image = bmp,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    Text = "",
                    FlatStyle = FlatStyle.Flat
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
        settingsLayout.Controls.Add(scaleSlider, 0, 2);
        settingsLayout.Controls.Add(flowPanel, 0, 3);

        return settingsLayout;
    }

    private Button CreateColorButton()
    {
        var colorButton = new Button { Text = "Color", Width = 80 };
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
            Font = new Font(FontFamily.GenericSansSerif, 12),
            BackColor = Color.LightGray,
            FlatStyle = FlatStyle.Flat,
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
            showCrosshairButton.Text = "Ocultar Crosshair";
        }
        else
        {
            crosshairForm.Close();
            crosshairForm = null;
            showCrosshairButton.Text = "Mostrar Crosshair";
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

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
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

        base.OnFormClosing(e);
    }
}

// Helper extension
public static class ControlExtensions
{
    public static T Apply<T>(this T control, Action<T> action)
    {
        action(control);
        return control;
    }
}

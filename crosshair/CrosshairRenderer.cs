using System;
using System.Drawing;
using System.Drawing.Drawing2D;

public static class CrosshairRenderer
{
    public static Bitmap GetCrosshairBitmap(CrosshairShape shape, int size, Color color)
    {
        Bitmap bmp = new Bitmap(size, size);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen pen = new Pen(color, 2);
            int center = size / 2;

            switch (shape)
            {
                case CrosshairShape.Cruz:
                    g.DrawLine(pen, center, 0, center, size);
                    g.DrawLine(pen, 0, center, size, center);
                    break;

                case CrosshairShape.CruzCircular:
                    g.DrawLine(pen, center, 0, center, size);
                    g.DrawLine(pen, 0, center, size, center);
                    g.DrawEllipse(pen, center - 10, center - 10, 20, 20);
                    break;

                case CrosshairShape.Punto:
                    g.FillEllipse(new SolidBrush(color), center - 3, center - 3, 6, 6);
                    break;

                case CrosshairShape.MiraCircular:
                    g.DrawEllipse(pen, center - 15, center - 15, 30, 30);
                    g.DrawLine(pen, center, 0, center, size);
                    g.DrawLine(pen, 0, center, size, center);
                    break;

                case CrosshairShape.Tactica:
                    g.DrawRectangle(pen, center - 2, center - 2, 4, 4);
                    g.DrawLine(pen, center, 0, center, center - 6);
                    g.DrawLine(pen, center, size, center, center + 6);
                    g.DrawLine(pen, 0, center, center - 6, center);
                    g.DrawLine(pen, size, center, center + 6, center);
                    break;

                case CrosshairShape.PuntoAnillo:
                    g.FillEllipse(new SolidBrush(color), center - 2, center - 2, 4, 4);
                    g.DrawEllipse(pen, center - 8, center - 8, 16, 16);
                    break;

                case CrosshairShape.FlechaInversa:
                    g.DrawPolygon(pen, new Point[]
                    {
                        new Point(center, 0),
                        new Point(center - 5, 10),
                        new Point(center + 5, 10)
                    });
                    g.DrawLine(pen, center, 10, center, size);
                    break;

                case CrosshairShape.DobleCruz:
                    g.DrawLine(pen, center, 0, center, size);
                    g.DrawLine(pen, 0, center, size, center);
                    g.DrawLine(pen, center - 10, center - 10, center + 10, center + 10);
                    g.DrawLine(pen, center - 10, center + 10, center + 10, center - 10);
                    break;

                case CrosshairShape.FlorCuatroPetalos:
                    int petalRadius = 6; // Radio de cada pétalo
                    int flowerRadius = 14; // Distancia desde el centro hacia cada pétalo

                    for (int i = 0; i < 8; i++)
                    {
                        double angle = i * Math.PI / 2; // 0, 90, 180, 270 grados
                        int petalX = center + (int)(Math.Cos(angle) * flowerRadius);
                        int petalY = center + (int)(Math.Sin(angle) * flowerRadius);

                        g.FillEllipse(new SolidBrush(color),
                            petalX - petalRadius, petalY - petalRadius, petalRadius * 2, petalRadius * 2);
                    }

                    // Centro de la flor (amarillo)
                    g.FillEllipse(Brushes.Yellow,
                        center - petalRadius, center - petalRadius, petalRadius * 2, petalRadius * 2);
                    break;

                case CrosshairShape.Perrito:
                    float scale = size / 100f; // escala relativa al tamaño
                    float cx = center;
                    float cy = center;

                    using (GraphicsPath dogPath = new GraphicsPath())
                    {
                        // Cabeza
                        dogPath.AddEllipse(cx - 20 * scale, cy - 20 * scale, 40 * scale, 40 * scale);

                        // Orejas
                        dogPath.AddEllipse(cx - 30 * scale, cy - 30 * scale, 15 * scale, 25 * scale);
                        dogPath.AddEllipse(cx + 15 * scale, cy - 30 * scale, 15 * scale, 25 * scale);

                        // Hocico
                        dogPath.AddEllipse(cx - 8 * scale, cy + 5 * scale, 16 * scale, 10 * scale);

                        // Ojos
                        dogPath.AddEllipse(cx - 10 * scale, cy - 5 * scale, 4 * scale, 4 * scale);
                        dogPath.AddEllipse(cx + 6 * scale, cy - 5 * scale, 4 * scale, 4 * scale);

                        // Nariz
                        dogPath.AddEllipse(cx - 2 * scale, cy + 5 * scale, 4 * scale, 4 * scale);

                        g.DrawPath(pen, dogPath);
                    }
                    break;

                default:
                    g.DrawLine(pen, center, 0, center, size);
                    g.DrawLine(pen, 0, center, size, center);
                    break;
            }
        }
        return bmp;
    }
}

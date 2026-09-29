using System;
using System.Numerics;
using Raylib_cs;

namespace ColorLab
{
    public struct ColorPaletteItem
    {
        public string Name;
        public Color Color;

        public ColorPaletteItem(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }

    class Program
    {
        static string ColorAHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        static void Main(string[] args)
        {
            const int windowWidth = 960;
            const int windowHeight = 680;

            Raylib.InitWindow(windowWidth, windowHeight, "Practica 3 - Color Lab + Grid 16x16");
            Raylib.SetTargetFPS(60);

            // Paleta temática de 5 colores
            ColorPaletteItem[] palette = new ColorPaletteItem[]
            {
                new ColorPaletteItem("Rojo coral",    new Color(220, 50,  50,  255)),
                new ColorPaletteItem("Azul profundo", new Color(30,  80,  180, 255)),
                new ColorPaletteItem("Verde hoja",    new Color(40,  170, 90,  255)),
                new ColorPaletteItem("Amarillo sol",  new Color(245, 200, 40,  255)),
                new ColorPaletteItem("Morado",        new Color(140, 80,  180, 255))
            };

            int colorIndexA = 0;
            int colorIndexB = 1;
            byte alphaValue = 255;
            bool isDraggingSlider = false;

            Rectangle sliderBounds = new Rectangle(430, 395, 380, 20);

            // --- Configuración de la Retícula / Grid 16x16 ---
            const int gridCols = 16;
            const int gridRows = 16;
            const int cellSize = 13; // 16 * 13 = 208px
            Rectangle gridBounds = new Rectangle(380, 120, gridCols * cellSize, gridRows * cellSize);

            // Matriz para guardar el color pintado en cada celda (por defecto blanco transparente)
            Color[,] pixelGrid = new Color[gridCols, gridRows];
            for (int x = 0; x < gridCols; x++)
            {
                for (int y = 0; y < gridRows; y++)
                {
                    pixelGrid[x, y] = Color.Blank;
                }
            }

            while (!Raylib.WindowShouldClose())
            {
                Vector2 mousePos = Raylib.GetMousePosition();

                // Interacción con la paleta de colores
                for (int i = 0; i < palette.Length; i++)
                {
                    Rectangle cardRect = new Rectangle(30, 120 + i * 65, 300, 55);
                    if (Raylib.CheckCollisionPointRec(mousePos, cardRect))
                    {
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) colorIndexA = i;
                        if (Raylib.IsMouseButtonPressed(MouseButton.Right)) colorIndexB = i;
                    }
                }

                // Interacción de Pintado en el Grid 16x16
                Color activeColorA = palette[colorIndexA].Color;
                Color currentPaintColor = new Color(activeColorA.R, activeColorA.G, activeColorA.B, alphaValue);

                if (Raylib.CheckCollisionPointRec(mousePos, gridBounds))
                {
                    int gx = (int)((mousePos.X - gridBounds.X) / cellSize);
                    int gy = (int)((mousePos.Y - gridBounds.Y) / cellSize);

                    if (gx >= 0 && gx < gridCols && gy >= 0 && gy < gridRows)
                    {
                        // Clic / Arrastrar Izquierdo: Pintar celda
                        if (Raylib.IsMouseButtonDown(MouseButton.Left))
                        {
                            pixelGrid[gx, gy] = currentPaintColor;
                        }
                        // Clic / Arrastrar Derecho: Borrar celda
                        else if (Raylib.IsMouseButtonDown(MouseButton.Right))
                        {
                            pixelGrid[gx, gy] = Color.Blank;
                        }
                    }
                }

                // Control del Slider de Transparencia
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mousePos, sliderBounds))
                {
                    isDraggingSlider = true;
                }
                if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    isDraggingSlider = false;
                }

                if (isDraggingSlider)
                {
                    float clampedX = Math.Clamp(mousePos.X, 430, 810);
                    alphaValue = (byte)Math.Round(((clampedX - 430) / 380.0f) * 255.0f);
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(242, 244, 248, 255));

                // Encabezado
                Raylib.DrawText("COLOR LAB", 30, 20, 28, new Color(30, 30, 30, 255));
                Raylib.DrawText("Selecciona un color y pinta sobre la reticula 16x16.", 30, 55, 16, new Color(90, 90, 90, 255));

                // Panel Izquierdo: Paleta de colores
                Raylib.DrawText("Paleta de cinco colores", 30, 92, 18, new Color(30, 30, 30, 255));
                for (int i = 0; i < palette.Length; i++)
                {
                    int cardX = 30;
                    int cardY = 120 + i * 65;
                    Rectangle cardRect = new Rectangle(cardX, cardY, 300, 55);

                    bool isSelectedA = (colorIndexA == i);
                    bool isSelectedB = (colorIndexB == i);

                    Color cardBg = isSelectedA ? new Color(225, 236, 248, 255) : Color.White;
                    Raylib.DrawRectangleRec(cardRect, cardBg);
                    Raylib.DrawRectangleLinesEx(cardRect, 1, isSelectedA ? new Color(70, 130, 200, 255) : new Color(200, 200, 200, 255));

                    Raylib.DrawRectangle(cardX + 10, cardY + 8, 38, 38, palette[i].Color);
                    Raylib.DrawRectangleLines(cardX + 10, cardY + 8, 38, 38, new Color(0, 0, 0, 40));

                    Raylib.DrawText(palette[i].Name, cardX + 58, cardY + 8, 16, new Color(30, 30, 30, 255));
                    string rgbText = $"RGB({palette[i].Color.R}, {palette[i].Color.G}, {palette[i].Color.B})  {ColorAHex(palette[i].Color)}";
                    Raylib.DrawText(rgbText, cardX + 58, cardY + 28, 12, new Color(100, 100, 100, 255));

                    if (isSelectedA) Raylib.DrawText("A", cardX + 282, cardY + 8, 14, new Color(30, 80, 180, 255));
                    if (isSelectedB) Raylib.DrawText("B", cardX + 282, cardY + 30, 14, new Color(180, 50, 50, 255));
                }

                Raylib.DrawText("Izq: Color A | Der: Color B", 30, 460, 13, new Color(100, 100, 100, 255));

                // --- Panel Derecho: Grid 16x16 + Vista Previa de Color ---
                Raylib.DrawText("Reticula 16x16 para pintar", 380, 92, 20, new Color(30, 30, 30, 255));

                // 1. Dibujar el Grid 16x16
                for (int x = 0; x < gridCols; x++)
                {
                    for (int y = 0; y < gridRows; y++)
                    {
                        int posX = (int)gridBounds.X + x * cellSize;
                        int posY = (int)gridBounds.Y + y * cellSize;

                        // Fondo en damero para contrastar transparencias
                        Color bgChecker = ((x + y) % 2 == 0) ? new Color(230, 230, 230, 255) : Color.White;
                        Raylib.DrawRectangle(posX, posY, cellSize, cellSize, bgChecker);

                        // Celda pintada
                        if (pixelGrid[x, y].A > 0)
                        {
                            Raylib.DrawRectangle(posX, posY, cellSize, cellSize, pixelGrid[x, y]);
                        }

                        // Rejilla de división
                        Raylib.DrawRectangleLines(posX, posY, cellSize, cellSize, new Color(210, 210, 210, 255));
                    }
                }
                Raylib.DrawRectangleLinesEx(gridBounds, 1, new Color(100, 100, 100, 255));

                // 2. Cuadro de vista previa de color sólido al lado del Grid
                int previewX = 610;
                int previewY = 120;
                int previewW = 290;
                int previewH = 208;

                Raylib.DrawRectangle(previewX, previewY, previewW / 2, previewH, new Color(240, 240, 240, 255));
                Raylib.DrawRectangle(previewX + previewW / 2, previewY, previewW / 2, previewH, new Color(40, 40, 40, 255));
                Raylib.DrawRectangle(previewX, previewY, previewW, previewH, currentPaintColor);
                Raylib.DrawRectangleLines(previewX, previewY, previewW, previewH, new Color(100, 100, 100, 255));

                // Slider de Transparencia
                Raylib.DrawText("Transparencia (alpha)", 380, 360, 18, new Color(30, 30, 30, 255));
                int sliderX = 430, sliderY = 400, sliderWidth = 380;
                Raylib.DrawText("0", sliderX - 15, sliderY - 6, 14, new Color(80, 80, 80, 255));
                Raylib.DrawText("255", sliderX + sliderWidth + 10, sliderY - 6, 14, new Color(80, 80, 80, 255));
                Raylib.DrawRectangle(sliderX, sliderY - 2, sliderWidth, 5, new Color(210, 210, 210, 255));

                float knobX = sliderX + (alphaValue / 255.0f) * sliderWidth;
                Raylib.DrawCircle((int)knobX, sliderY, 10, new Color(45, 95, 160, 255));
                Raylib.DrawText($"Alpha: {alphaValue}", sliderX + (sliderWidth / 2) - 35, sliderY + 20, 15, new Color(60, 60, 60, 255));

                // Degradado Interpolado
                Color activeColorB = palette[colorIndexB].Color;
                Raylib.DrawText($"Degradado: {palette[colorIndexA].Name} -> {palette[colorIndexB].Name}", 380, 465, 18, new Color(30, 30, 30, 255));

                int steps = 8, swatchW = 58, swatchH = 55, swatchGap = 8, startX = 380, startY = 495;
                for (int i = 0; i < steps; i++)
                {
                    float t = i / (float)(steps - 1);
                    byte r = (byte)(activeColorA.R + (activeColorB.R - activeColorA.R) * t);
                    byte g = (byte)(activeColorA.G + (activeColorB.G - activeColorA.G) * t);
                    byte b = (byte)(activeColorA.B + (activeColorB.B - activeColorA.B) * t);

                    int posX = startX + i * (swatchW + swatchGap);
                    Raylib.DrawRectangle(posX, startY, swatchW, swatchH, new Color(r, g, b, (byte)255));
                    Raylib.DrawText($"{t:F2}", posX + 12, startY + swatchH + 8, 12, new Color(60, 60, 60, 255));
                    Raylib.DrawText($"{r},{g},{b}", posX + 2, startY + swatchH + 24, 10, new Color(100, 100, 100, 255));
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
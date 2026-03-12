// Global using aliases - maps System.Drawing/WinForms types to SkiaSharp/Avalonia equivalents

// WinForms compatibility enums (from WinFormsEnums.cs)
global using System.Windows.Forms;
global using System.Drawing;
global using System.Drawing.Imaging;

// SkiaSharp types
global using Color = SkiaSharp.SKColor;
global using Bitmap = SkiaSharp.SKBitmap;
global using Image = SkiaSharp.SKBitmap;
global using Point = SkiaSharp.SKPointI;
global using PointF = SkiaSharp.SKPoint;
global using Size = SkiaSharp.SKSizeI;
global using SizeF = SkiaSharp.SKSize;
global using Rectangle = SkiaSharp.SKRectI;
global using RectangleF = SkiaSharp.SKRect;

// Avalonia types
global using Control = Avalonia.Controls.Control;
global using UserControl = Avalonia.Controls.UserControl;
global using Form = Avalonia.Controls.Window;
global using TextBox = Avalonia.Controls.TextBox;
global using RichTextBox = Avalonia.Controls.TextBox;
global using TabControl = Avalonia.Controls.TabControl;
global using TabPage = Avalonia.Controls.TabItem;
global using Keys = Avalonia.Input.Key;

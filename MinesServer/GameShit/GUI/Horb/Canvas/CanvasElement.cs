using System.Drawing;
using MinesServer.GameShit.GUI;

namespace MinesServer.GameShit.GUI.Horb.Canvas
{
    public readonly record struct CanvasElement(CanvasElementType Type, int? OriginX, int? OriginY, int? OffsetX, int? OffsetY, int? SizeX, int? Width, int? Height, byte? Alpha, Button? Content, CanvasElementPivot Pivot = CanvasElementPivot.Default, bool IsBlinking = false)
    {
        public static CanvasElement Image(string url, int? width = null, int? height = null, CanvasElementPivot pivot = CanvasElementPivot.Default, int? originDX = null, int? originDY = null, int? offsetX = null, int? offsetY = null, bool isBlinking = false) => new()
        {
            Type = CanvasElementType.Image,
            OriginX = originDX,
            OriginY = originDY,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Width = width,
            Height = height,
            Pivot = pivot,
            IsBlinking = isBlinking,
            Content = new(url, "nop")
        };

        public static CanvasElement Line(Color color, int? dx = null, int? dy = null, CanvasElementPivot pivot = CanvasElementPivot.Default, int? originDX = null, int? originDY = null, int? offsetX = null, int? offsetY = null, bool isBlinking = false) => new()
        {
            Type = CanvasElementType.Line,
            OriginX = originDX,
            OriginY = originDY,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Width = dx,
            Height = dy,
            Pivot = pivot,
            IsBlinking = isBlinking,
            Content = new(string.Format("{0:x6}", color.ToArgb() & 0xFFFFFF), "nop")
        };

        public static CanvasElement Button(Button button, int? originDX = null, int? originDY = null, int? offsetX = null, int? offsetY = null, CanvasElementPivot pivot = CanvasElementPivot.Default) => new()
        {
            Type = CanvasElementType.Button,
            OriginX = originDX,
            OriginY = originDY,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Pivot = pivot,
            Content = button,
        };

        public static CanvasElement Rect(Color color, int? width = null, int? height = null, CanvasElementPivot pivot = CanvasElementPivot.Default, int? originDX = null, int? originDY = null, int? offsetX = null, int? offsetY = null, bool isBlinking = false) => new()
        {
            Type = CanvasElementType.Rect,
            OriginX = originDX,
            OriginY = originDY,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Width = width,
            Height = height,
            Pivot = pivot,
            IsBlinking = isBlinking,
            Content = new(string.Format("{0:x6}", color.ToArgb() & 0xFFFFFF), "nop")
        };

        public static CanvasElement TextField(string label, int? originDX = null, int? originDY = null, int? offsetX = null, int? offsetY = null, CanvasElementPivot pivot = CanvasElementPivot.Default, bool isBlinking = false) => new()
        {
            Type = CanvasElementType.TextField,
            OriginX = originDX,
            OriginY = originDY,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Pivot = pivot,
            Content = new(label, "nop"),
            IsBlinking = isBlinking
        };

        public static CanvasElement TPButton(Button button, int? originDX = null, int? originDY = null, int? offsetX = null, int? offsetY = null, CanvasElementPivot pivot = CanvasElementPivot.Default) => new()
        {
            Type = CanvasElementType.TPButton,
            OriginX = originDX,
            OriginY = originDY,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Pivot = pivot,
            Content = button
        };

        public static CanvasElement MicroButton(Button button, int? originDX = null, int? originDY = null, int? offsetX = null, int? offsetY = null, CanvasElementPivot pivot = CanvasElementPivot.Default) => new()
        {
            Type = CanvasElementType.MicroButton,
            OriginX = originDX,
            OriginY = originDY,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Pivot = pivot,
            Content = button
        };
    }
}

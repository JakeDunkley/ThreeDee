using SFML.Graphics;

namespace ThreeDee;

public static class OverlayManager
{
    private static readonly Font _font = new("../../../misc/SpaceMono-Regular.ttf");
    public static readonly List<Text> TextObjects = new();

    public static void DrawOverlay()
    {
        foreach (Text text in TextObjects)
        {
            WindowManager.Window.Draw(text);
        }
    }

    public static Text CreateAndAddOverlayTextObject(string text)
    {
        Text textObject = new(text, _font);
        TextObjects.Add(textObject);

        return textObject;
    }
}
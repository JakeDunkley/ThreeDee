using SFML.Window;

namespace ThreeDee;

public static class InputManager
{
    public static readonly Dictionary<Keyboard.Key, bool> IsKeyPressedThisFrame = InitializeIsKeyPressedThisFrame();
    public static readonly Dictionary<Keyboard.Key, bool> IsKeyImpulseThisFrame = InitializeIsKeyImpulseThisFrame();

    private static Dictionary<Keyboard.Key, bool> InitializeIsKeyPressedThisFrame()
    {
        Keyboard.Key[] _enumVals = Enum.GetValues<Keyboard.Key>();

        Dictionary<Keyboard.Key, bool> keyDict = new();

        foreach (Keyboard.Key key in _enumVals)
        {
            keyDict.TryAdd(key, false);
        }

        return keyDict;
    }

    private static Dictionary<Keyboard.Key, bool> InitializeIsKeyImpulseThisFrame()
    {
        Keyboard.Key[] _enumVals = Enum.GetValues<Keyboard.Key>();

        Dictionary<Keyboard.Key, bool> keyDict = new();

        foreach (Keyboard.Key key in _enumVals)
        {
            keyDict.TryAdd(key, false);
        }

        return keyDict;
    }

    public static void PollInput()
    {
        foreach (Keyboard.Key key in IsKeyPressedThisFrame.Keys)
        {
            IsKeyImpulseThisFrame[key] = false;

            if (Keyboard.IsKeyPressed(key) && !IsKeyPressedThisFrame[key])
            {
                IsKeyPressedThisFrame[key] = true;
                IsKeyImpulseThisFrame[key] = true;
            }

            if (!Keyboard.IsKeyPressed(key) && IsKeyPressedThisFrame[key])
            {
                IsKeyPressedThisFrame[key] = false;
            }
        }
    }

    public static void PollInput_Debug()
    {
        foreach (Keyboard.Key key in IsKeyPressedThisFrame.Keys)
        {
            IsKeyImpulseThisFrame[key] = false;

            if (Keyboard.IsKeyPressed(key) && !IsKeyPressedThisFrame[key])
            {
                IsKeyPressedThisFrame[key] = true;
                IsKeyImpulseThisFrame[key] = true;
                Console.WriteLine($"{Enum.GetName(key)} impulse!");
                Console.WriteLine($"{Enum.GetName(key)} was pressed!");
            }

            if (!Keyboard.IsKeyPressed(key) && IsKeyPressedThisFrame[key])
            {
                IsKeyPressedThisFrame[key] = false;
                Console.WriteLine($"{Enum.GetName(key)} was released!");
            }
        }
    }
}
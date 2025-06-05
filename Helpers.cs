namespace ThreeDee;

public static class Helpers
{
    public static long TimeThis<T>(Action<T> action, T arg)
    {
        SFML.System.Clock clock = new();
        clock.Restart();
        action(arg);
        return clock.ElapsedTime.AsMicroseconds();
    }
}
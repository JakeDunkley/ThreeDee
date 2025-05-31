namespace ThreeDee;

public static class TickManager
{
    public const int MicrosecondsPerTick = 1000;
    public const int TicksPerSecond = 1000000 / MicrosecondsPerTick;
    public const double InverseTickRate = 1.0 / TicksPerSecond;

    private static readonly SFML.System.Clock _clock = new();
    private static int _ticksElapsed = 0;
    private static int _ticksThisFrame = 0;

    private static long _microsecondsElapsed = 0;
    private static long _microsecondsThisFrame = 0;
    private static long _microsecondsOverLastFrame = 0;

    public static int TicksElapsed => _ticksElapsed;
    public static int TicksThisFrame => _ticksThisFrame;
    public static double TickDelta => _ticksThisFrame * InverseTickRate;
    public static long MicrosecondsElapsed => _microsecondsElapsed;

    public static void FrameStart()
    {
        _clock.Restart();
        _ticksThisFrame = 0;
    }

    /// <summary>
    /// Should be called at the end of each frame to update the global tick count.
    /// </summary>
    public static void FrameEnd()
    {
        _microsecondsThisFrame = _clock.ElapsedTime.AsMicroseconds() + _microsecondsOverLastFrame;
        _microsecondsElapsed += _microsecondsThisFrame;

        while (_microsecondsThisFrame > MicrosecondsPerTick)
        {
            _microsecondsThisFrame -= MicrosecondsPerTick;
            _ticksThisFrame += 1;
        }

        _microsecondsOverLastFrame = _microsecondsThisFrame;
        _ticksElapsed += _ticksThisFrame;
    }
}
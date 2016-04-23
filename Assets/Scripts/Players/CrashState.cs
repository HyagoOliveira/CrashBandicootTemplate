public class CrashState : PlatformState
{
    public static readonly CrashState SPINNING = new CrashState("Spinning");
    public static readonly CrashState CROUCHING = new CrashState("Crouching");
    public static readonly CrashState CRAWLING = new CrashState("Crawling");
    public static readonly CrashState BELLY_FALL = new CrashState("BellyFall");
    public static readonly CrashState BELLY_RISING = new CrashState("BellyRising");

    protected CrashState(string value)
        : base(value)
    {
    }
}

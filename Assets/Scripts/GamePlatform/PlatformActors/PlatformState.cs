public class PlatformState : ActorState
{
    public static readonly PlatformState RUNNING = new PlatformState("Running");
    public static readonly PlatformState JUMPING = new PlatformState("Jumping");
    public static readonly PlatformState FALLING = new PlatformState("Falling");

    protected PlatformState(string value)
        : base(value)
    {
    }
}

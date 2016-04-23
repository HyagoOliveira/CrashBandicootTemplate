/// <summary>
/// State for actors.
/// Works like a modified Enum. You may inherit this class to create your own 
/// enum state for your specific player.
/// </summary>
public class ActorState
{
    public static readonly ActorState IDLE = new ActorState("Idle");
    public static readonly ActorState WALKING = new ActorState("Walking");

    public string Value { get; private set; }
    

    protected ActorState(string value)
    {
        Value = value;
    }    

    public override string ToString()
    {
        return Value;
    }
}

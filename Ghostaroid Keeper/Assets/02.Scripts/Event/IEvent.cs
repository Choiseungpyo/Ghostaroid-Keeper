
public interface IEvent { }

public class AppNavigateRequestEvent : IEvent
{
    public AppSection Section;
    public AppNavigateRequestEvent(AppSection section) { Section = section; }
}

public class AppExploreMapSelectedEvent : IEvent
{
    public int MapId;
    public MapType MapType;
}
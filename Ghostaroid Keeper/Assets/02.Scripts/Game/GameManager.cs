using UnityEngine;

public enum AppSection
{
    Home,
    Explore,
    Growth,
    Bag,
    Book
}

public class GameManager : Singleton<GameManager>, IEventListener<AppNavigateRequestEvent>
{
    public AppSection Section { get; private set; }

    private void Start()
    {
        Section = AppSection.Home;
        EventDispatcher.Dispatch(new AppNavigateRequestEvent(Section));
    }

    protected override void Awake()
    {
        base.Awake();

        EventDispatcher.RegisterListener(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventDispatcher.UnregisterListener(this);   
    }

    public void OnEvent(AppNavigateRequestEvent appNavigateRequestEvent)
    {
        Section = appNavigateRequestEvent.Section;
    }
}

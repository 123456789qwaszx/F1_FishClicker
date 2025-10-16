public interface IUIEventHandler
{
    bool Handle(UIEventType ui, object payload);
}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public enum ETouchEvent
{
    PointerUp,
    PointerDown,
    Click,
    LongPressed,
    BeginDrag,
    Drag,
    EndDrag,
}

public abstract class UI_Base : MonoBehaviour
{
    public virtual void Init()
    {
        gameObject.name = GetType().Name;
        //gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
    public static void BindEvent(GameObject go, Action<PointerEventData> action = null, ETouchEvent type = ETouchEvent.Click)
    {
        UI_EventHandler evt = ComponentHelper.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case ETouchEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case ETouchEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case ETouchEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case ETouchEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
            case ETouchEvent.BeginDrag:
                evt.OnBeginDragHandler -= action;
                evt.OnBeginDragHandler += action;
                break;
            case ETouchEvent.EndDrag:
                evt.OnEndDragHandler -= action;
                evt.OnEndDragHandler += action;
                break;
            case ETouchEvent.LongPressed:
                evt.OnLongPressHandler -= action;
                evt.OnLongPressHandler += action;
                break;
        }
    }

    
    #region BindHelper
    protected Dictionary<Type, Object[]> _objects = new Dictionary<Type, Object[]>();

    protected void Bind<T>(Type type) where T : Object
    {
        string[] names = Enum.GetNames(type);
        Object[] objects = new Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = ComponentHelper.FindChildObject(gameObject, names[i], true);
            else
                objects[i] = ComponentHelper.FindChildObject<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    protected void BindObjects(Type type) { Bind<GameObject>(type); }
    protected void BindImages(Type type) { Bind<Image>(type); }
    protected void BindTexts(Type type) { Bind<TextMeshProUGUI>(type); }
    protected void BindButtons(Type type) { Bind<Button>(type); }
    protected void BindToggles(Type type) { Bind<Toggle>(type); }
    protected void BindSliders(Type type) { Bind<Slider>(type); }

    protected T Get<T>(int idx) where T : Object
    {
        Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }
    protected Slider GetSlider(int idx)
    {
        Slider ret = Get<Slider>(idx);
        ret.interactable = false;
        return ret;
    }
    #endregion
}
using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    public virtual void Init()
    {
        gameObject.name = GetType().Name;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
}
using UnityEngine;

public static class ComponentHelper
{
    /// <summary>
    /// 찾고싶은 자식 오브젝트의 Transform을 반환함
    /// </summary>
    /// <param name="_parent"></param>
    /// <param name="_findName"></param>
    /// <returns></returns>
    public static Transform TryFindChild(this MonoBehaviour _parent, string _findName)
    {
        var child = FindChild(_parent.transform, _findName);
        if (child == null) Debug.Log($"{_parent.name}에 {_findName}라는 자식 오브젝트가 존재하지 않음");
        return child;
    }
    private static Transform FindChild(Transform _parent, string _findName)
    {
        //특정 이름의 자식을 찾는 재귀 메서드
        Transform findChild = null;
        for (int i = 0; i < _parent.childCount; i++)
        {
            var child = _parent.GetChild(i);
            findChild = child.name == _findName ? child : FindChild(child, _findName);
            if (findChild != null) break;
        }
        return findChild;
    }


    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
    
    
    public static GameObject FindChildObject(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChildObject<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChildObject<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}

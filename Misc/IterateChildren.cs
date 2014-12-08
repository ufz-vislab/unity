using UnityEngine;

// Usage example:
// IterateChildren.Iterate(myRootGameObject, delegate(GameObject go) { go.layer = 10; }, true);
public class IterateChildren
{
    public delegate void ChildHandler(GameObject child);

    public static void Iterate(GameObject gameObject, ChildHandler childHandler, bool recursive)
    {
        DoIterate(gameObject, childHandler, recursive);
    }

    private static void DoIterate(GameObject gameObject, ChildHandler childHandler, bool recursive)
    {
        foreach (Transform child in gameObject.transform)
        {
            childHandler(child.gameObject);
            if (recursive)
                DoIterate(child.gameObject, childHandler, true);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class GameStateSelectionListBase<T> : GameState
{
    public Color itemColorDefault = Color.white;
    public Color itemColorSelected = Color.green;
    public SelectionItemBase<T> selected = null;
    public ScrollRect scroller = null;

    protected System.Action<bool, T> callbackFunc;

    public GameStateSelectionListBase<T> SetupCallback(System.Action<bool, T> callback)
    {
        callbackFunc -= callback;
        callbackFunc += callback;
        return this;
    }

    protected void SetSelected(System.Predicate<T> compareFunction)
    {
        for (int i = 0; i < scroller.content.childCount; i++)
            if (compareFunction(scroller.content.GetChild<SelectionItemBase<T>>(i).data))
                scroller.content.GetChild<SelectionItemBase<T>>(i).OnSelect();
    }

    public virtual void OnConfirm(bool isOk)
    {
        callbackFunc(isOk, selected ? selected.data : default(T));
        base.Back();
    }

    public override void Back()
    {
        OnConfirm(false);
    }
}

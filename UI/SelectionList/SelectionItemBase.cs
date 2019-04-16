using UnityEngine.UI;

public class SelectionItemBase<T> : Base
{
    public T data = default(T);

    public virtual void OnSelect()
    {
        var owner = GetComponentInParent<GameStateSelectionListBase<T>>();
        if (owner.selected != null)
            owner.selected.GetComponent<Image>(true, true).color = owner.itemColorDefault;
        owner.selected = this;
        owner.selected.GetComponent<Image>(true, true).color = owner.itemColorSelected;
    }
}

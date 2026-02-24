using UnityEngine;

public class ItemContainerLink : MonoBehaviour {
    [SerializeField] private Component containerComponent;
    public IItemContainerUI Container => containerComponent as IItemContainerUI;
}

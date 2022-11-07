using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Sprite selectedButton;
    public Sprite normalButton;
    Image image;
    Text text;

    private void Awake()
    {
        selectedButton = Resources.Load<Sprite>("UI/Sprites/Sel_Btn");
        normalButton = Resources.Load<Sprite>("UI/Sprites/Btn");
        image = transform.GetComponent<Image>();
        text = transform.GetComponentInChildren<Text>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        image.sprite = selectedButton;
        text.color = Color.yellow;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        image.sprite = normalButton;
        text.color = Color.white;
    }
}

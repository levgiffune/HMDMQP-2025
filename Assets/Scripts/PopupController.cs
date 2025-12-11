using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject popupPanel;

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}

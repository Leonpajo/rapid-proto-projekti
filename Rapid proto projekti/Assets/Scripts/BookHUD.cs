using UnityEngine;
using UnityEngine.UI;

public class BookHUD : MonoBehaviour
{
    public string bookID;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        image.color = Color.gray;
    }

    public void SetFound()
    {
        image.color = Color.white;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<BookHUD> hudIcons;

    private void Awake()
    {
        Instance = this;
    }

    public void BookFound(string bookID)
    {
        foreach (var icon in hudIcons)
        {
            if (icon.bookID == bookID)
                icon.SetFound();
        }
    }
}
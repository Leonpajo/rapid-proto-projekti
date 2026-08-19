using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<BookHUD> hudIcons;

    public void BookFound(string bookID)
    {
        foreach (var icon in hudIcons)
        {
            if (icon.bookID == bookID)
                icon.SetFound();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BookFound("1");
        if (Input.GetKeyDown(KeyCode.Alpha2)) BookFound("2");
        if (Input.GetKeyDown(KeyCode.Alpha3)) BookFound("3");
        if (Input.GetKeyDown(KeyCode.Alpha4)) BookFound("4");
        if (Input.GetKeyDown(KeyCode.Alpha5)) BookFound("5");
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<BookHUD> hudIcons;
    public GameObject winPanel;
    public AudioSource musicSource;
    private int foundCount = 0;

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

        foundCount++;
        if (foundCount >= hudIcons.Count)
        {
            winPanel.SetActive(true);

            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.EndGame();
            }

            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
    }
}
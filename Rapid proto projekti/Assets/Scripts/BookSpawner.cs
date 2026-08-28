using System.Collections.Generic;
using UnityEngine;

public class BookSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> bookPrefabs;

    void Start()
    {
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        foreach (GameObject book in bookPrefabs)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform chosenPoint = availablePoints[randomIndex];

            Instantiate(book, chosenPoint.position, chosenPoint.rotation);

            availablePoints.RemoveAt(randomIndex);
        }
    }
}
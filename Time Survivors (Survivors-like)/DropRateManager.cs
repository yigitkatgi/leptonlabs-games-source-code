using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DropRateManager;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public List<Drops> drops;

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
        {
            return;
        }

        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>(); // To handle two items being able to drop from same enemy

        foreach (Drops drop in drops)
        {
            if (randomNumber <= drop.dropRate)
            {
                possibleDrops.Add(drop);
            }
        }
        if (possibleDrops.Count > 0) // If there are possible drops 
        {
            Drops drop = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
        }
    }

}

using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetHearts : MonoBehaviour
{
    [SerializeField] private GameObject heart;

    private List<GameObject> hearts = new();
    public void SetHeartsImg(int max)
    {
        Debug.Log(max);
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();
        if (hearts.Count < max || hearts.Count > max)
        {
            for (int i = 0; i < max; i++)
            {
                hearts.Add(Instantiate(heart, gameObject.transform));
            }
        }
    }

    public void ActivateHearts(int amount)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i >= amount)
            {
                hearts[i].GetComponent<Image>().enabled = false;
            }
            else
            {
                hearts[i].GetComponent<Image>().enabled = true;
            }
        }
    }
}

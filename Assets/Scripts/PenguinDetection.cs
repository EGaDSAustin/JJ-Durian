using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PenguinDetection : MonoBehaviour
{
    public TextMeshProUGUI penguinText;

    public int penguinCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Penguin"))
        {
            penguinCount++;
            penguinText.SetText("Penguins\r\n" + penguinCount);
        }
    }
}

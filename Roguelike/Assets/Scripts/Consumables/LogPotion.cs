using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPotion : MonoBehaviour, IConsumable
{
    public void Consume()
    {
        Debug.Log("You've just consumed this potion. Nice");
    }

    public void Consume(CharacterStats stats)
    {
        Debug.Log("You've just consumed this potion. Not nice");
    }
}

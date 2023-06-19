using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class HitAnimation : MonoBehaviour
{
    [SerializeField] PlayerCharacter playerCharacter;

    public void Hit()
    {
        playerCharacter.Hit();
    }
    
}

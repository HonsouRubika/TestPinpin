using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class HitAnimation : MonoBehaviour
{
    [SerializeField] Character character;

    public void Hit()
    {
        character.Hit();
    }
    
}

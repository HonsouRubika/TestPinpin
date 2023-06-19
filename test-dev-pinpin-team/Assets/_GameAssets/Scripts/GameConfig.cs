using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Config", menuName = "Scriptable Objects/Game Config/New Config", order = 0)]
public class GameConfig : ScriptableObject
{
    public float PlayerSpeed;
    public int TreeLifePoints;
}

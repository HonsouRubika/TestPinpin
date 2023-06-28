using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class LightningManager : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] bool isActive = true;
    [SerializeField] float lightningFrequence = 30f;
    private float lastTimeLightningStriked = 0;

    private void Update()
    {
        if(isActive && Time.time >= lastTimeLightningStriked + lightningFrequence)
        {
            GenerateLigthning();
            lastTimeLightningStriked = Time.time;
        }
    }

    [ContextMenu("GenerateLigthning")]
    void GenerateLigthning()
    {
        if (GameManager.Instance.TreeSpawner.Trees.Count == 0) return;

        //get random tree
        Pinpin.Tree unluckyTree = GameManager.Instance.TreeSpawner.Trees[Random.Range(0, GameManager.Instance.TreeSpawner.Trees.Count)];

        unluckyTree.HitByLightning();
    }
}

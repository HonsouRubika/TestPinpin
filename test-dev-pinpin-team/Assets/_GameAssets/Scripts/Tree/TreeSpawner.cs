using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class TreeSpawner : MonoBehaviour
{
    //TODO : make an actual tool
    #region Tool
    [Header("Tree Generator")]
    [SerializeField] Vector3 t_spawnPosition;
    [SerializeField] int t_nbOfTree = 1;
    #endregion

    [Header("References")]
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject treesParent;
    [SerializeField] BoxCollider spawnArea;

    [Header("Properties")]
    [SerializeField] float spawnDelay = 5;
    public List<Pinpin.Tree> Trees = new List<Pinpin.Tree>();


    [ContextMenu("GenerateTree")]
    public void GenerateTree()
    {
        Vector3 treePosition = t_spawnPosition;

        if (t_nbOfTree == 1)
        {
            GameObject newTree = Instantiate(treePrefab, treePosition, treesParent.transform.rotation, treesParent.transform);
            Trees.Add(newTree.GetComponent<Pinpin.Tree>());
        }
        else
        {
            for (int i = 0; i < t_nbOfTree; i++)
            {
                treePosition = t_spawnPosition + (Vector3)Random.insideUnitCircle;
                GameObject newTree = Instantiate(treePrefab, treePosition, treesParent.transform.rotation, treesParent.transform);
                Trees.Add(newTree.GetComponent<Pinpin.Tree>());
            }
        }
    }

    public void SpawnNewTree()
    {
        StartCoroutine(SpawnerClock());
    }

    IEnumerator SpawnerClock()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnTree();
    }

    void SpawnTree()
    {
        Vector3 treePosition = RandomPointInBounds(spawnArea.bounds);
        GameObject newTree = Instantiate(treePrefab, treePosition, treesParent.transform.rotation, treesParent.transform);

        Trees.Add(newTree.GetComponent<Pinpin.Tree>());
    }

    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}

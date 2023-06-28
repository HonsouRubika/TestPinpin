using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class CollectibleSpawner : MonoBehaviour
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
    [SerializeField] BoxCollider treeSpawnArea;
    [SerializeField] GameObject rockPrefab;
    [SerializeField] GameObject rocksParent;
    [SerializeField] BoxCollider rockSpawnArea;

    [Header("Properties")]
    [SerializeField] float treeSpawnDelay = 5;
    public List<Pinpin.Tree> Trees = new List<Pinpin.Tree>();
    [SerializeField] float rockSpawnDelay = 30;


    #region Tree

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
        StartCoroutine(TreeSpawnerClock());
    }

    IEnumerator TreeSpawnerClock()
    {
        yield return new WaitForSeconds(treeSpawnDelay);
        SpawnTree();
    }

    void SpawnTree()
    {
        Vector3 treePosition = RandomPointInBounds(treeSpawnArea.bounds);
        GameObject newTree = Instantiate(treePrefab, treePosition, treePrefab.transform.rotation, treesParent.transform);
        Pinpin.Tree treeScript = newTree.GetComponent<Pinpin.Tree>();

        Trees.Add(treeScript);
        GameManager.Instance.Chopper.NewTreeAppeared(treeScript);
    }

    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    #endregion

    #region Rock

    public void SpawnNewRock()
    {
        StartCoroutine(RockSpawnerClock());
    }

    IEnumerator RockSpawnerClock()
    {
        yield return new WaitForSeconds(treeSpawnDelay);
        SpawnRock();
    }

    void SpawnRock()
    {
        Vector3 rockPosition = RandomPointInBounds(rockSpawnArea.bounds);
        GameObject newTree = Instantiate(rockPrefab, rockPosition, rockPrefab.transform.rotation, rocksParent.transform);
    }

    #endregion
}

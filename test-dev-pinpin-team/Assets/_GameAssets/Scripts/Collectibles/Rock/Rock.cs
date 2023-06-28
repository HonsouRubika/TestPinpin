using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class Rock : Collectible
{

    private void Start()
    {
        DisableOutline();    
    }

    public override void Hit(float strength)
    {
        base.Hit(strength);

        //play wood cut sfx
        GameManager.Instance.AudioManager.PickRockSFX(m_lifePoints);
    }

    protected override void Die()
    {
        //add tree to storage and update ui
        GameManager.Instance.NewRockInStorage();

        //spaw a new rock in 5 sec
        //GameManager.Instance.TreeSpawner.SpawnNewTree();

        base.Die();
    }

}

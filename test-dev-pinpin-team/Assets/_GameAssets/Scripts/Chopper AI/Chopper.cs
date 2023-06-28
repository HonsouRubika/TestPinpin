using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class Chopper : Character
{
    [Header("References")]
    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] List<AudioClip> walkingSFX;
    [SerializeField] protected Rigidbody m_rigidbody;

    [Header("Properties")]
    [SerializeField] float walkingStepAudioDelay;
    float lastTimePlayedWalkingSFX;

    //movement
    private float m_moveSpeed = 0f;
    [SerializeField] float m_moveAcceleration = 3f;

    //IA
    public bool IsPurchased = false;
    private enum ChopperState { Waiting, ReachingTree, Chop, EndChop }
    private ChopperState currentState = ChopperState.Waiting;
    private Pinpin.Tree targetedTree;

    //activates IA via UI
    public void Purchase()
    {
        IsPurchased = true;
        PlayerPrefs.SetInt("IsChopperPurchased", (IsPurchased ? 1 : 0));
    }

    #region Movement

    #region SFX

    public void PlayWalkingSFX()
    {
        lastTimePlayedWalkingSFX = Time.time;
        int randomWalkingSFXId = Random.Range(0, walkingSFX.Count);
        playerAudioSource.clip = walkingSFX[randomWalkingSFXId];
        playerAudioSource.Play();
    }

    #endregion

    public void StopMoving()
    {
        m_rigidbody.velocity = new Vector3(0,0,0);
        m_rigidbody.angularVelocity = new Vector3(0, 0, 0);
        m_moveSpeed = 0f;
        m_lastMagnitude = 0;

        m_lastPosition = transform.position;
        UpdateMovementSpeedAnimation();
    }

    public void Respawn()
    {
        m_lastAttackTime = 0;
        StopMoving();

        targetedTree = null;
        m_targetCollectible = null;
    }

    public void Move()
    {
        //increment speed
        m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_maxSpeed, Time.deltaTime * m_moveAcceleration);
        
        //calculate targeted tree direction
        Vector3 direction = (targetedTree.transform.position - transform.position).normalized;

        //apply rotation
        transform.forward = direction;
        //transform.forward = Vector3.Lerp(transform.position, direction, m_moveSpeed);

        //apply velocity in order to move to desination
        m_rigidbody.velocity = direction * m_moveSpeed;
    }

    #endregion

    #region IA

    public void NewTreeAppeared(Pinpin.Tree newTree)
    {
        if (currentState != ChopperState.Waiting || !IsPurchased) return;

        targetedTree = newTree;
        currentState = ChopperState.ReachingTree;
    }

    public void SearchTree()
    {
        if (GameManager.Instance.TreeSpawner.Trees.Count > 0)
        {
            m_moveSpeed = 0;
            targetedTree = GameManager.Instance.TreeSpawner.Trees[0];
            currentState = ChopperState.ReachingTree;
        }
        else
        {
            targetedTree = null;
            currentState = ChopperState.Waiting;
        }
    }

    public override void Update()
    {
        if (!IsPurchased) return;

        switch (currentState)
        {
            case ChopperState.Waiting:

                StopMoving();
                break;
            case ChopperState.ReachingTree:
                //in case player cuts targeted tree
                if (targetedTree == null)
                {
                    currentState = ChopperState.Waiting;
                    return;
                }
                //chopper AI reached targeted tree
                else if (m_collectiblesInRange.Contains(targetedTree))
                {
                    currentState = ChopperState.Chop;
                    return;
                }
                //targeted tree is burning, go back to waiting state
                if (targetedTree.IsBurning)
                {
                    targetedTree = null;
                    currentState = ChopperState.Waiting;
                    return;
                }

                //alculate and apply movement and rotation
                Move();

                PlayWalkingSFX();

                break;
            case ChopperState.Chop:
                StopMoving();

                if (targetedTree == null || targetedTree.IsBurning)
                {
                    currentState = ChopperState.EndChop;
                    return;
                }

                break;
            case ChopperState.EndChop:

                //look for new tree or switch state to waiting
                SearchTree();
                break;
        }

        //chopping wood
        base.Update();
    }

    #endregion
}

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
    private enum ChopperState { Waiting, ReachingTree, Chop, EndChop }
    private ChopperState currentState = ChopperState.Waiting;
    private Pinpin.Tree targetedTree;

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
        m_targetTree = null;
    }

    public void Move()
    {
        m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_maxSpeed, Time.deltaTime * m_moveAcceleration);
        Vector3 direction = (targetedTree.transform.position - transform.position).normalized;
        //direction.y = 0f;

        //rotation
        transform.forward = direction;
        //transform.forward = Vector3.Lerp(transform.position, direction, m_moveSpeed);

        //position
        m_rigidbody.velocity = direction * m_moveSpeed;
    }

    #endregion

    #region IA

    public void NewTreeAppeared(Pinpin.Tree newTree)
    {
        if (currentState != ChopperState.Waiting) return;

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
        switch (currentState)
        {
            case ChopperState.Waiting:
                Debug.Log("Waiting state");

                StopMoving();
                break;
            case ChopperState.ReachingTree:
                Debug.Log("ReachingTree state");

                //in case player cuts targeted tree
                if (targetedTree == null)
                {
                    currentState = ChopperState.Waiting;
                    return;
                }
                else if (m_treesInRange.Contains(targetedTree))
                {
                    currentState = ChopperState.Chop;
                    return;
                }
                if (targetedTree.IsBurning)
                {
                    targetedTree = null;
                    return;
                }

                //movement and rotation
                Move();

                PlayWalkingSFX();

                break;
            case ChopperState.Chop:
                Debug.Log("Chop state");
                StopMoving();

                if (targetedTree == null || targetedTree.IsBurning)
                {
                    currentState = ChopperState.EndChop;
                    return;
                }

                break;
            case ChopperState.EndChop:
                Debug.Log("EndChop state");

                //look for new tree or switch state to waiting
                SearchTree();
                break;
        }

        base.Update();
    }

    #endregion
}

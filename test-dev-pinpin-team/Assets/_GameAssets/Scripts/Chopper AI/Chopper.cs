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
    private Vector3 m_lastValidPosition = Vector3.zero;
    private float m_moveSpeed = 0f;
    private Vector3 m_targetPosition = Vector3.zero;

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

    private void MoveTo(Vector3 position)
    {
        Vector3 direction = (position - transform.position);

        Vector3 vel = direction / Time.fixedDeltaTime;
        vel.y = m_rigidbody.velocity.y;
        m_rigidbody.velocity = vel;
    }

    private void Move(Vector2 moveDirection)
    {
        Vector3 moveDir = new Vector3(moveDirection.x, 0f, moveDirection.y);

        Vector3 moveOffset = moveDir * Time.fixedDeltaTime * m_moveSpeed;
        moveOffset.y = 0f;

        m_targetPosition = transform.position + moveOffset;

        if (CanWalk(m_targetPosition))
        {
            m_lastValidPosition = transform.position;
            MoveTo(m_targetPosition);
        }
        else
        {
            m_rigidbody.velocity = Vector3.zero;
            transform.position = m_lastValidPosition;
            m_targetPosition = transform.position + moveOffset.z * Vector3.forward;
            if (CanWalk(m_targetPosition))
                MoveTo(m_targetPosition);
            else
            {
                m_targetPosition = transform.position + moveOffset.x * Vector3.right;
                if (CanWalk(m_targetPosition))
                    MoveTo(m_targetPosition);
                else
                {
                    StopMoving();
                    currentState = ChopperState.Waiting;
                }
            }
        }

        if (m_targetTree != null)
        {
            Vector3 treeDir = (m_targetTree.transform.position - transform.position);
            treeDir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, treeDir.normalized, 0.3f);
        }
        else
        {
            transform.forward = Vector3.Lerp(transform.forward, moveDir, 0.3f);
        }
    }

    public void StopMoving()
    {
        m_lastPosition = transform.position;
        m_targetPosition = transform.position;
        m_moveSpeed = 0f;

        m_rigidbody.velocity = new Vector3(0,0,0);
        m_rigidbody.angularVelocity = new Vector3(0, 0, 0);
        UpdateMovementSpeedAnimation();
    }

    public void Respawn()
    {
        m_lastAttackTime = 0;
        m_lastMagnitude = 0;
        m_lastValidPosition = Vector3.zero;
        StopMoving();

        targetedTree = null;
        m_targetTree = null;
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
            targetedTree = GameManager.Instance.TreeSpawner.Trees[0];
            currentState = ChopperState.ReachingTree;
        }
        else
        {
            targetedTree = null;
            currentState = ChopperState.Waiting;
        }
    }

    protected override void FixedUpdate()
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

                //movement
                Vector3 direction = (targetedTree.transform.position - transform.position).normalized;
                direction = new Vector3(direction.x, 0f, direction.z);
                direction *= 0.5f;

                m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_maxSpeed, 0.125f);
                Move(new Vector2(direction.x, direction.z));
                base.FixedUpdate();

                PlayWalkingSFX();

                break;
            case ChopperState.Chop:
                Debug.Log("Chop state");
                StopMoving();

                if (targetedTree == null)
                {
                    currentState = ChopperState.EndChop;
                    return;
                }

                break;
            case ChopperState.EndChop:
                Debug.Log("EndChop state");

                //look for new tree or switch state to waiting
                base.FixedUpdate();
                SearchTree();
                break;
        }

    }

    #endregion
}

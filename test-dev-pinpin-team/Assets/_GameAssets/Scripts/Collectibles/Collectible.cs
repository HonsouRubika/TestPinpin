using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using Pinpin;

public class Collectible : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Outline m_outline;

    [Header("Properties")]
    [SerializeField] protected float m_lifePoints = 3f;
    public Action<Collectible> onDestroy;
    public Transform lookAtTfm;

    public virtual void EnableOutline()
    {
        m_outline.enabled = true;
    }

    public virtual void DisableOutline()
    {
        m_outline.enabled = false;
    }

    public virtual void Hit(float strength)
    {
        m_lifePoints -= strength;

        UpdateState();
    }

    protected virtual void UpdateState()
    {
        if (m_lifePoints <= 0f)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    protected virtual void Die()
    {
        onDestroy?.Invoke(this);
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null)
            return;
        Character character = other.attachedRigidbody.GetComponent<Character>();
        if (character != null)
        {
            character.AddCollectible(this);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null)
            return;
        Character character = other.attachedRigidbody.GetComponent<Character>();
        if (character != null)
        {
            character.RemoveCollectible(this);
        }
    }
}

using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class Tree : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Outline m_outline;
        [SerializeField] Animator animator;
        [SerializeField] ParticleSystem lightningVFX;
        [SerializeField] GameObject burningVFX;

        [Header("Properties")]
        [SerializeField] float m_lifePoints = 3f;
        public bool IsBurning;
        public Transform lookAtTfm;
        public Action<Tree> onDestroy;

        //animation driven
        public void Hit(float strength)
        {
            m_lifePoints -= strength;

            //play wood cut sfx
            GameManager.Instance.AudioManager.CutWoodSFX(m_lifePoints);

            UpdateState();
        }

        public void HitByLightning()
        {
            Debug.Log("Tree hit by lightning");
            IsBurning = true;

            //lightning and burning feedback
            lightningVFX.Play();
            burningVFX.SetActive(true);
        }

        private void Start()
        {
            IsBurning = false;
            m_lifePoints = GameManager.Instance.GameConfig.TreeLifePoints;
            DisableOutline();
        }

        public void EnableOutline()
        {
            m_outline.enabled = true;
        }

        public void DisableOutline()
        {
            m_outline.enabled = false;
        }

        private void UpdateState()
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

        private void Die()
        {
            onDestroy?.Invoke(this);

            //add tree to storage and update ui
            GameManager.Instance.NewTreeInStorage();

            //spaw a new tree in 5 sec
            GameManager.Instance.TreeSpawner.SpawnNewTree();

            GameManager.Instance.TreeSpawner.Trees.Remove(this);

            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsBurning)
                return;

            if (other.attachedRigidbody == null)
                return;
            Character character = other.attachedRigidbody.GetComponent<Character>();
            if (character != null)
            {
                character.AddTree(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsBurning)
                return;

            if (other.attachedRigidbody == null)
                return;
            Character character = other.attachedRigidbody.GetComponent<Character>();
            if (character != null)
            {
                character.RemoveTree(this);
            }
        }
    }
}

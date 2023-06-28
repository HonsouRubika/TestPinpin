using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class Tree : Collectible
    {
        [Header("References")]
        [SerializeField] ParticleSystem lightningVFX;
        [SerializeField] GameObject burningVFX;

        [Header("Properties")]
        public bool IsBurning;

        //animation driven
        public override void Hit(float strength)
        {
            //play wood cut sfx
            GameManager.Instance.AudioManager.CutWoodSFX(m_lifePoints);

            base.Hit(strength);
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

        protected override void Die()
        {
            //add tree to storage and update ui
            GameManager.Instance.NewTreeInStorage();

            //spaw a new tree in 5 sec
            GameManager.Instance.TreeSpawner.SpawnNewTree();

            GameManager.Instance.TreeSpawner.Trees.Remove(this);

            base.Die();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (IsBurning)
                return;

            base.OnTriggerEnter(other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (IsBurning)
                return;

            base.OnTriggerExit(other);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class Character : MonoBehaviour
    {
        [Header("Character References")]
        [SerializeField] Animation chopAnimation;
        [SerializeField] protected Animator m_animator;
        [SerializeField] protected IKTargeter m_animatorIKTargeter;
        [SerializeField] ParticleSystem toolHitParticleSystem;
        [Header("Character Properties")]
        [SerializeField] protected float m_maxSpeed = 1f;
        [SerializeField] protected float m_strength= 1f;
        [SerializeField] protected float m_chopPerSecond = 1f;
        //[SerializeField] protected float m_attackPerSecond = 1f; //for rocks
        protected Vector3 m_lastPosition = Vector3.zero;
        protected float m_lastMagnitude = 0f;
        protected Vector3 m_animationDirection = Vector3.zero;
        protected List<Tree> m_treesInRange = new List<Tree>();
        protected float m_lastAttackTime = 0f;
        protected Tree m_targetTree = null;

        protected virtual void Start ()
        {
            m_lastPosition = transform.position;

            //load game config
            m_maxSpeed = GameManager.Instance.GameConfig.PlayerSpeed;
        }

#region Movement
        protected virtual bool CanWalk ( Vector3 position )
        {
            return true;
        }

        protected virtual bool IsGrounded ( Vector3 targetPos )
        {
            targetPos += Vector3.up;
            RaycastHit hit;
            if (Physics.Raycast(targetPos, -Vector3.up, out hit, 10f, GameManager.Instance.groundLayerMask.value))
            {
                float dst = hit.distance;
                return dst < 10f;
            }
            return false;
        }

        protected virtual void UpdateMovementSpeedAnimation ()
        {
            if (IsGrounded(transform.position))
            {
                Vector3 speed = ((transform.position - m_lastPosition) / Time.fixedDeltaTime / m_maxSpeed);

                float speedMagnitude = speed.magnitude;
                speed = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward, speed, Vector3.up), Vector3.up) * Vector3.forward * speedMagnitude;

                if (speedMagnitude > m_lastMagnitude)
                {
                    m_lastMagnitude = Mathf.Lerp(m_lastMagnitude, speedMagnitude, 0.5f);
                    m_animationDirection = Vector3.Lerp(m_animationDirection, speed, 0.5f);
                }
                else
                {
                    m_lastMagnitude = Mathf.Lerp(m_lastMagnitude, speedMagnitude, 0.125f);
                    m_animationDirection = Vector3.Lerp(m_animationDirection, speed, 0.125f);
                }
            }
            else
            {
                m_lastMagnitude = Mathf.Lerp(m_lastMagnitude, 0f, 0.125f);
                m_animationDirection = Vector3.Lerp(m_animationDirection, Vector3.zero, 0.125f);
            }

            m_animator.SetFloat("VelocityX", m_animationDirection.x);
            m_animator.SetFloat("VelocityY", m_animationDirection.z);
            m_lastPosition = transform.position;
        }
        #endregion

        #region Upgrades


        #region Movement Speed

        public void AddMovementSpeedBoost(float maxSpeedAdded, float boostDuration)
        {
            m_maxSpeed += maxSpeedAdded;

            StartCoroutine(RemoveMovementSeepdBoost(maxSpeedAdded, boostDuration));
        }

        IEnumerator RemoveMovementSeepdBoost(float amountToRemove, float boostDuration)
        {
            yield return new WaitForSeconds(boostDuration);
            m_maxSpeed -= amountToRemove;
        }

        #endregion

        #region Chopping Speed

        public void AddChoppingSpeedBoost(int boostAmount = 1)
        {
            m_chopPerSecond += boostAmount;

            //save change
            PlayerPrefs.SetFloat("ChoppingSpeedMultiplicator", m_chopPerSecond);
            PlayerPrefs.Save();

            //change animation speed
            float previousMultiplicator = m_animator.GetFloat("ChoppingSpeed");
            m_animator.SetFloat("ChoppingSpeed", previousMultiplicator + boostAmount);
        }

        #endregion

        #endregion

        protected virtual void FixedUpdate ()
        {
            UpdateMovementSpeedAnimation();
        }

        public void AddTree(Tree tree)
        {
            if (!m_treesInRange.Contains(tree))
            {
                m_treesInRange.Add(tree);
                tree.onDestroy += RemoveTree;
            }
        }

        public void RemoveTree(Tree tree)
        {
            tree.onDestroy -= RemoveTree;
            m_treesInRange.Remove(tree);
            if (tree == m_targetTree)
            {
                m_targetTree.DisableOutline();
                m_targetTree = null;
            }
        }

        private int SortByDistance(Tree a, Tree b)
        {
            if (a == null || b == null)
            {
                return a == b ? 0 : (a == null ? -1 : 1);
            }
            float dstA = (a.transform.position - transform.position).sqrMagnitude;
            float dstB = (b.transform.position - transform.position).sqrMagnitude;
            return dstA.CompareTo(dstB);
        }

        #region Cut Wood Animation

        //animation event
        public virtual void Hit()
        {
            if (m_targetTree == null) return;

            m_targetTree.Hit(m_strength);

            //play vfx (particle system)
            toolHitParticleSystem.Play();
        }

        private void StartChopAnimation()
        {
            m_animator.SetTrigger("Chop");
        }

        #endregion

        private void Update ()
        {
            if (m_treesInRange.Count > 0)
            {
                //turn on cutting wood anim

                m_treesInRange.Sort(SortByDistance);
                if (m_targetTree != null && m_targetTree != m_treesInRange[0])
                {
                    m_targetTree.DisableOutline();
                }
                m_targetTree = m_treesInRange[0];

                if (m_targetTree.IsBurning)
                {
                    RemoveTree(m_targetTree);
                    m_targetTree = null;
                    return;
                }

                m_targetTree.EnableOutline();
                m_animatorIKTargeter.headTargetTransform = m_targetTree.lookAtTfm;

                //if (Time.time - m_lastAttackTime > m_chopPerSecond)
                if(Time.time >= m_lastAttackTime + (1/m_chopPerSecond))
                {
                    m_lastAttackTime = Time.time;
                    StartChopAnimation();
                }
            }
            else
            {
                //turn off cutting wood anim

                if (m_targetTree != null)
                {
                    m_targetTree.DisableOutline();
                    m_targetTree = null;
                }
                m_animatorIKTargeter.headTargetTransform = null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class Character : MonoBehaviour
    {
        [Header("Character References")]
        [SerializeField] protected Animator m_animator;
        [SerializeField] protected IKTargeter m_animatorIKTargeter;
        [SerializeField] GameObject axe;
        [SerializeField] GameObject pickaxe;
        [SerializeField] ParticleSystem axeHitParticleSystem;
        [SerializeField] ParticleSystem pickaxeHitParticleSystem;

        [Header("Character Properties")]
        [SerializeField] protected float m_maxSpeed = 1f;
        [SerializeField] protected float m_strength = 1f;
        [SerializeField] protected float m_chopPerSecond = 1f;
        public float ChoppingSpeed { get => m_chopPerSecond; set => m_chopPerSecond = value; }
        //[SerializeField] protected float m_attackPerSecond = 1f; //for rocks
        protected Vector3 m_lastPosition = Vector3.zero;
        protected float m_lastMagnitude = 0f;
        protected Vector3 m_animationDirection = Vector3.zero;
        protected List<Collectible> m_collectiblesInRange = new List<Collectible>();
        protected float m_lastAttackTime = 0f;
        protected Collectible m_targetCollectible = null;

        //tools
        public enum Tools { Axe, Pickaxe }
        public Tools CurrentTool = Tools.Axe;

        protected virtual void Start()
        {
            m_lastPosition = transform.position;
        }

        #region Movement
        protected virtual bool CanWalk(Vector3 position)
        {
            return true;
        }

        protected virtual bool IsGrounded(Vector3 targetPos)
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

        protected virtual void UpdateMovementSpeedAnimation()
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


        protected virtual void FixedUpdate()
        {
            UpdateMovementSpeedAnimation();
        }

        public void AddCollectible(Collectible collectible)
        {
            if (!m_collectiblesInRange.Contains(collectible))
            {
                m_collectiblesInRange.Add(collectible);
                collectible.onDestroy += RemoveCollectible;
            }
        }

        public void RemoveCollectible(Collectible collectible)
        {
            collectible.onDestroy -= RemoveCollectible;
            m_collectiblesInRange.Remove(collectible);
            if (collectible == m_targetCollectible)
            {
                m_targetCollectible.DisableOutline();
                m_targetCollectible = null;
            }
        }

        private int SortByDistance(Collectible a, Collectible b)
        {
            if (a == null || b == null)
            {
                return a == b ? 0 : (a == null ? -1 : 1);
            }
            float dstA = (a.transform.position - transform.position).sqrMagnitude;
            float dstB = (b.transform.position - transform.position).sqrMagnitude;
            return dstA.CompareTo(dstB);
        }

        #region Tools

        public void ChangeTool(Tools newTool)
        {
            if (CurrentTool == newTool) return;

            CurrentTool = newTool;

            switch (CurrentTool)
            {
                case Tools.Axe:
                    pickaxe.SetActive(false);
                    axe.SetActive(true);
                    break;
                case Tools.Pickaxe:
                    pickaxe.SetActive(true);
                    axe.SetActive(false);
                    break;
            }
        }

        //animation event
        public virtual void Hit()
        {
            if (m_targetCollectible == null) return;

            m_targetCollectible.Hit(m_strength);

            //play vfx (particle system)
            if (CurrentTool == Tools.Axe)
            {
                axeHitParticleSystem.Play();
            }
            else
            {
                pickaxeHitParticleSystem.Play();
            }
        }

        private void StartChopAnimation()
        {
            m_animator.SetTrigger("Chop");
        }

        private void StartPickAnimation()
        {
            m_animator.SetTrigger("Pick");
        }

        #endregion

        public virtual void Update()
        {
            if (m_collectiblesInRange.Count > 0)
            {
                //turn on collecting anim

                m_collectiblesInRange.Sort(SortByDistance);
                if (m_targetCollectible != null && m_targetCollectible != m_collectiblesInRange[0])
                {
                    m_targetCollectible.DisableOutline();
                }
                m_targetCollectible = m_collectiblesInRange[0];

                if (m_targetCollectible is Pinpin.Tree)
                {
                    Tree tree = (Tree)m_targetCollectible;
                    if (tree.IsBurning)
                    {
                        RemoveCollectible(m_targetCollectible);
                        m_targetCollectible = null;
                        return;
                    }
                }

                m_targetCollectible.EnableOutline();
                m_animatorIKTargeter.headTargetTransform = m_targetCollectible.lookAtTfm;

                //if (Time.time - m_lastAttackTime > m_chopPerSecond)
                if (Time.time >= m_lastAttackTime + (1 / m_chopPerSecond))
                {
                    m_lastAttackTime = Time.time;

                    //switch tool and animation according to current collectible kind
                    if (m_targetCollectible is Tree)
                    {
                        StartChopAnimation();
                        ChangeTool(Tools.Axe);
                    }
                    else if (m_targetCollectible is Rock)
                    {
                        StartPickAnimation();
                        ChangeTool(Tools.Pickaxe);
                    }
                }
            }
            else
            {
                //turn off collecting anim

                if (m_targetCollectible != null)
                {
                    m_targetCollectible.DisableOutline();
                    m_targetCollectible = null;
                }
                m_animatorIKTargeter.headTargetTransform = null;
            }
        }
    }
}

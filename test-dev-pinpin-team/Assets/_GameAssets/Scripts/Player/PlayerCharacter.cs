using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerCharacter : Character
    {
        [Header("References")]
        [SerializeField] AudioSource playerAudioSource;
        [SerializeField] List<AudioClip> walkingSFX;
        [SerializeField] protected Rigidbody m_rigidbody;

        [Header("Properties")]
        [SerializeField] float walkingStepAudioDelay;
        float lastTimePlayedWalkingSFX;

        //movement
        private Vector2 m_lastJoystickDirection = Vector2.zero;
        private Vector3 m_lastValidPosition = Vector3.zero;
        private float m_moveSpeed = 0f;
        private Vector3 m_targetPosition = Vector3.zero;

        private void Reset ()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        protected override void Start ()
        {
            base.Start();
            m_targetPosition = transform.position;
            m_lastValidPosition = transform.position;
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

        private void MoveTo ( Vector3 position )
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
                        StopMoving();
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

        private void StopMoving()
        {
            m_targetPosition = transform.position;
            m_lastJoystickDirection = Vector2.zero;
            m_moveSpeed = 0f;

            m_rigidbody.velocity = new Vector3(0f, m_rigidbody.velocity.y, 0f);
        }

        #endregion

        protected override void FixedUpdate ()
        {
            //movement
            if (Joystick.Instance.moveDirection != Vector2.zero)
            {
                m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_maxSpeed, 0.125f);
                Move(Joystick.Instance.moveDirection);
                m_lastJoystickDirection = Joystick.Instance.moveDirection;
            }
            else
            {
                m_lastJoystickDirection = Vector2.Lerp(m_lastJoystickDirection, Vector2.zero, 0.3f);
                m_moveSpeed = Mathf.Lerp(m_moveSpeed, 0f, 0.3f);
                if (m_lastJoystickDirection == Vector2.zero)
                {
                    StopMoving();
                }
                else
                {
                    Vector3 forward = transform.forward;
                    Move(m_lastJoystickDirection);
                    if (m_targetTree != null)
                    {
                        Vector3 treeDir = (m_targetTree.transform.position - transform.position);
                        treeDir.y = 0;
                        transform.forward = Vector3.Lerp(forward, treeDir.normalized, 0.3f);
                    }
                    else
                    {
                        transform.forward = forward;
                    }
                }
            }
            base.FixedUpdate();

            if (m_rigidbody.velocity.y > 0)
            {
                m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, m_rigidbody.velocity.y / 2f, m_rigidbody.velocity.z);
            }

            //sfx
            if (!playerAudioSource.isPlaying && m_moveSpeed > 1 && Time.time >= lastTimePlayedWalkingSFX + walkingStepAudioDelay)
            {
                PlayWalkingSFX();
            }
        }
    }
}

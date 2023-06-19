using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    [RequireComponent(typeof(Animator))]
    public class IKTargeter : MonoBehaviour
    {
        [SerializeField]
        private Animator m_animator;
        private float m_headIkWeight = 0f;
        [HideInInspector]
        public Transform headTargetTransform = null;
        private Vector3 m_lastPos;

        private void Reset ()
        {
            m_animator = GetComponent<Animator>();
        }

        private void FixedUpdate ()
        {
            m_headIkWeight = Mathf.Lerp(m_headIkWeight, headTargetTransform == null ? 0f : 1f, 0.125f);
        }

        private void OnAnimatorIK ( int layerIndex )
        {
            if (headTargetTransform != null)
            {
                m_lastPos = headTargetTransform.transform.position;
            }
            if (m_headIkWeight > 0f)
            {
                m_animator.SetLookAtPosition(m_lastPos);
            }
            m_animator.SetLookAtWeight(m_headIkWeight);
        }
    }
}

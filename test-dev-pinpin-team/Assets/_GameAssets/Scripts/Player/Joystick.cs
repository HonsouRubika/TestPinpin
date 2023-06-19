using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField] private float m_screenPercentSize = 0.1f;

        private bool m_dragging = false;
        private Vector2 m_startPos;
        private float m_maxDist;
        private static Joystick m_instance;
        public static Joystick Instance => m_instance;

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private Vector2 m_moveDirection = Vector2.zero;
        public Vector2 moveDirection {
            get => m_moveDirection;
            private set
            {
                Vector3 camForward = GameManager.mainCamera.transform.forward;
                camForward.y = 0f;
                Vector3 valueVector = Quaternion.LookRotation(camForward, Vector3.up) * new Vector3(value.x, 0f, value.y);
                m_moveDirection = new Vector2(valueVector.x, valueVector.z);
            }
        }

        private void Start ()
        {
            m_maxDist = Mathf.Min(Screen.width, Screen.height) * m_screenPercentSize;
        }

        private void Update ()
        {
            if (Input.GetMouseButton(0) != m_dragging)
            {
                m_startPos = Input.mousePosition;
                m_dragging = !m_dragging;
            }
            else if (m_dragging)
            {
                moveDirection = Vector2.ClampMagnitude((Vector2)Input.mousePosition - m_startPos, m_maxDist) / m_maxDist;
            }
            else
            {
                moveDirection = Vector2.zero;
            }
        }
    }
}

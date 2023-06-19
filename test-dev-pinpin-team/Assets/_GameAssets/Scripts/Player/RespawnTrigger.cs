using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class RespawnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter ( Collider other )
        {
            if (other.attachedRigidbody != null)
            {
                Character character = other.attachedRigidbody.GetComponent<Character>();
                if (character != null)
                {
                    character.transform.position = Vector3.zero;
                }
            }
        }
    }
}

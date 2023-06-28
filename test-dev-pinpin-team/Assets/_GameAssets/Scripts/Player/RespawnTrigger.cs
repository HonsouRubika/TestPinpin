using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class RespawnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null)
            {
                PlayerCharacter character = other.attachedRigidbody.GetComponent<PlayerCharacter>();
                Chopper chopper = other.attachedRigidbody.GetComponent<Chopper>();

                if (character != null)
                {
                    Debug.Log("respawn player");
                    character.transform.position = new Vector3(0, 0, 0);
                }
                if (chopper != null)
                {
                    Debug.Log("respawn IA");
                    chopper.transform.position = new Vector3(0, 0, 0);
                    chopper.Respawn();
                }
            }
        }
    }
}

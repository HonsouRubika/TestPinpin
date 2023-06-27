using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinpin;

public class SpeedBoost : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject parent;

    [Header("Properties")]
    [SerializeField] float boostAmount = 2;
    [SerializeField] float boostDuration = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null)
            return;

        PlayerCharacter character = other.attachedRigidbody.GetComponent<PlayerCharacter>();
        if (character != null)
        {
            character.AddMovementSpeedBoost(boostAmount, boostDuration);

            Destroy(parent.gameObject);
        }
    }
}

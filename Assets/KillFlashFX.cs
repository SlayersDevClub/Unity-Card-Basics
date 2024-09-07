using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFlashFX : MonoBehaviour
{
    public Light MuzzleFlashLight;

    void Start()
    {
        if(MuzzleFlashLight)MuzzleFlashLight.gameObject.SetActive(false);

    }
    public void AIKillPlayer()
    {
        BlackjackUIManager.Instance.AIKillPlayerFlash();
    }

    public void PlayerKillAI()
    {
        BlackjackUIManager.Instance.PlayerKilledFlash();
    }

    public void FlashLight()
    {
        if (MuzzleFlashLight) MuzzleFlashLight.gameObject.SetActive(true);
    }
}

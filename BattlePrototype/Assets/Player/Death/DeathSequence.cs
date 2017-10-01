﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSequence : MonoBehaviour
{
    public static IEnumerator Start(DarknessOverlay darkness, AnimationController playerAnimator, SpriteRenderer playerSpriteRenderer, Flasher damageFlasher)
    {
        damageFlasher.enabled = false;
        Time.timeScale = 0.0f;

        float spinTime = 4.0f;
        float spinRate = 16.0f;
        float endSpinRate = 2.0f;
        float spinRateChange = (endSpinRate - spinRate) / spinTime;
        float angle = 0.0f;

        float startRadius = 10.0f;
        float endRadius = 0.0f;
        float darkesRadius = startRadius;
        float radiusChangeRage = (endRadius - startRadius) / spinTime;

        playerSpriteRenderer.sortingLayerName = "Overlay";

        darkness.gameObject.SetActive(true);

        playerAnimator.SetFloat("Distance", 0.0f);

        while (spinTime > 0.0f)
        {
            playerAnimator.SetInteger("Direction", Mathf.FloorToInt(angle));
            angle += spinRate * Time.unscaledDeltaTime;
            spinRate += spinRateChange * Time.unscaledDeltaTime;
            spinTime -= Time.unscaledDeltaTime;

            darkness.UpdateValues(playerAnimator.transform.position, darkesRadius);

            darkesRadius += radiusChangeRage * Time.unscaledDeltaTime;

            yield return null;
        }

        playerAnimator.SetInteger("Direction", 0);

        yield return new WaitForSecondsRealtime(1.0f);

        GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldController>().SwitchTo(MapDirections.Dead);
        playerAnimator.SetBool("Dead", true);

        yield return new WaitForSecondsRealtime(1.0f);

        while (darkesRadius < startRadius)
        {
            darkesRadius -= 2.0f * radiusChangeRage * Time.unscaledDeltaTime;
            darkness.UpdateValues(playerAnimator.transform.position, darkesRadius);
            yield return null;
        }

        playerAnimator.SetBool("Dead", false);
        darkness.gameObject.SetActive(false);
        playerSpriteRenderer.sortingLayerName = "Default";
        Time.timeScale = 1.0f;
        yield return null;

        yield return StoryFunctionBindings.GetBindings().interact("on_player_death");
    }
}
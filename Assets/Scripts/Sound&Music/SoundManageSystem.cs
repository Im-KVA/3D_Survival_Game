using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManageSystem : MonoBehaviour
{
    public static SoundManageSystem Instance { get; set; }

    //Player
    public AudioSource walkingSound;

    //Inventoy & Storeage
    public AudioSource itemSound;
    public AudioSource itemInteractSound;
    public AudioSource eatItemSound;

    //Crafting
    public AudioSource craftingSound;

    //Building
    public AudioSource changeBuildingSound;
    public AudioSource buildSound;

    //Others
    public AudioSource useAxeSound;
    public AudioSource lootingSound;

    public AudioSource enemyDeathSound;

    //GameMusic
    public AudioSource startingForestMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySound(AudioSource soundToPlay)
    {
        if (!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
        }
    }
}

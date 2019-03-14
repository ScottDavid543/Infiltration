using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerProgressController : MonoBehaviour {

    private static PlayerProgressController instance;
    public bool[] LevelsUnlocked;
    public int SkillPoints;
    
    // Weapon upgrade variables
    public int playerAmmo;
    public float shootSpeed;
    public int ammoPickupAmount;
    public bool suppressorActive;
    public float shotSoundDistance;

    // Player upgrade variables
    public float playerHealth;
    public float rollCooldown;
    public float throwSpeed;
    public float footstepNoise;
    public float footstepVolume;

    public bool[] weaponUpgrades;
    public bool[] playerUpgrades;

    public bool[] collectedPoints;

    private void Awake()
    {
        LevelsUnlocked[0] = true;

        DontDestroyOnLoad(transform.gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        for (int i = 0; i < data.LevelsUnlocked.Length; i++)
        {
            LevelsUnlocked[i] = data.LevelsUnlocked[i];
        }
        SkillPoints = data.SkillPoints;

        playerAmmo = data.playerAmmo;
        shootSpeed = data.shootSpeed;
        ammoPickupAmount = data.ammoPickupAmount;
        suppressorActive = data.suppressorActive;
        shotSoundDistance = data.shotSoundDistance;

        playerHealth = data.playerHealth;
        rollCooldown = data.rollCooldown;
        throwSpeed = data.throwSpeed;
        footstepNoise = data.footstepNoise;
        footstepVolume = data.footstepVolume;

        for (int i = 0; i < data.weaponUpgrades.Length; i++)
        {
            weaponUpgrades[i] = data.weaponUpgrades[i];
        }
        for (int i = 0; i < data.playerUpgrades.Length; i++)
        {
            playerUpgrades[i] = data.playerUpgrades[i];
        }

        for (int i = 0; i < data.collectedPoints.Length; i++)
        {
            collectedPoints[i] = data.collectedPoints[i];
        }
    }

}

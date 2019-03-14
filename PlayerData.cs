using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData  {
    // data to be gathered and saved

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

    public PlayerData(PlayerProgressController progControl)
    {
        LevelsUnlocked = new bool[progControl.LevelsUnlocked.Length];
        for (int i = 0; i < progControl.LevelsUnlocked.Length; i++)
        {            
            LevelsUnlocked[i] = progControl.LevelsUnlocked[i];            
        }
        SkillPoints = progControl.SkillPoints;

        playerAmmo = progControl.playerAmmo;
        shootSpeed = progControl.shootSpeed;
        ammoPickupAmount = progControl.ammoPickupAmount;
        suppressorActive = progControl.suppressorActive;
        shotSoundDistance = progControl.shotSoundDistance;

        playerHealth = progControl.playerHealth;
        rollCooldown = progControl.rollCooldown;
        throwSpeed = progControl.throwSpeed;
        footstepNoise = progControl.footstepNoise;
        footstepVolume = progControl.footstepVolume;

        weaponUpgrades = new bool[progControl.weaponUpgrades.Length];
        for (int i = 0; i <progControl.weaponUpgrades.Length; i++)
        {
            weaponUpgrades[i] = progControl.weaponUpgrades[i];
        }

        playerUpgrades = new bool[progControl.playerUpgrades.Length];
        for (int i = 0; i < progControl.playerUpgrades.Length; i++)
        {
            playerUpgrades[i] = progControl.playerUpgrades[i];
        }

        collectedPoints = new bool[progControl.collectedPoints.Length];
        for (int i = 0; i < progControl.collectedPoints.Length; i++)
        {
            collectedPoints[i] = progControl.collectedPoints[i];
        }
    }
}

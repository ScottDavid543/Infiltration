using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades :MonoBehaviour {

    public GameObject[] weaponButtons;
    public GameObject[] playerButtons;

    public GameObject[] weaponUpgradedIcon;
    public GameObject[] playerUpgradedIcon;

    public Text points;

    // script that exists on each scene containing values made from upgrades
    PlayerProgressController progController;

    public void Awake()
    {
        progController = FindObjectOfType<PlayerProgressController>();
    }
    private void Update()
    {
        for (int i = 0; i < progController.weaponUpgrades.Length; i++)
        {
            if (progController.weaponUpgrades[i] == true)
            {
                weaponUpgradedIcon[i].SetActive(true);
                weaponButtons[i].SetActive(false);
                if (i!=4)
                {
                    weaponButtons[i + 1].SetActive(true);
                }
            }
        }
        for (int i = 0; i < progController.playerUpgrades.Length; i++)
        {
            if (progController.playerUpgrades[i] == true)
            {
                playerUpgradedIcon[i].SetActive(true);
                playerButtons[i].SetActive(false);
                if (i!=4)
                {
                    playerButtons[i + 1].SetActive(true);
                }
            }
        }
        points.text = "POINTS: " + progController.SkillPoints;
        
    }
    

    public void WeaponUpgrade1()
    {
        if (progController.SkillPoints > 0)
        {
            weaponUpgradedIcon[0].SetActive(true);

            weaponButtons[0].SetActive(false);
            weaponButtons[1].SetActive(true);

            progController.weaponUpgrades[0] = true;

            progController.playerAmmo = progController.playerAmmo + 2;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void WeaponUpgrade2()
    {
        if (progController.SkillPoints > 0)
        {
            weaponUpgradedIcon[1].SetActive(true);

            weaponButtons[1].SetActive(false);
            weaponButtons[2].SetActive(true);

            progController.weaponUpgrades[1] = true;

            progController.shootSpeed = 0.5f;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void WeaponUpgrade3()
    {
        if (progController.SkillPoints > 0)
        {
            weaponUpgradedIcon[2].SetActive(true);

            weaponButtons[2].SetActive(false);
            weaponButtons[3].SetActive(true);

            progController.weaponUpgrades[2] = true;

            progController.playerAmmo = progController.playerAmmo + 2;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void WeaponUpgrade4()
    {
        if (progController.SkillPoints > 0)
        {
            weaponUpgradedIcon[3].SetActive(true);

            weaponButtons[3].SetActive(false);
            weaponButtons[4].SetActive(true);

            progController.weaponUpgrades[3] = true;

            progController.ammoPickupAmount = progController.ammoPickupAmount + 3;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void WeaponUpgrade5()
    {
        if (progController.SkillPoints > 0)
        {
            weaponUpgradedIcon[4].SetActive(true);

            weaponButtons[4].SetActive(false);

            progController.weaponUpgrades[4] = true;

            progController.suppressorActive = true;
            progController.shotSoundDistance = 15f;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void PlayerUpgrade1()
    {
        if (progController.SkillPoints > 0)
        {
            playerUpgradedIcon[0].SetActive(true);

            playerButtons[0].SetActive(false);
            playerButtons[1].SetActive(true);

            progController.playerUpgrades[0] = true;

            progController.playerHealth = progController.playerHealth + 20;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void PlayerUpgrade2()
    {
        if (progController.SkillPoints > 0)
        {
            playerUpgradedIcon[1].SetActive(true);

            playerButtons[1].SetActive(false);
            playerButtons[2].SetActive(true);

            progController.playerUpgrades[1] = true;

            progController.rollCooldown = 2f;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void PlayerUpgrade3()
    {
        if (progController.SkillPoints > 0)
        {
            playerUpgradedIcon[2].SetActive(true);

            playerButtons[2].SetActive(false);
            playerButtons[3].SetActive(true);

            progController.playerUpgrades[2] = true;

            progController.playerHealth = progController.playerHealth + 20;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void PlayerUpgrade4()
    {
        if (progController.SkillPoints > 0)
        {
            playerUpgradedIcon[3].SetActive(true);

            playerButtons[3].SetActive(false);
            playerButtons[4].SetActive(true);

            progController.playerUpgrades[3] = true;

            progController.throwSpeed = 25f;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }
    public void PlayerUpgrade5()
    {
        if (progController.SkillPoints > 0)
        {
            playerUpgradedIcon[4].SetActive(true);
            playerButtons[4].SetActive(false);

            progController.playerUpgrades[4] = true;

            progController.footstepNoise = 7;
            progController.footstepVolume = 0.01f;
            progController.SkillPoints = progController.SkillPoints - 1;
        }
    }

}

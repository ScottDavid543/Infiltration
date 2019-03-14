using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perception;

public class GunLogic : MonoBehaviour
{
    // The Bullet Prefab
    [SerializeField]
    GameObject m_BulletPrefab;

    // The Explosive Bullet Prefab
    [SerializeField]
    GameObject m_ExplosiveBulletPrefab;

    // The Bullet Spawn Point
    [SerializeField]
    Transform m_BulletSpawnPoint;

    public float m_ShotCooldown;

    bool m_CanShoot = true;

    // VFX
    [SerializeField]
    ParticleSystem m_Flare;

    [SerializeField]
    ParticleSystem m_Smoke;

    [SerializeField]
    ParticleSystem m_Sparks;

    // SFX
    [SerializeField]
    AudioClip m_BulletShot;

    [SerializeField]
    AudioClip m_GrenadeLaunched;

    // The AudioSource to play Sounds for this object
    AudioSource m_AudioSource;

   public int m_BulletAmmo;

    [SerializeField]
    int m_GrenadeAmmo = 5;

    UIManager m_UIManager;

    PerceptionManager pm;

    public GameObject player;

    GameController gameController;

    PlayerProgressController progController;
    public GameObject Suppressor;
    float shotSoundDistance;

    // Use this for initialization
    void Start ()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_UIManager = FindObjectOfType<UIManager>();
        pm = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<PerceptionManager>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        progController = FindObjectOfType<PlayerProgressController>();

        m_BulletAmmo = progController.playerAmmo;
        m_ShotCooldown = progController.shootSpeed;
        shotSoundDistance = progController.shotSoundDistance;

        // Update UI
        if (m_UIManager)
        {
            m_UIManager.SetAmmoText(m_BulletAmmo, m_GrenadeAmmo);
        }
        if (progController.suppressorActive)
        {
            Suppressor.gameObject.SetActive(true);
            m_AudioSource.volume = m_AudioSource.volume / 2;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (PauseMenu.GameIsPaused)
        {
            return;
        }
        if (!gameController.gameStarted)
        {
            return;
        }
        if (!m_CanShoot)
        {
            m_ShotCooldown -= Time.deltaTime;
            if (m_ShotCooldown < 0.0f)
            {
                m_CanShoot = true;
                m_ShotCooldown = progController.shootSpeed;
            }
        }

        if (m_CanShoot)
        {
            if(Input.GetButtonDown("Fire1") && m_BulletAmmo > 0)
            {
                Fire();
                m_CanShoot = false;
            }
            else if (Input.GetButtonDown("Fire2") && m_GrenadeAmmo > 0)
            {
                FireGrenade();
                m_CanShoot = false;
            }
        }
        RotateCharacterTowardsMouseCursor();
    }

    void Fire()
    {
        if(m_BulletPrefab)
        {
            // Reduce the Ammo count
            --m_BulletAmmo;

            // Create the Projectile from the Bullet Prefab
            Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, transform.rotation * m_BulletPrefab.transform.rotation);

            // Play Particle Effects
            PlayGunVFX();

            // Play Sound effect
            if (m_AudioSource && m_BulletShot)
            {
                m_AudioSource.PlayOneShot(m_BulletShot);
                pm.AcceptStimulus(new Stimulus(Stimulus.StimulusTypes.AudioWeapon, player.gameObject, player.transform.position, transform.forward, shotSoundDistance, null));               
            }

            // Update UI
            if(m_UIManager)
            {
                m_UIManager.SetAmmoText(m_BulletAmmo, m_GrenadeAmmo);
            }
        }
    }

    void FireGrenade()
    {
        if(m_ExplosiveBulletPrefab)
        {
            // Reduce the Ammo count
            --m_GrenadeAmmo;

            // Create the Projectile from the Explosive Bullet Prefab
            Instantiate(m_ExplosiveBulletPrefab, m_BulletSpawnPoint.position, transform.rotation * m_ExplosiveBulletPrefab.transform.rotation);

            // Play Particle Effects
            PlayGunVFX();

            // Play Sound effect
            if (m_AudioSource && m_GrenadeLaunched)
            {
                m_AudioSource.PlayOneShot(m_GrenadeLaunched);
            }

            // Update UI
            if(m_UIManager)
            {
                m_UIManager.SetAmmoText(m_BulletAmmo, m_GrenadeAmmo);
            }
        }
    }

    void PlayGunVFX()
    {
        if (m_Flare)
        {
            m_Flare.Play();
        }

        if (m_Sparks)
        {
            m_Sparks.Play();
        }

        if (m_Smoke)
        {
            m_Smoke.Play();
        }
    }

    public void AddAmmo(int bullets, int grenades)
    {
        m_BulletAmmo += bullets;
        m_GrenadeAmmo += grenades;

        // Update UI
        if (m_UIManager)
        {
            m_UIManager.SetAmmoText(m_BulletAmmo, m_GrenadeAmmo);
        }
    }
    void RotateCharacterTowardsMouseCursor()
    {
        Vector3 mousePosInScreenSpace = Input.mousePosition;
        Vector3 playerPosInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 directionInScreenSpace = mousePosInScreenSpace - playerPosInScreenSpace;

        float angle = Mathf.Atan2(directionInScreenSpace.y, directionInScreenSpace.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 90.0f, Vector3.up);
        transform.Rotate(Vector3.forward * 180);
    }
}

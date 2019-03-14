using UnityEngine;

public class TurretController : MonoBehaviour {

    public Transform target;
    public Transform barrels;
    public Transform shootPoint;

    public GameObject m_bulletPrefab;

    Vector3 direction;
    public float speed;

    Senses senses;

    public float shootDelayReset;
    float shootDelay;

    public AudioSource audioSource;
    public AudioClip shootSound;

	// Use this for initialization
	void Start () {
        senses = this.GetComponent<Senses>();
        audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit hitInfo;
        LayerMask playerMask = 1 << 8;
        // Player may be obscurred by cover so ensure ray picks up cover too
        LayerMask coverMask = 1 << 9;
        // AI 
        LayerMask aiMask = 1 << 10;
        LayerMask objMask = 1 << 11;
        LayerMask mask =  coverMask | playerMask | aiMask | objMask;

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, fwd * 50, Color.red);
        Physics.Raycast(transform.position, fwd, out hitInfo, 100,mask.value);

        if (hitInfo.collider.CompareTag(Tags.Player) || hitInfo.collider.CompareTag("Throwable"))
        {
            target = hitInfo.collider.gameObject.transform;
            FaceTarget();
        }
        else
        {
            if(transform.eulerAngles.x != 0)
            {
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            }
            target = null;
            Vector3 rot = new Vector3(0.0f, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * Time.deltaTime*speed, Space.World);            
        }
    }
    public void FaceTarget()
    {
        if (target.CompareTag("Player"))
        {
            Vector3 targetDir = target.transform.position - transform.position;

            targetDir.y = 0;

            Quaternion rotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = rotation;
            shootDelay -= Time.deltaTime;
            if (shootDelay <= 0)
            {
                Shoot();
                shootDelay = shootDelayReset;
            }
        }
        if (target.CompareTag("Throwable"))
        {
            Vector3 targetDir = target.transform.position - transform.position;           

            Quaternion rotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = rotation;
            shootDelay -= Time.deltaTime;
            if (shootDelay <= 0)
            {
                Shoot();
                shootDelay = shootDelayReset;
            }
        }
    }
    public void Shoot()
    {
        Instantiate(m_bulletPrefab, shootPoint.position, transform.rotation * m_bulletPrefab.transform.rotation);
        audioSource.PlayOneShot(shootSound);
    }
}

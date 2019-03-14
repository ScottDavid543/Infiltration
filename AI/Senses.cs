using UnityEngine;
using System.Collections;

public class Senses : MonoBehaviour {
    public GameObject target;
    private CharacterController characterController;
    public float viewingAngle = 200.0f;
    public float sightRange = 200.0f;
    public float engageAngle = 20f;
    [HideInInspector]
    public float forwardWeaponRotationCorrectionFactor;

	

    void Start(){
        characterController = GetComponent<CharacterController>();
        // Initialise here as initialising when defined saved value in properties even though hide had been enabled and name changed!!
        forwardWeaponRotationCorrectionFactor = 20f;
    }


    // Is the target visible
    public bool CanSeeTarget(){
        // Check target is alive
        if (target != null){
            // Get distance to target
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
            // Check in visible range
            if (sightRange > distanceToTarget) {
                // Direction of target from agent
                Vector3 targetDirection = target.transform.position - transform.position;
                // Angle between agent and Target
                float angle = Vector3.Angle(targetDirection, transform.forward);
               // angle += forwardWeaponRotationCorrectionFactor;
                // Convert to positive value
                angle = System.Math.Abs(angle);
                // Is the target within the viewing angle. Ignores obstacles 
                if (angle < (viewingAngle / 2)){
                    CharacterController targetCharacterController = target.GetComponent<CharacterController>();
                    RaycastHit hitData;
                    // Create a layer mask for the ray. Look for players and agents
                    LayerMask playerMask = 1 << 8;
                    LayerMask aiMask = 1 << 10;
                    // target may be obscured by cover so ensure ray picks up cover too
                    LayerMask coverMask = 1 << 9;
                    // Combine the mask
                    LayerMask mask = coverMask | playerMask | aiMask;
                    // Height of target
                    float targetHeight = targetCharacterController.height;
                    // Heigth of agent
                    float height = characterController.height;
                    // Agent's eye position
                    Vector3 eyePosition = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
                    // A position in the middle of the target
                    Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y - (targetHeight / 2.0f), target.transform.position.z);
                    // Vector from agent to middle of target
                    Vector3 direction = (targetPos - transform.position).normalized;
                    // Cast a ray to ensure target is not hidden by obstacles
                    bool hit = Physics.Raycast(eyePosition, direction, out hitData, sightRange, mask.value);
                    Debug.DrawRay(eyePosition, direction * sightRange, Color.red);
                    // Ray hit target/cover 
                    if (hit){
                        // Does object ray hit tag match that of target
                        if (hitData.collider.tag == target.gameObject.tag) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }


    public bool IsBehindLowCover(GameObject agent, GameObject target) {
        float[] checkHeights = { 0.2f, 0.5f, 2.0f };
        bool[] covered = { false, false, false };
        float range = 3f;
        RaycastHit hitData;
        LayerMask coverMask = 1 << 9;
        Vector3 checkPosition;
        Vector3 direction = (target.transform.position - agent.transform.position).normalized;
        for(int n = 0; n < checkHeights.Length; n++){
            checkPosition = new Vector3(agent.transform.position.x, agent.transform.position.y + checkHeights[n], agent.transform.position.z);
            covered[n] = Physics.Raycast(checkPosition, direction, out hitData, range, coverMask.value);
            //Debug.DrawRay(checkPosition, direction * range, Color.yellow);
            //Debug.Log("Ray hit : " + covered[n] );
        }
        // Partial cover considered OK
        return ((covered[0] || covered[1]) && !covered[2]);
    }

 

}

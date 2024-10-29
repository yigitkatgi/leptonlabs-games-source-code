using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTrailController : MonoBehaviour
{
    public Transform leftWheel;
    public Transform rightWheel;
    public ParticleSystem fireFX;

    public void StartFireTrail()
    {
        ParticleSystem leftFire = Instantiate(fireFX,leftWheel.transform.position, fireFX.transform.rotation);
        ParticleSystem rightFire = Instantiate(fireFX,rightWheel.transform.position, fireFX.transform.rotation);

        leftFire.transform.SetParent(leftWheel.transform, true);
        rightFire.transform.SetParent(rightWheel.transform, true);

    }

    public void EndFireTrail()
    {
        Destroy(leftWheel.GetChild(0).gameObject);
        Destroy(rightWheel.GetChild(0).gameObject);
    }
    
}

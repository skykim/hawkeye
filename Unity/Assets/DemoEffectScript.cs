using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoEffectScript : MonoBehaviour
{
    //--> street
    //CAR_BEEP -> CFXR Hit D 3D (Yellow) + CFXR _BEEPBEEP
    //SCREAM ->  CFXR2 Skull Head Alt
    //SIREN -> CFXR Water Ripples + "Siren"
    //--> indoor
    //doorbell : CFX2_PickupSmiley2
    //glass breaking : CFX2_RockHit
    //fire alarm : CFX_Explosion_B_Smoke+Text + "Fire!"
    //appliance sound : CFX2_PickupDiamond2 + "Microwave"
    
    public GameObject effect_carbeep;
    public GameObject effect_scream;
    public GameObject effect_siren;
    
    public GameObject effect_doorbell;
    public GameObject effect_glassbreaking;
    public GameObject effect_fire;
    public GameObject effect_microwave;
    
    float next_spawn_time;
    
    void Start()
    {
        next_spawn_time = Time.time+2.0f;
    }

    Vector3 GetRandomPosition()
    {
        float x = 0.0f;
        float y = Random.Range(-12.0f, 12.0f);
        float z = Random.Range(-25.0f, 25.0f);
        
        return new Vector3(x, y, z);
    }

    void Update()
    {
        if(Time.time > next_spawn_time)
        {
            Instantiate(effect_carbeep, GetRandomPosition(), Quaternion.identity);
            Instantiate(effect_scream, GetRandomPosition(), Quaternion.identity);
            Instantiate(effect_siren, GetRandomPosition(), Quaternion.identity);
            
            Instantiate(effect_doorbell, GetRandomPosition(), Quaternion.identity);
            Instantiate(effect_glassbreaking, GetRandomPosition(), Quaternion.identity);
            Instantiate(effect_fire, GetRandomPosition(), Quaternion.identity);
            Instantiate(effect_microwave, GetRandomPosition(), Quaternion.identity);
            
 
            next_spawn_time += 2.0f;
        }
    }
}

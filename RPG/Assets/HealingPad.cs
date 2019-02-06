using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPad : MonoBehaviour
{
    [Range(-10f, -1f)][SerializeField] float healingValue = -1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        GameObject target = other.gameObject;
        if(target.tag == "Player")
        {
            var player = target.GetComponentInParent<Player>();
            player.TakeDamage(healingValue); 
        }
    }
}

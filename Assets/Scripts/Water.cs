using UnityEngine;

public class Water : MonoBehaviour
{
    public PlayerController player;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("objeto"))
        {
            other.GetComponent<Rigidbody>().useGravity = false;
            other.GetComponent<GravidadeObjeto>().agua = true;
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("objeto"))
        {
            other.GetComponent<Rigidbody>().useGravity = true;


        }


    }
}

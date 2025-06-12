using UnityEngine;

public class GravidadeObjeto : MonoBehaviour
{
    [Header("Gravidade")]
    public Transform water;
    public float forcaEmpuxo;
    public Vector3 cubeSize = new Vector3();

    [Header("Movimentação parado")]
    public bool submerso;

    //movimento Y
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 0.3f;

    //Rotação
    public float rotatonAmplitude = 5f;
    public float rotationFrequency = 0.3f;

    private float randomOffSet;
    private Vector3 startPos;

    private Rigidbody rb;

    public float densidadeObjeto;
    public bool agua;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        randomOffSet = Random.Range(0f, 100f);

    }

    private void FixedUpdate()
    {
        if (agua)
        {
            rb.linearDamping = 1;
        }
        //else
        //{
        //    rb.linearDamping = 0.1f;
        //}


        forcaEmpuxo = rb.mass * 0.5f;

        float densidadeObjeto = rb.mass / (cubeSize.x * cubeSize.y * cubeSize.z);

        //float densidadeAgua = 997f;
        float densidadeAgua = 99f;

        print("Densidade do objeto: " + densidadeObjeto);

        //verifica a massa do objeto
        if (densidadeObjeto < densidadeAgua)
        {
            Vector3 empuxo = new Vector3(0, forcaEmpuxo, 0);

            rb.AddForce(empuxo, ForceMode.Force);

        }
        else if (densidadeObjeto > densidadeAgua)
        {
            Vector3 empuxo = new Vector3(0, forcaEmpuxo, 0);

            rb.AddForce(-empuxo, ForceMode.Force);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;

        }
    }

    private void Update()
    {
        if (rb.linearVelocity.magnitude < 0.001f && !submerso)
        {
            startPos = transform.position;
            //rb.isKinematic = true;
            submerso = true;
        }

        if (submerso)
        {
            float time = Time.time + randomOffSet;

            float newY = startPos.y + Mathf.Sin(time * floatFrequency * 2f * Mathf.PI) * floatAmplitude;

            float newRotation = Mathf.Sin(time * rotationFrequency * 2f * Mathf.PI) * rotatonAmplitude;

            //rb.MovePosition(new Vector3(startPos.x, newY, startPos.z));
            transform.rotation = Quaternion.Euler(0, 0, newRotation);

        }
    }
}

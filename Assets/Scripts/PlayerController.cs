using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform orientation;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Health")]
    public int currentHealth;
    private int maxHealth = 10;
    public bool isDead = false;

    [Header("Attack")]
    public GameObject projectile;
    public Transform camera;
    public float attackRate = 0.5f;
    private float nextAttack;

    [Header("Pontos")]
    public int pontos = 0;
    [SerializeField] TextMeshProUGUI pontosText;


    public bool isSwimming;

    [SerializeField] private GameObject menuGameover;
    [SerializeField] private GameObject cameraObj;

    private float HorizontalImput;
    private float VerticalImput;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        MovePlayer();


    }

    void Update()
    {
        //Transformar ponto em string


        //input do movimento
        HorizontalImput = Input.GetAxisRaw("Horizontal");
        VerticalImput = Input.GetAxisRaw("Vertical");

        //ground Check
        //Physics.RAycast(<origem do raio>, <Direção do raio>, <comprimento do raio> <Objeto que será verificado>);
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

        //controle de velocidade
        SpeedControl();


        //attack

        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextAttack)
        {
            Attack();
            nextAttack = attackRate + Time.time;
            //Invoke("Attack", attackRate );
        }

    }

    void Attack()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);

        Rigidbody rb = Instantiate(projectile, spawnPos, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(camera.forward * 80f, ForceMode.Impulse);
        rb.AddForce(transform.up, ForceMode.Impulse);
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * VerticalImput + orientation.right * HorizontalImput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }




    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
            isDead = false;


            //Reseta a cena
            SceneManager.LoadScene(1);

        }
    }

    private void SpeedControl()
    {
        //pega a velocidade atual
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //limita a velocidade caso passe da moveSpeed do player
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;

            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);


        }

    }

    public void ContadorPontos()
    {
        pontos += 5;
        pontosText.text = pontos.ToString();
    }
}

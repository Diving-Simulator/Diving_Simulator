using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    [Header("Referência ao corpo visual")]
    public Transform corpoVisual;

    [Header("Velocidade")]
    [Range(0f, 200f)] public float velocidadeMaxima = 20f;
    public float aceleracao = 3f;
    public float desaceleracao = 4f;
    public float recuo = 10f;

    [Header("Rotação")]
    public float velocidadeRotacao = 50f;
    public float pitchMaximo = 50f;

    [Header("Configuração de roll")]
    public float velocidadeRoll = 40f;
    public float fatorCorrecaoRoll = 0.35f;

    [Header("Teclas")]
    public KeyCode acelerarKey = KeyCode.LeftShift;
    public KeyCode frearKey = KeyCode.Space;
    public KeyCode rollEsquerdaKey = KeyCode.Q;
    public KeyCode rollDireitaKey = KeyCode.E;

    private float velocidadeAtual = 0f;
    private bool modoLivre = false;
    private bool realinhando = false;

    //Luz
    public GameObject luzExterna;
    private bool luzAtiva = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (modoLivre)
            {
                realinhando = true;
            }

            modoLivre = !modoLivre;
            Debug.Log("Modo Livre: " + (modoLivre ? "Ativado" : "Desativado"));
        }

        if (Input.GetKeyDown(KeyCode.C) && !luzAtiva)
        {
            luzExterna.SetActive(true);
            luzAtiva = true;
        }else if (Input.GetKeyDown(KeyCode.C) && luzAtiva)
        {
            luzExterna.SetActive(false);
            luzAtiva = false;
        }


        if (realinhando)
        {
            Vector3 euler = corpoVisual.localEulerAngles;
            euler.x = NormalizeAngle(euler.x);
            euler.z = NormalizeAngle(euler.z);

            float velocidadeReset = velocidadeRotacao * 3f;

            euler.x = Mathf.MoveTowards(euler.x, 0f, velocidadeReset * Time.deltaTime);
            euler.z = Mathf.MoveTowards(euler.z, 0f, velocidadeReset * Time.deltaTime);

            corpoVisual.localEulerAngles = new Vector3(euler.x, corpoVisual.localEulerAngles.y, euler.z);

            if (Mathf.Abs(euler.x) < 0.1f && Mathf.Abs(euler.z) < 0.1f)
                realinhando = false;

            return;
        }

        float rotHorizontal = Input.GetAxis("Horizontal");
        float rotVertical = -Input.GetAxis("Vertical");

        corpoVisual.Rotate(Vector3.up, rotHorizontal * velocidadeRotacao * Time.deltaTime, Space.Self);

        if (Mathf.Abs(rotVertical) > 0.01f)
        {
            Vector3 euler = corpoVisual.localEulerAngles;
            euler.x = NormalizeAngle(euler.x);

            float novoPitch = euler.x + rotVertical * velocidadeRotacao * Time.deltaTime;
            novoPitch = Mathf.Clamp(novoPitch, -pitchMaximo, pitchMaximo);

            euler.x = novoPitch;
            corpoVisual.localEulerAngles = new Vector3(euler.x, corpoVisual.localEulerAngles.y, corpoVisual.localEulerAngles.z);
        }
        else if (Mathf.Abs(velocidadeAtual) > 0.01f)
        {
            Vector3 euler = corpoVisual.localEulerAngles;
            euler.x = NormalizeAngle(euler.x);

            if (euler.x < 0f)
            {
                float fatorEstabilizacao = Mathf.InverseLerp(0f, velocidadeMaxima, Mathf.Abs(velocidadeAtual));
                float intensidade = velocidadeRotacao * fatorEstabilizacao * 0.3f;

                euler.x = Mathf.MoveTowards(euler.x, 0f, intensidade * Time.deltaTime);
                corpoVisual.localEulerAngles = new Vector3(euler.x, corpoVisual.localEulerAngles.y, corpoVisual.localEulerAngles.z);
            }
        }

        if (Input.GetKey(acelerarKey))
            velocidadeAtual += aceleracao * Time.deltaTime;
        else if (Input.GetKey(frearKey))
            velocidadeAtual = Mathf.MoveTowards(velocidadeAtual, -recuo, desaceleracao * Time.deltaTime);
        else
            velocidadeAtual = Mathf.MoveTowards(velocidadeAtual, 0f, desaceleracao * Time.deltaTime);

        float rollInput = 0f;
        if (Input.GetKey(rollEsquerdaKey)) rollInput = 1f;
        if (Input.GetKey(rollDireitaKey)) rollInput = -1f;

        Vector3 eulerRoll = corpoVisual.localEulerAngles;
        eulerRoll.z = NormalizeAngle(eulerRoll.z);

        if (!modoLivre)
        {
            if (rollInput != 0f)
            {
                float novoRoll = eulerRoll.z + rollInput * velocidadeRoll * Time.deltaTime;
                novoRoll = Mathf.Clamp(novoRoll, -45f, 45f);
                corpoVisual.localEulerAngles = new Vector3(corpoVisual.localEulerAngles.x, corpoVisual.localEulerAngles.y, novoRoll);
            }
            else
            {
                float fatorEstabilizacaoZ = Mathf.InverseLerp(0f, velocidadeMaxima, Mathf.Abs(velocidadeAtual));
                float intensidadeZ = velocidadeRoll * Mathf.Max(fatorEstabilizacaoZ, 0.3f) * fatorCorrecaoRoll;

                eulerRoll.z = Mathf.MoveTowards(eulerRoll.z, 0f, intensidadeZ * Time.deltaTime);
                corpoVisual.localEulerAngles = new Vector3(corpoVisual.localEulerAngles.x, corpoVisual.localEulerAngles.y, eulerRoll.z);
            }
        }
        else
        {
            corpoVisual.Rotate(Vector3.forward, rollInput * velocidadeRoll * Time.deltaTime, Space.Self);
        }

        velocidadeAtual = Mathf.Clamp(velocidadeAtual, -recuo, velocidadeMaxima);

        transform.position += corpoVisual.forward * velocidadeAtual * Time.deltaTime;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
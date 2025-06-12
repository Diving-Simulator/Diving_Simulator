using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class DephController : MonoBehaviour
{

    public TMP_Text atm_Text;
    public TMP_Text depth_Text;
    public Light luzInterna;

    public Transform superficieAgua;
    private bool metersUnit = false;

    private const float GRAVIDADE = 9.81f;              // acelera��o da gravidade em m/s�
    private const float DENSIDADE_AGUA = 1000f;         // densidade da �gua em kg/m�
    private const float PRESSAO_ATM_PA = 101325f;       // press�o atmosf�rica em Pa (pascal)
    private const float METROS_PARA_PES = 3.28084f;     // convers�o de metros para p�s

    //mudar efeito submerso
    public Volume volume;

    void Update()
    {
        float profundidadeM = Mathf.Max(0, superficieAgua.position.y - transform.position.y);
        float profundidade = metersUnit ? profundidadeM : profundidadeM * METROS_PARA_PES;

        float pressaoPa = PRESSAO_ATM_PA + (DENSIDADE_AGUA * GRAVIDADE * profundidadeM);
        float pressaoAtm = pressaoPa / PRESSAO_ATM_PA;

        string unidade = metersUnit ? "m" : "ft";

        atm_Text.text = $"{pressaoAtm:F2} atm";
        depth_Text.text = $"{profundidade:F2} {unidade}";

        //fazer o efeito aumentar conforme vai afundando 

        // Transi��o gradual do volume
        float pressaoMin = 1f;
        float pressaoMax = 100f;
        float t = Mathf.InverseLerp(pressaoMin, pressaoMax, pressaoAtm);
        t = Mathf.SmoothStep(0, 1, t);
        volume.weight = t;

        //Transi��o gradual da luz interna
        float luzMin = 1f;
        float luzMax = 100f;
        float luz = Mathf.InverseLerp(luzMin, luzMax, pressaoAtm);
        luz = Mathf.SmoothStep(0, 1, luz);
        luzInterna.intensity = Mathf.Lerp(0, 25, luz);


        if (pressaoAtm > 200)
        {
            //SceneManager.LoadScene(0);
        }
        else if (pressaoAtm >= 150)
        {
            atm_Text.color = Color.red;
            new CharacterController();
        }
        else if (pressaoAtm >= 100)
        {
            atm_Text.color = Color.yellow;

        }
        else
        {
            atm_Text.color = Color.white;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public static string nickname;

    [SerializeField] InputField input;
    [SerializeField] string cena;

    private void Start() => input.Select();

    public void Botao()
    {
        if (input.text.Length < 3) return;

        nickname = input.text;
        SceneManager.LoadScene(cena);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) Botao();
    }
}

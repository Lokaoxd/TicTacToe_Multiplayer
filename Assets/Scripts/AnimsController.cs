using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimsController : MonoBehaviour
{
    [SerializeField]
    Image[] horizontal = new Image[3],
            vertical = new Image[3],
            diagonal = new Image[2];

    enum Horizontal
    {
        cima,
        meio,
        baixo
    }

    enum Vertical
    {
        esquerda,
        meio,
        direita
    }

    enum Diagonal
    {
        esquerda,
        direita
    }

    public void PlayAnim(string direcao, int posicao, char simbolTarget)
    {
        Image temp = null;

        if (direcao == "horizontal") temp = horizontal[posicao];
        else if (direcao == "vertical") temp = vertical[posicao];
        else if (direcao == "diagonal") temp = diagonal[posicao];

        StartCoroutine(Anim(temp, GetColor(simbolTarget)));
    }

    public void ResetAnims()
    {
        for (int i = 0; i < horizontal.Length; i++)
            horizontal[i].fillAmount = 0;
        for (int i = 0; i < vertical.Length; i++)
            vertical[i].fillAmount = 0;
        for (int i = 0; i < diagonal.Length; i++)
            diagonal[i].fillAmount = 0;
    }

    private IEnumerator Anim(Image img, Color cor)
    {
        if (img != null)
        {
            img.color = cor;
            while (img.fillAmount != 1)
            {
                img.fillAmount += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private Color GetColor(char simbolTarget)
    {
        return simbolTarget switch
        {
            'x' => new(1f, 0.3803922f, 0.372549f, 1f),
            'o' => new(0.2431373f, 0.7725491f, 0.9529412f, 1f),
            _ => Color.magenta
        };
    }
}
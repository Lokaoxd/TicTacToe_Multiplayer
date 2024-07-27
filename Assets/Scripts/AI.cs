using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    string mode = "facil";
    char simbolTarget = 'o';
    GameController controller;
    bool block, blockCoroutine;

    private void Start() => controller = Camera.main.GetComponent<GameController>();

    private void Update()
    {
        switch (mode)
        {
            case "facil": ModeEasy(); break;
        }
    }

    private void ModeEasy()
    {
        char simbolController = controller.GetTurno();
        if (simbolController == simbolTarget)
        {
            int i, j;
            do
            {
                i = Random.Range(0, 3);
                j = Random.Range(0, 3);
            } while (controller.posicoes[i,j].GetComponent<SpriteRenderer>().sprite != null);

            if (!blockCoroutine) StartCoroutine(FazerJogada(i,j));
        }
    }

    private IEnumerator FazerJogada(int i, int j)
    {
        blockCoroutine = true;

        //foreach (var item in controller.posicoes)
        //    item.GetComponent<Pecas>().BloquearJogada();

        yield return new WaitForSeconds(0.5f);
        //controller.posicoes[i, j].GetComponent<Pecas>().DesbloquearJogada();
        //controller.posicoes[i, j].GetComponent<Pecas>().Jogar();

        //foreach (var item in controller.posicoes)
        //    item.GetComponent<Pecas>().DesbloquearJogada();

        blockCoroutine = false;
    }
}

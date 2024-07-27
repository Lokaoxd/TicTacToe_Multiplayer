using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class GameController : MonoBehaviour
{
    readonly public GameObject[,] posicoes = new GameObject[3, 3];
    private char turno;
    public PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        for (int i = 0; i < posicoes.GetLength(0); i++)
            for (int j = 0; j < posicoes.GetLength(1); j++)
                posicoes[i, j] = GameObject.Find($"{i},{j}");
    }

    private void Start() => turno = 'x';

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) print(turno);
    }

    private bool Velha()
    {
        foreach (var item in posicoes)
            if (item.GetComponent<SpriteRenderer>().sprite == null)
                return false;

        return true;
    }

    public char GetTurno() => turno;
    
    public void ChangeTurno()
    {
        if (turno == 'x') turno = 'o';
        else turno = 'x';
    }
}
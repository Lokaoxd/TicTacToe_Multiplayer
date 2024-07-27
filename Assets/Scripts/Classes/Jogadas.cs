using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jogadas
{
    public Jogador jogador {  get; private set; }
    public string coordenadas { get; private set; }

    public Jogadas(string coordenadas)
    {
        this.coordenadas = coordenadas;
    }
}
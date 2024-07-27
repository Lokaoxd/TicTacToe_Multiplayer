using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;

public class Jogador
{
    public Player player { get; private set; }
    public string state { get; set; }
    public char simbol {  get; set; }
    public bool ready { get; set; }

    public Jogador(Player player, string state, char simbol)
    {
        this.player = player;
        this.state = state;
        this.simbol = simbol;
        ready = false;
    }
}
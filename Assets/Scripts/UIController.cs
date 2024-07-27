using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using System.Collections;

public class UIController : MonoBehaviour
{
    GameManager manager;
    PhotonView photonView;
    public Text jogadores, espectadores, nomeSala;
    public GameObject readyButton, reiniciarButton, entrarButton;
    bool block;

    private void Awake()
    {
        manager = GetComponent<GameManager>();
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !block)
            StartCoroutine(TextCoroutine());

        if (PhotonNetwork.CurrentRoom != null) nomeSala.text = $"Sala: {PhotonNetwork.CurrentRoom.Name}";
    }

    private string WritePlayers()
    {
        var temp = $"JOGADORES ({manager.CountJogadores()})\n";

        if (manager.playerJogar == null)
        {
            foreach (var player in manager.jogadores)
                if (player != null)
                    if (player.state == JogadorState.jogador)
                    {
                        temp += $"\n{player.player.NickName}";

                        if (player.player == PhotonNetwork.MasterClient) temp += " (Host)";
                        if (player.ready) temp += " - <Color=Green>Pronto!</Color>";
                        else temp += " - <Color=Orange>Pronto?</Color>";
                    }
        }
        else if (!string.IsNullOrEmpty(manager.winner))
        {
            foreach (var player in manager.jogadores)
                if (player != null)
                    if (player.state == JogadorState.jogador)
                    {
                        temp += $"\n{player.player.NickName}";

                        if (player.player == PhotonNetwork.MasterClient) temp += " (Host)";
                        if (player.player.NickName.Equals(manager.winner)) temp += " - <Color=Green>Vencedor</Color>";
                        else if (manager.winner.Equals("ninguem")) temp += " - <Color=Gray>Empate</Color>";
                        else temp += " - <Color=Red>Perdedor</Color>";
                    }
        }
        else
        {
            foreach (var player in manager.jogadores)
                if (player != null)
                    if (player.state == JogadorState.jogador)
                    {
                        temp += $"\n{player.player.NickName}";

                        if (player.player == PhotonNetwork.MasterClient) temp += " (Host)";
                        if (player == manager.playerJogar) temp += " - <Color=Blue>Sua Vez</Color>";
                        else temp += " - <Color=Gray>Aguardando</Color>";
                    }
        }

        return temp;
    }

    private string WriteSpectators()
    {
        var temp = $"ESPECTADORES ({manager.CountEspectadores()})\n";

        foreach (var player in manager.jogadores)
            if (player != null)
                if (player.state == JogadorState.espectador)
                {
                    if (player.ready) temp += $"\n{player.player.NickName}";
                    else temp += $"\n{player.player.NickName}";
                }

        return temp;
    }

    [PunRPC]
    private void UITransmission(string players, string spectators)
    {
        string nick = PhotonNetwork.LocalPlayer.NickName;
        string tempPlayers = players, tempSpec = spectators;

        foreach (var item in PhotonNetwork.PlayerList)
        {
            if (players.Contains(item.NickName))
            {
                if (item.NickName.Equals(nick))
                {
                    tempPlayers = Tools.ColorirTextoHtml(tempPlayers, nick, Color.yellow);
                    tempPlayers = tempPlayers.Replace(nick, "Você");
                }
                else tempPlayers = Tools.ColorirTextoHtml(tempPlayers, item.NickName, "#8B4513");
            }
            else if (spectators.Contains(item.NickName))
            {
                if (item.NickName.Equals(nick))
                {
                    tempSpec = Tools.ColorirTextoHtml(tempSpec, nick, Color.yellow);
                    tempPlayers = tempPlayers.Replace(nick, "Você");
                }
                else tempSpec = Tools.ColorirTextoHtml(tempSpec, item.NickName, "#8B4513");
            }
        }

        jogadores.text = tempPlayers;
        espectadores.text = tempSpec;
    }

    public void ReadyButton() => manager.photonView.RPC("GetReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);

    public void ReiniciarButton()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        reiniciarButton.SetActive(false);
        manager.winner = string.Empty;
        manager.photonView.RPC("LimparPosicoes", RpcTarget.All);
        manager.playerJogar = null;
        manager.ResetReadyPlayers();
    }

    public void EntrarButton()
    {
        manager.photonView.RPC("SpecToPlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        entrarButton.SetActive(false);
    }

    public void SairButton()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.Disconnect();
        else Application.Quit();
    }

    private IEnumerator TextCoroutine()
    {
        block = true;
        photonView.RPC("UITransmission", RpcTarget.AllViaServer, WritePlayers(), WriteSpectators());
        yield return new WaitForSeconds(1f);
        block = false;
    }
}
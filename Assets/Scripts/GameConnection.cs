using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class GameConnection : MonoBehaviourPunCallbacks
{
    GameManager manager;
    Lobby lobbyManager;
    UIController uicontroller;
    public Text chatLog;
    [SerializeField] GameObject lobby, game;

    private void Awake()
    {
        manager = GetComponent<GameManager>();
        lobbyManager = GetComponent<Lobby>();
        uicontroller = GetComponent<UIController>();
    }

    private void Start()
    {
        Log("Conectando ao servidor...");
        if (string.IsNullOrEmpty(Login.nickname)) PhotonNetwork.LocalPlayer.NickName = $"Lokao_{Random.Range(1, 1000)}";
        else PhotonNetwork.LocalPlayer.NickName = Login.nickname;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Log("Conectado ao servidor!");

        if (!PhotonNetwork.InLobby)
        {
            Log("Entrando no lobby...");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Log("Entrei no Lobby!");

        //string roomName = "LokaoGame";

        //chatLog.text += $"\nEntrando na sala {roomName}...";

        //PhotonNetwork.JoinOrCreateRoom(roomName, new()
        //{
        //    MaxPlayers = 10
        //}, null);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Log($"Criando a sala: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        Log($"O Host disconectou.");
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Log($"Você entrou na sala {PhotonNetwork.CurrentRoom.Name}! Seu username é: {PhotonNetwork.LocalPlayer.NickName}");

        if (PhotonNetwork.IsMasterClient) manager.RegistrarJogador(PhotonNetwork.LocalPlayer);

        lobby.SetActive(false);
        game.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        if (PhotonNetwork.IsMasterClient) manager.ExcluirDadosHost();

        //print($"\nVocê saiu da sala {PhotonNetwork.CurrentRoom.Name}! Seu username é: {PhotonNetwork.LocalPlayer.NickName}");
        //chatLog.text += $"\nVocê saiu da sala {PhotonNetwork.CurrentRoom.Name}! Seu username é: {PhotonNetwork.LocalPlayer.NickName}";

        lobby.SetActive(true);
        game.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Log($"Um jogador entrou na sala {PhotonNetwork.CurrentRoom.Name}! O username dele é: {newPlayer.NickName}");

        if (PhotonNetwork.IsMasterClient) manager.RegistrarJogador(newPlayer);
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            manager.photonView.RPC("ReadyButton", RpcTarget.All, PhotonNetwork.LocalPlayer, newPlayer, true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (PhotonNetwork.IsMasterClient) manager.ExcluirJogador(otherPlayer);
        if (uicontroller.espectadores.text.Contains(PhotonNetwork.LocalPlayer.NickName))
            uicontroller.entrarButton.SetActive(true);
        Log($"Um jogador saiu da sala {PhotonNetwork.CurrentRoom.Name}! O username dele era: {otherPlayer.NickName}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        if (chatLog != null) Log($"Você foi desconectado da sala. Motivo: {cause}");
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        base.OnErrorInfo(errorInfo);

        Log($"Aconteceu um erro! {errorInfo.Info}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) => 
        lobbyManager.AtualizarLista(roomList);

    private void Log(string text) => print(text);
}

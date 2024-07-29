using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Exemplo : MonoBehaviour
{
    PhotonView photonView;

    private void Awake()
    {
        // Adicionar e armazenar o componente
        photonView = gameObject.AddComponent<PhotonView>();
    }

    public void EnviarMensagem(string mensagem, string nomeJogadorAlvo)
    {
        photonView.RPC("EnviarMensagemRPC", RpcTarget.Others,
            // Parâmetros
            PhotonNetwork.LocalPlayer, mensagem, nomeJogadorAlvo);
    }

    [PunRPC]
    public void EnviarMensagemRPC(Player player, string mensagem, string nomeJogadorAlvo)
    {
        if (nomeJogadorAlvo.Equals(PhotonNetwork.NickName))
            print($"O jogador {player.NickName} enviou a seguinte mensagem para você: '{mensagem}''");
    }
}
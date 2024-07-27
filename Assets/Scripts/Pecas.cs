using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Pecas : MonoBehaviour
{
    GameManager manager;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = Camera.main.GetComponent<GameManager>();
    }

    private void OnMouseDown()
    {
        if (spriteRenderer.sprite == null)
            manager.photonView.RPC("VerificarJogada", RpcTarget.MasterClient, gameObject.name, PhotonNetwork.LocalPlayer);
    }
}
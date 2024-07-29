using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] Transform salasParent;
    GameObject[] salas_Template;

    List<RoomInfo> salas;

    private void Awake()
    {
        salas_Template = new GameObject[salasParent.childCount];
        for (int i = 0; i < salas_Template.Length; i++)
            salas_Template[i] = salasParent.GetChild(i).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) CriarSala();
    }

    public void AtualizarLista(List<RoomInfo> roomList)
    {
        salas = roomList;
        ResetarLista();
        MostrarLista();
    }

    private void ResetarLista()
    {
        foreach (var item in salas_Template)
            item.SetActive(false);
    }

    private void MostrarLista()
    {
        if (salas.Count == 0) print("Nenhuma sala encontrada.");

        for (int i = 0; i < salas.Count; i++)
        {
            RoomInfo room = salas[i];
            GameObject salaTemplate = salas_Template[i];

            salaTemplate.name = $"Sala '{room.Name}'";

            var temp = salaTemplate.transform.GetChild(0);

            Text nomeSala = temp.transform.GetChild(0).GetComponent<Text>();
            Text capacidade = temp.transform.GetChild(1).GetComponent<Text>();
            Button entrarButton = temp.transform.GetChild(2).GetComponent<Button>();

            nomeSala.text = room.Name;
            capacidade.text = $"{room.PlayerCount}/{room.MaxPlayers}";
            entrarButton.onClick.AddListener(() => EntrarSala(room.Name));

            salaTemplate.SetActive(true);
        }
    }

    private void EntrarSala(string nomeSala) => PhotonNetwork.JoinRoom(nomeSala);

    public void CriarSala()
    {
        print("Criando a sala...");
        PhotonNetwork.CreateRoom($"salaTeste_{Random.Range(0, 1000)}", new()
        {
            MaxPlayers = 10,
        });
    }
}

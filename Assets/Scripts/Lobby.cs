using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] GameObject prefab, canvas;

    List<RoomInfo> salas;
    readonly List<GameObject> objetos = new();

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
        foreach (var item in objetos)
            Destroy(item);
        objetos.Clear();
    }

    private void MostrarLista()
    {
        if (salas.Count == 0) print("Nenhuma sala encontrada.");

        foreach (RoomInfo room in salas)
        {
            var temp = Instantiate(prefab, canvas.transform);
            temp.name = $"Sala '{room.Name}'";
            temp.transform.position = Vector3.zero;

            var rect = temp.GetComponentInChildren<Image>().GetComponent<RectTransform>();
            rect.offsetMax = new(-260f, -75f);
            rect.offsetMin = new(260f, 200f);

            Text nomeSala = temp.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
            Text capacidade = temp.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
            Button entrarButton = temp.transform.GetChild(0).transform.GetChild(2).GetComponent<Button>();

            nomeSala.text = room.Name;
            capacidade.text = $"{room.PlayerCount}/{room.MaxPlayers}";
            entrarButton.onClick.AddListener(() => EntrarSala(room.Name));

            objetos.Add(temp);
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

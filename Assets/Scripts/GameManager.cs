using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    UIController UIController;
    AnimsController AnimsController;
    public GameObject[,] posicoes = new GameObject[3, 3];
    public Jogador[] jogadores = new Jogador[10];
    public string winner;
    public PhotonView photonView;
    public Jogador playerJogar;
    [SerializeField] Text ping;
    bool block;

    #region Unity
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        UIController = GetComponent<UIController>();
        AnimsController = GetComponent<AnimsController>();
    }
    private void Update()
    {
        if (!block) StartCoroutine(PingCoroutine());
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.Return))
        {
            foreach (var item in jogadores)
                if (item != null)
                    print(item.player.ActorNumber);
        }

        if (PhotonNetwork.IsMasterClient && posicoes[0,0] == null)
        {
            for (int i = 0; i < posicoes.GetLength(0); i++)
                for (int j = 0; j < posicoes.GetLength(1); j++)
                    posicoes[i, j] = GameObject.Find($"{i},{j}");
        }
    }
    #endregion

    #region VOIDS
    public void RegistrarJogador(Player player)
    {
        for (int i = 0; i < jogadores.Length; i++)
        {
            if (jogadores[i] == null)
            {
                string state;
                char simbol = GetSimbol(i);
                if (i == 0 || i == 1) state = JogadorState.jogador;
                else state = JogadorState.espectador;

                Jogador temp = new(player, state, simbol);
                jogadores[i] = temp;
                break;
            }
        }
    }
    public void ExcluirJogador(Player player)
    {
        try
        {
            int index = GetJogador(player);

            if (jogadores[index].state == JogadorState.jogador)
            {
                ResetReadyPlayers();
                playerJogar = null;
                photonView.RPC("LimparPosicoes", RpcTarget.All);
            }
            jogadores[index] = null;
        }
        catch
        {
            print("Houve um erro ao tentar excluir um jogador.");
        }
    }
    public void ExcluirDadosHost()
    {
        posicoes = new GameObject[3, 3];
        jogadores = new Jogador[10];
        winner = string.Empty;
        playerJogar = null;
        block = false;
    }
    public void ResetReadyPlayers()
    {
        for (int i = 0; i < jogadores.Length; i++)
            if (jogadores[i] != null)
                if (jogadores[i].state == JogadorState.jogador)
                    jogadores[i].ready = false;
    }
    private string JuntarInfos(string info1, string info2, string info3, char separator)
    => string.Join(separator, info1, info2, info3);
    private void SepararInfos(char separator, string infos, out char simbolTarget, out string coord1, out string coord2)
    {
        string[] temp = infos.Split(separator);

        simbolTarget = temp[0][0];
        coord1 = temp[1];
        coord2 = temp[2];
    }
    #endregion

    #region Gets
    private char GetSimbol(int index)
    {
        return index switch
        {
            0 => 'x',
            1 => 'o',
            _ => 'ç'
        };
    }
    public int CountEspectadores()
    {
        int temp = 0;

        foreach (var item in jogadores)
            if (item != null)
                if (item.state.Equals(JogadorState.espectador)) temp++;

        return temp;
    }
    public int CountJogadores()
    {
        int temp = 0;

        foreach (var item in jogadores)
            if (item != null)
                if (item.state.Equals(JogadorState.jogador)) temp++;

        return temp;
    }
    public int CountJogadoresProntos()
    {
        int temp = 0;

        foreach (var item in jogadores)
            if (item != null)
                if (item.ready) temp++;

        return temp;
    }
    private Jogador GetAnotherJogador()
    {
        foreach (var item in jogadores)
            if (item != null)
                if (item != playerJogar && item.state == JogadorState.jogador)
                    return item;
        return null;
    }
    private Sprite GetSprite(Sprite[] sprite, char target)
    {
        foreach (var item in sprite)
            if (item.name.Equals(target.ToString()))
                return item;
        return null;
    }
    private Jogador GetJogador(char simbol)
    {
        foreach (var item in jogadores)
            if (item != null)
                if (item.simbol.Equals(simbol)) return item;
        return null;
    }
    private int GetJogador(Player player)
    {
        for (int i = 0; i < jogadores.Length; i++)
            if (jogadores[i] != null)
                if (jogadores[i].player == player) return i;

        return -1;
    }
    private Jogador[] GetJogadores()
    {
        Jogador[] temp = new Jogador[2];
        int index = 0;

        foreach (var item in jogadores)
            if (item != null)
                if (item.state == JogadorState.jogador)
                    temp[index++] = item;

        return temp;
    }
    private string FindAnim(string coord1, string coord2)
    {
        string coord1_X = coord1.Split(',')[0],
               coord1_Y = coord1.Split(',')[1];

        string coord2_X = coord2.Split(',')[0],
               coord2_Y = coord2.Split(',')[1];

        if (coord1_X == coord2_X) return $"horizontal,{coord1_X}";
        else if (coord1_Y == coord2_Y) return $"vertical,{coord1_Y}";
        else if (coord1_X == coord2_Y) return "diagonal,0";
        else if (coord1_X != coord2_X && coord1_Y != coord2_Y) return "diagonal,1";

        return null;
    }
    #endregion

    #region PunRPC
        #region MasterClient
        [PunRPC]
        private void VerificarJogada(string coordenadas, Player player)
        {
            // Verifica se a jogada é valida
            if (playerJogar != null)
            {
                if (player == playerJogar.player)
                {
                    GameObject objeto = GameObject.Find(coordenadas);
                    if (objeto.GetComponent<SpriteRenderer>().sprite == null)
                    {
                        photonView.RPC("TransmitirJogadas", RpcTarget.AllViaServer, coordenadas, jogadores[player.ActorNumber - 1].simbol.ToString());
                        playerJogar = GetAnotherJogador();
                    }
                }
            }
        }
        [PunRPC]
        private void GetReady(Player player)
        {
            jogadores[GetJogador(player)].ready = !jogadores[GetJogador(player)].ready;

            if (CountJogadoresProntos() == 2)
            {
                playerJogar ??= GetAnotherJogador();
                Jogador[] jogadores = GetJogadores();
                photonView.RPC("ReadyButton", RpcTarget.All, jogadores[0].player, jogadores[1].player, false);
            }
        }
        [PunRPC]
        private void SpecToPlayer(Player player)
        {
            int index = GetJogador(player);
            Jogador jogador = jogadores[index];
            if (jogador.state == JogadorState.espectador)
            {
                jogadores[index].state = JogadorState.jogador;
                jogadores[index].simbol = 'o';
            }
        }
        #endregion

        #region All
        [PunRPC]
        private void TransmitirJogadas(string coordenadas, string simbol)
        {
            GameObject objeto = GameObject.Find(coordenadas);
            Sprite spr = GetSprite(Resources.LoadAll<Sprite>("formas"), simbol[0]);

            objeto.GetComponent<SpriteRenderer>().sprite = spr;

            if (PhotonNetwork.IsMasterClient)
            {
                string horizontal = VerificarHorizontal(),
                       vertical = VerificarVertical(),
                       diagonal = VerificarDiagonal();

                string winner = null, direcao = null;
                int posicao = 0;

                string horizontalG = VerificacaoGeral(horizontal),
                       verticalG = VerificacaoGeral(vertical),
                       diagonalG = VerificacaoGeral(diagonal);

                if (horizontalG != null)
                {
                    string[] temp = horizontalG.Split(',');
                    direcao = temp[0];
                    posicao = int.Parse(temp[1]);
                    winner = temp[2];
                }
                else if (verticalG != null)
                {
                    string[] temp = verticalG.Split(',');
                    direcao = temp[0];
                    posicao = int.Parse(temp[1]);
                    winner = temp[2];
                }
                else if (diagonalG != null)
                {
                    string[] temp = diagonalG.Split(',');
                    direcao = temp[0];
                    posicao = int.Parse(temp[1]);
                    winner = temp[2];
                }
                else if (Velha()) winner = "ninguem";

                this.winner = winner;
                
                
                if (!string.IsNullOrEmpty(winner))
                {
                    if (winner != "ninguem") photonView.RPC("PlayAnims", RpcTarget.AllViaServer, direcao, posicao, simbol);
                    UIController.reiniciarButton.SetActive(true);
                }
            }
        }
        [PunRPC]
        private void ReadyButton(Player player1, Player player2, bool ativar)
        {
            if (PhotonNetwork.LocalPlayer == player1 || PhotonNetwork.LocalPlayer == player2)
                UIController.readyButton.SetActive(ativar);
        }
        [PunRPC]
        private void LimparPosicoes()
        {
            for (int i = 0; i < posicoes.GetLength(0); i++)
                for (int j = 0; j < posicoes.GetLength(1); j++)
                    GameObject.Find($"{i},{j}").GetComponent<SpriteRenderer>().sprite = null;

            UIController.readyButton.SetActive(true);
            AnimsController.ResetAnims();
        }
        [PunRPC]
        private void PlayAnims(string direcao, int posicao, string simbol)
        => AnimsController.PlayAnim(direcao, posicao, simbol[0]);
        #endregion
    #endregion

    #region Verificacoes
    private string VerificacaoGeral(string infos)
    {
        if (infos == null) return null;

        SepararInfos('/', infos, out char simbolTarget, out string coord1, out string coord2);

        string temp = GetJogador(simbolTarget).player.NickName;

        return $"{FindAnim(coord1, coord2)},{temp}";
    }
    private string VerificarHorizontal()
    {
        for (int i = 0; i < posicoes.GetLength(0); i++)
        {
            string simbolTarget = null;
            int verifiy = 0;
            string coord1 = null, coord2 = null;

            for (int j = 0; j < posicoes.GetLength(1); j++)
            {
                Sprite spr = posicoes[i, j].GetComponent<SpriteRenderer>().sprite;
                if (spr == null) break;
                else if (simbolTarget == null)
                {
                    simbolTarget = spr.name;
                    coord1 = posicoes[i, j].name;
                }
                else if (!simbolTarget.Equals(spr.name)) break;
                else
                {
                    verifiy++;
                    coord2 = posicoes[i, j].name;
                }
            }

            if (verifiy == 2)
                return JuntarInfos(simbolTarget, coord1, coord2, '/');
        }

        return null;
    }
    private string VerificarVertical()
    {
        for (int j = 0; j < posicoes.GetLength(1); j++)
        {
            string simbolTarget = null;
            int verifiy = 0;
            string coord1 = null, coord2 = null;

            for (int i = 0; i < posicoes.GetLength(0); i++)
            {
                Sprite spr = posicoes[i, j].GetComponent<SpriteRenderer>().sprite;
                if (spr == null) break;
                else if (simbolTarget == null)
                {
                    simbolTarget = spr.name;
                    coord1 = posicoes[i, j].name;
                }
                else if (!simbolTarget.Equals(spr.name)) break;
                else
                {
                    verifiy++;
                    coord2 = posicoes[i, j].name;
                }
            }

            if (verifiy == 2)
                return JuntarInfos(simbolTarget, coord1, coord2, '/');
        }

        return null;
    }
    private string VerificarDiagonal()
    {
        // Diagonal Direita
        {
            int i = 0, j = 0;
            string simbolTarget = null;
            int verifiy = 0;
            string coord1 = null, coord2 = null;

            for (int x = 0; x < 3; x++)
            {
                Sprite spr = posicoes[i, j].GetComponent<SpriteRenderer>().sprite;
                if (spr == null) break;
                else if (simbolTarget == null)
                {
                    simbolTarget = spr.name;
                    coord1 = posicoes[i, j].name;
                }
                else if (!simbolTarget.Equals(spr.name)) break;
                else
                {
                    verifiy++;
                    coord2 = posicoes[i, j].name;
                }

                if (verifiy == 2)
                    return JuntarInfos(simbolTarget, coord1, coord2, '/');
                i++;
                j++;
            }
        }

        // Diagonal Esquerda
        {
            int i = 0, j = 2;
            string simbolTarget = null;
            int verifiy = 0;
            string coord1 = null, coord2 = null;

            for (int x = 0; x < 3; x++)
            {
                Sprite spr = posicoes[i, j].GetComponent<SpriteRenderer>().sprite;
                if (spr == null) break;
                else if (simbolTarget == null)
                {
                    simbolTarget = spr.name;
                    coord1 = posicoes[i, j].name;
                }
                else if (!simbolTarget.Equals(spr.name)) break;
                else
                {
                    verifiy++;
                    coord2 = posicoes[i, j].name;
                }

                if (verifiy == 2)
                    return JuntarInfos(simbolTarget, coord1, coord2, '/');
                i++;
                j--;
            }
        }

        return null;
    }
    private bool Velha()
    {
        foreach (var item in posicoes)
            if (item.GetComponent<SpriteRenderer>().sprite == null)
                return false;

        return true;
    }
    #endregion

    #region Coroutine
    private IEnumerator PingCoroutine()
    {
        block = true;
        ping.text = $"Ping: {PhotonNetwork.GetPing()}ms";
        yield return new WaitForSeconds(1f);
        block = false;
    }
    #endregion
}
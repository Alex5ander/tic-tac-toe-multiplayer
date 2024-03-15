using System;
using SocketIOClient.Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Data
{
    public int index;
    public string simbol;
}

public class Socket : MonoBehaviour
{
    [SerializeField] Cell[] cells;
    [SerializeField] TextMeshProUGUI SimbolText;
    [SerializeField] TextMeshProUGUI StatusText;
    [SerializeField] UnityEvent onPlay;
    [SerializeField] UnityEvent onStartGame;
    [SerializeField] UnityEvent onEndGame;
    [SerializeField] UnityEvent onWin;
    SocketIOUnity socketIO;
    readonly Uri uri = new("http://localhost:3000");
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnUpdate(Data data)
    {
        cells[data.index].SetText(data.simbol);
    }

    void OnStartGame()
    {
        onStartGame.Invoke();
    }

    void OnEndGame(bool win = false)
    {
        for (int i = 0; i < 3; i++)
        {
            Cell[] hcells = { cells[i * 3 + 0], cells[i * 3 + 1], cells[i * 3 + 2] };
            Cell[] vcells = { cells[0 * 3 + i], cells[1 * 3 + i], cells[2 * 3 + i] };
            Cell[] dfistcells = { cells[0], cells[4], cells[8] };
            Cell[] dsecondcells = { cells[2], cells[4], cells[6] };

            bool horizontal = cells[i * 3 + 0].value == cells[i * 3 + 1].value && cells[i * 3 + 2].value == cells[i * 3 + 0].value;
            bool vertical = cells[0 * 3 + i].value == cells[1 * 3 + i].value && cells[2 * 3 + i].value == cells[0 * 3 + i].value;
            bool firstDiagonal = cells[0].value == cells[4].value && cells[8].value == cells[0].value;
            bool secondDiagonal = cells[2].value == cells[4].value && cells[6].value == cells[2].value;

            if (horizontal || vertical || firstDiagonal || secondDiagonal)
            {
                Cell[] wincells = horizontal ? hcells : vertical ? vcells : firstDiagonal ? dfistcells : dsecondcells;
                foreach (Cell c in wincells)
                {
                    c.SetBackgroundColor();
                }
                break;
            }
        }
        if (win)
        {
            onWin.Invoke();
        }
        else
        {
            onEndGame.Invoke();
        }
        socketIO.Disconnect();
    }

    public void Play()
    {
        onPlay.Invoke();
        Connect();
    }

    public void Click(int index)
    {
        socketIO.Emit("click", _ =>
        {
            StatusText.text = "Waiting for your opponent...";
        }, index);
    }

    void Connect()
    {
        socketIO = new(uri)
        {
            JsonSerializer = new NewtonsoftJsonSerializer()
        };
        socketIO.OnConnected += (sender, e) =>
        {
            StatusText.text = "Waiting for your opponent...";
            socketIO.OnUnityThread("simbol", socketIOResponse =>
            {
                SimbolText.text = "You: " + socketIOResponse.GetValue<string>();
            });
            socketIO.OnUnityThread("your-turn", _ =>
            {
                StatusText.text = "Your turn!";
            });
            socketIO.OnUnityThread("start", _ =>
            {
                OnStartGame();
            });
            socketIO.OnUnityThread("update", socketIOResponse =>
            {
                OnUpdate(socketIOResponse.GetValue<Data>());
            });
            socketIO.OnUnityThread("win", socketIOResponse =>
            {
                OnEndGame(true);
            });
            socketIO.OnUnityThread("end-game", socketIOResponse =>
            {
                OnEndGame();
            });
        };
        socketIO.Connect();
    }

    public void Disconnect()
    {
        socketIO.Disconnect();
    }
}

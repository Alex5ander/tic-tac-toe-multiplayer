using System;
using System.Runtime.InteropServices;
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
    [SerializeField] UnityEvent onDisconnected;
    SocketIOUnity socketIO;
    [DllImport("__Internal")]
    static extern void ConnectWebGL();
    [DllImport("__Internal")]
    static extern void DisconnectWebGL();
    [DllImport("__Internal")]
    static extern void ClickWebGL(int index);
    readonly Uri uri = new("https://tic-tac-toe-multiplayer-server.onrender.com/");
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnUpdate(string json)
    {
        Data data = JsonUtility.FromJson<Data>(json);
        cells[data.index].SetText(data.simbol);
    }

    void OnUpdate(Data data)
    {
        cells[data.index].SetText(data.simbol);
    }

    void OnStartGame()
    {
        onStartGame.Invoke();
    }

    void OnEndGame(int win)
    {
        for (int i = 0; i < 3; i++)
        {
            Cell[] hcells = { cells[i * 3 + 0], cells[i * 3 + 1], cells[i * 3 + 2] };
            Cell[] vcells = { cells[0 * 3 + i], cells[1 * 3 + i], cells[2 * 3 + i] };
            Cell[] dfistcells = { cells[0], cells[4], cells[8] };
            Cell[] dsecondcells = { cells[2], cells[4], cells[6] };

            bool horizontal = cells[i * 3 + 0].value == cells[i * 3 + 1].value && cells[i * 3 + 2].value == cells[i * 3 + 0].value && cells[i * 3 + 0].value != "";
            bool vertical = cells[0 * 3 + i].value == cells[1 * 3 + i].value && cells[2 * 3 + i].value == cells[0 * 3 + i].value && cells[0 * 3 + i].value != "";
            bool firstDiagonal = cells[0].value == cells[4].value && cells[8].value == cells[0].value && cells[0].value != "";
            bool secondDiagonal = cells[2].value == cells[4].value && cells[6].value == cells[2].value && cells[2].value != "";

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
        if (win == 1)
        {
            onWin.Invoke();
        }
        else
        {
            onEndGame.Invoke();
        }
    }

    public void Play()
    {
        onPlay.Invoke();
#if UNITY_WEBGL && !UNITY_EDITOR
        ConnectWebGL();
#else
        Connect();
#endif
    }
    public void OpponentTurn()
    {
        StatusText.text = "Waiting for your opponent...";
    }

    public void Click(int index)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ClickWebGL(index);
#else
        socketIO.Emit("click", _ =>
        {
            OpponentTurn();
        }, index);
#endif
    }
    public void OnConnected()
    {
        StatusText.text = "Waiting for your opponent...";
    }

    public void OnSimbol(string simbol)
    {
        SimbolText.text = "You: " + simbol;
    }

    public void OnYourTurn()
    {
        StatusText.text = "Your turn!";
    }

    void Connect()
    {
        socketIO = new(uri)
        {
            JsonSerializer = new NewtonsoftJsonSerializer()
        };
        socketIO.OnConnected += (sender, e) =>
        {
            OnConnected();
        };
        socketIO.OnUnityThread("simbol", socketIOResponse =>
        {
            OnSimbol(socketIOResponse.GetValue<string>());
        });
        socketIO.OnUnityThread("your-turn", _ =>
        {
            OnYourTurn();
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
            OnEndGame(1);
        });
        socketIO.OnUnityThread("end-game", socketIOResponse =>
        {
            OnEndGame(0);
        });
        socketIO.OnDisconnected += (sender, e) =>
        {
            UnityThread.executeInUpdate(() =>
            {
                OnDisconnect();
            });
        };
        socketIO.Connect();
    }

    public void Disconnect()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            DisconnectWebGL();
#else
        if (socketIO != null && socketIO.Connected)
        {
            socketIO.Disconnect();
        }
#endif            
    }

    public void OnDisconnect()
    {
        onDisconnected.Invoke();
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }
}

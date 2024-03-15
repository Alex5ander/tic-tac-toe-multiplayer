using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] Image Backgroud;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] Socket socket;
    public int index;
    public string value;
    void OnEnable()
    {
        Text.text = "";
        Backgroud.color = Color.white;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        if (Text.text == "")
        {
            socket.Click(index);
        }
    }

    public void SetText(string value)
    {
        Text.text = value;
        this.value = value;
    }

    public void SetBackgroundColor()
    {
        Backgroud.color = Color.gray;
    }
}

using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] RectTransform Container;

    void Awake()
    {
        Container.anchorMin = new(Screen.safeArea.x / Screen.width, Screen.safeArea.y / Screen.height);
        Container.anchorMax = new((Screen.safeArea.x + Screen.safeArea.size.x) / Screen.width, (Screen.safeArea.y + Screen.safeArea.size.y) / Screen.height);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

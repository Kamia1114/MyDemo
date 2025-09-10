using Battle.Manager;
using Core.Mgr;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button startBtn = transform.Find("StartBtn").GetComponent<Button>();
        Button Btn1Year = transform.Find("GameMode/Btn1Year").GetComponent<Button>();
        Button Btn3Year = transform.Find("GameMode/Btn3Year").GetComponent<Button>();
        Button Btn10Year = transform.Find("GameMode/Btn10Year").GetComponent<Button>();
        startBtn.onClick.AddListener(() =>
        {
            // 切换到游戏场景
            GameManager.Instance.StartGame();
        });
        Btn1Year.onClick.AddListener(() =>
        {
            // 设置游戏模式为一年
            GameManager.Instance.SetGameMode(GameMode.MODE_1YEAR);
        });
        Btn3Year.onClick.AddListener(() =>
        {
            // 设置游戏模式为三年
            GameManager.Instance.SetGameMode(GameMode.MODE_3YEAR);
        });
        Btn10Year.onClick.AddListener(() =>
        {
            // 设置游戏模式为十年
            GameManager.Instance.SetGameMode(GameMode.MODE_10YEAR);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void InitUI()
    {
        // 初始化UI元素
    }
}

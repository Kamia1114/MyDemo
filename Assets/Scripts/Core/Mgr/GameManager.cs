using System.Collections.Generic;
using Battle.Manager;
using Core.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Mgr
{
    public class GameManager : MonoBehaviour
    {
        private GameMode currentGameMode;
        public GameMode CurrentGameMode
        {
            get { return currentGameMode; }
        }

        private int playerCount;
        public int PlayerCount
        {
            get { return playerCount; }
        }

        private List<PlayerData> playerDataList;
        public List<PlayerData> PlayerDataList
        {
            get { return playerDataList; }
        }

        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // 关键：让GameManager跨场景保留
            }
            else
            {
                Destroy(gameObject); // 防止重复创建
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // 初始化游戏管理器
            InitGame();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void InitGame()
        {
            currentGameMode = GameMode.MODE_1YEAR; // 默认游戏模式为一年
            SetPlayerCount(4); // 默认玩家数量为4
            //默认玩家id
            SetPlayerID(0, 88001, 102); // 玩家1 ID
            SetPlayerID(1, 88002, 103); // 玩家2 ID
            SetPlayerID(2, 222, 104, true); // 玩家3 ID
            SetPlayerID(3, 333, 106, true); // 玩家4 ID
        }

        public void StartGame()
        {
            // 游戏开始逻辑
            Debug.Log("游戏开始！");
            ConfigManager.ChangeScene(SceneState.Battle);
            SceneManager.LoadScene("Game");
        }

        public void ExitGame()
        {
            // 退出游戏逻辑
            Debug.Log("退出游戏");
            Application.Quit();
        }

        public void SetGameMode(GameMode mode)
        {
            this.currentGameMode = mode;
            Debug.Log("游戏模式已设置为: " + mode);
        }

        public void SetPlayerCount(int count)
        {
            this.playerCount = count;
        }
        
        /// <summary>
        /// 设置玩家信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="userId"></param>
        /// <param name="characterId"></param>
        /// <param name="isAI"></param>
        public void SetPlayerID(int index, int userId, int characterId, bool isAI = false)
        {
            playerDataList ??= new List<PlayerData>();
            playerDataList.Add(new PlayerData { index = index, id = userId,  characterId = characterId, isAI = isAI});
            // 设置玩家ID逻辑
            Debug.Log($"玩家{userId}的ID已设置为: {characterId}");
        }
    }
}
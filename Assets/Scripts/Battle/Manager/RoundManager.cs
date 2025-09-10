using System;
using Core.Mgr;
using UnityEngine;

namespace Battle.Manager
{
    public enum GameMode
    {
        MODE_1YEAR,     // 一年模式
        MODE_3YEAR,     // 三年模式
        MODE_10YEAR     // 十年模式
    }

    public enum Season
    {
        SPRING,
        SUMMER,
        AUTUMN,
        WINTER,
    }

    /// <summary>
    /// 只负责回合数、当前玩家索引、回合推进等数据和流程。
    /// 通过事件通知BattleManager。
    /// </summary>
    public class RoundManager : MonoBehaviour
    {
        private int curRound;                // 当前回合（月）
        private int currentPlayIndex;        // 当前玩家索引
        private int curRoundPassed;          // 总经过的回合数
        private int endRound;                // 游戏结束回合数
        public event Action OnRoundOver;         // 当前回合完成
        public event Action<int> OnPlayIndexChanged; // 当前玩家索引变化事件
        public static RoundManager Instance { get; private set; }

        public int CurRound => curRound;
        public int CurrentPlayIndex => currentPlayIndex;
        public int CurRoundPassed => curRoundPassed;
        public int EndRound => endRound;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void Init()
        {
            var mode = GameManager.Instance.CurrentGameMode;
            curRoundPassed = 0;
            curRound = 0;
            currentPlayIndex = 0;
            endRound = mode switch
            {
                GameMode.MODE_1YEAR => 12,
                GameMode.MODE_3YEAR => 36,
                GameMode.MODE_10YEAR => 120,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
            };
        }

        public int CurMonth() => (curRound % 12) + 1;
        public int CurYear() => (curRound / 12) + 1;
        public Season CurSeason() => (Season)((curRound % 12) / 3);

        /// <summary>
        /// 游戏开始时调用，重置所有回合状态
        /// </summary>
        public void StartGame()
        {
            Init();
            StartRound();
        }

        /// <summary>
        /// 开始新一回合
        /// </summary>
        public void StartRound()
        {
            curRound = 1;
            curRoundPassed = 0;
            currentPlayIndex = 0;
            PlayerStart();
        }

        /// <summary>
        /// 进入下一回合
        /// </summary>
        public void NextRound()
        {
            if (curRoundPassed >= endRound)
            {
                BattleManager.Instance.GameOver();
                return;
            }
            curRound = (curRound + 1) % 12;
            PlayerStart();
        }

        /// <summary>
        /// 开始当前回合首个玩家行动
        /// </summary>
        public void PlayerStart()
        {
            currentPlayIndex = 0;
            OnPlayIndexChanged?.Invoke(currentPlayIndex);
        }

        /// <summary>
        /// 开始下个玩家行动
        /// </summary>
        public void PlayerNext()
        {
            currentPlayIndex++;
            int playerCount = GameManager.Instance.PlayerCount;
            if (currentPlayIndex == playerCount)
            {
                curRoundPassed++;
                OnRoundOver?.Invoke();
            }
            else
            {
                OnPlayIndexChanged?.Invoke(currentPlayIndex);
            }
        }
    }
}
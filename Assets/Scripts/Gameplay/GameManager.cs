using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

namespace Cards.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] bool m_GameBegun = false;
        [SerializeField] int m_InitialAmountOfCards = 4;
        [SerializeField] CardHolderStock m_CardStockHolder;
        [SerializeField] CardHolderStock m_EnemyCardStockHolder;
        [SerializeField] PlayerCardStock m_playerStock;
        [SerializeField] PlayerCardStock m_EnemyStock;

        [SerializeField] Player[] m_Players;

        [SerializeField] Player m_CurrentPlayer;
        [SerializeField] int m_Turns = 0;
        [SerializeField] int m_Rounds = 0;
        [SerializeField] int m_CardsRecieved = 0;

        [SerializeField] TextMeshProUGUI m_Label;

        public static GameManager instance { get => m_Instance; }
        private static GameManager m_Instance;

        private static float m_LerpSpeed = 0.35f;
        public static float lerpSpeed { get => m_LerpSpeed; }

        public bool gameHasBegun { get => m_GameBegun; }

        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else if (m_Instance == this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OrderCards(int amount)
        {
            m_CardStockHolder.OrderCards(amount, m_playerStock, true);
            m_EnemyCardStockHolder.OrderCards(amount, m_EnemyStock, false);
        }

        private void Start()
        {
            OrderCards(m_InitialAmountOfCards);

            m_CurrentPlayer = m_Players[Random.Range(0, m_Players.Length)];
            StartCoroutine(CheckForGameBegin());
            m_Rounds++;

            ShowLabel("Game begins");
        }

        private void ShowLabel(string text)
        {
            m_Label.text = text;
            RectTransform rect = m_Label.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector3(-1000, 0, 0);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(rect.DOAnchorPos(new Vector2(0, 0), 1));
            sequence.AppendInterval(1);
            sequence.Append(rect.DOAnchorPos(new Vector2(1000, 0), 1));
        }

        private IEnumerator CheckForGameBegin()
        {
            while (!m_GameBegun)
            {
                if (m_EnemyStock.cards.Length >= m_InitialAmountOfCards && m_playerStock.cards.Length >= m_InitialAmountOfCards)
                {
                    m_GameBegun = true;
                }

                yield return null;
            }
        }

        public bool IsMyTurn(Player player)
        {
            return m_CurrentPlayer == player;
        }

        public void EndTurn(Player player)
        {
            if (m_CurrentPlayer == player)
            {
                int index = 0;
                for (int i = 0; i < m_Players.Length; i++)
                {
                    if (m_Players[i] == player)
                    {
                        index = i;
                        break;
                    }
                }

                index++;

                if (index >= m_Players.Length)
                {
                    index = 0;
                }

                m_CurrentPlayer = m_Players[index];

                m_Turns++;
            }

            if (m_Turns >= 2)
            {
                m_GameBegun = false;
                EndRound();
            }
        }

        private void EndRound()
        {
            // Restore energy
            foreach (Player playerChar in m_Players)
            {
                playerChar.energy = m_InitialAmountOfCards + m_Rounds;

                foreach (CardBehaviour card in playerChar.battleDeck.cards)
                {
                    card.OnNewRound();
                }

                if (playerChar is EnemyBehaviour)
                {
                    (playerChar as EnemyBehaviour).Unlock();
                }
            }

            m_Rounds++;
            m_Turns = 0;

            ShowLabel("new round");

            StartCoroutine(GiveCardsWithDelay());
        }

        public void ResumeGame()
        {
            m_CardsRecieved++;

            if (m_CardsRecieved >= 2)
            {
                m_GameBegun = true;
                m_CardsRecieved = 0;
            }
        }

        IEnumerator GiveCardsWithDelay()
        {
            yield return new WaitForSeconds(1);
            OrderCards(1);
        }

        private void Update()
        {
            if (!m_GameBegun)
                return;

            foreach (Player player in m_Players)
            {
                if (player.health <= 0)
                {
                    if (player is WorldInteractor) ShowLabel("You lose");
                    else
                        ShowLabel("You win");
                }
            }
        }
    }
}

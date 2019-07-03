using UnityEngine;
using TMPro;
using Cards.Gameplay;

namespace Cards
{
    public class Player : MonoBehaviour
    {
        public const int MAX_ENERGY = 10;
        public int energy { get => m_CurrentEnergy; set { m_CurrentEnergy = value; m_CurrentEnergy = Mathf.Clamp(m_CurrentEnergy, 0, MAX_ENERGY); } }

        [SerializeField] protected int m_CurrentEnergy = 4;

        [SerializeField] TextMeshProUGUI m_Label;
        [SerializeField] TextMeshProUGUI m_HealthLabel;

        [SerializeField] protected BattleStock m_BattleStock;
        [SerializeField] protected PlayerCardStock m_CardStock;
        [SerializeField] protected CardHolderStock m_CardHolderStock;
        [SerializeField] protected CardStock m_BeatenDeck;

        public BattleStock battleDeck { get => m_BattleStock; }
        public CardStock beatenDeck { get => m_BeatenDeck; }
        public int health { get => m_BattleStock.cards.Length + m_CardStock.cards.Length; }

        protected void ClaimCards(CardBehaviour.Status status)
        {
            foreach (CardBehaviour card in m_CardStock.cards)
            {
                card.status = status;
            }
        }

        protected void DrawUI()
        {
            if (m_CurrentEnergy > 0)
            {
                m_Label.text = string.Format("Energy: {0} / {1}", m_CurrentEnergy, MAX_ENERGY);
            } else
            {
                m_Label.text = "Press space to end turn";
            }
            m_HealthLabel.text = string.Format("Health: {0}", health);
        }

        public bool IsCardMine(CardBehaviour card)
        {
            bool mine = false;

            foreach (CardBehaviour stockCard in m_CardStock.cards)
            {
                if (stockCard == card)
                {
                    mine = true;
                    break;
                }
            }

            foreach (CardBehaviour stockCard in m_BattleStock.cards)
            {
                if (stockCard == card)
                {
                    mine = true;
                    break;
                }
            }

            return mine;
        }
    }
}

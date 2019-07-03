using UnityEngine;
using DG.Tweening;

namespace Cards.Gameplay
{
    public class CardBehaviour : MonoBehaviour
    {
        public enum Status
        {
            NotTaken,
            Taken,
            Demonstrating,
            Usable,
        }

        [SerializeField] bool m_IsSleeping = true;
        [SerializeField] Status m_Status;
        [SerializeField] CardData m_Data;
        [SerializeField] CardStock m_Stock;

        public CardStock stock { get => m_Stock; set => m_Stock = value; }
        public bool isSleeping { get => m_IsSleeping; set => m_IsSleeping = value; }
        public CardData data { get => m_Data; }
        public Status status { get => m_Status; set => m_Status = value; }

        public void OnNewRound()
        {
            m_IsSleeping = false;
        }

        public void DealDamage(int damage, CardStock beatenDeck)
        {
            CardData data = m_Data;
            data.health -= damage;
            m_Data = data;

            if (m_Data.health <= 0)
            {
                m_Stock.RemoveCard(this);
                m_Stock = null;
                transform.DOMove(beatenDeck.transform.position, GameManager.lerpSpeed);
                transform.DORotateQuaternion(beatenDeck.transform.rotation, 0.8f);
            }
        }

        //public void ChangeDeck(CardStock newDeck)
        //{
        //    m_Stock?.RemoveCard(this);
        //    m_Stock = newDeck;
        //    m_Stock.AddCard(this);
        //}
    }
}

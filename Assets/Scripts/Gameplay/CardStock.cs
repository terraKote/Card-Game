using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;

namespace Cards.Gameplay
{
    public class CardStock : MonoBehaviour
    {
        [SerializeField] protected CardBehaviour[] m_Cards;
        [SerializeField] public Vector3 m_CardPadding = Vector3.zero;

        public CardBehaviour[] cards { get => m_Cards; }

        public void AllignCards()
        {
            Vector3 offset = Vector3.zero;
            float stockSize = 0;

            stockSize = m_CardPadding.x * (m_Cards.Length - 1);
            offset.x -= stockSize / 2;

            for (int i = 0; i < m_Cards.Length; i++)
            {
                m_Cards[i].transform.DOMove(transform.position + new Vector3(offset.x + m_CardPadding.x * i, 0, 0), GameManager.lerpSpeed);
                m_Cards[i].transform.DORotate(transform.eulerAngles, GameManager.lerpSpeed);
            }
        }

        public void AddCards(CardBehaviour[] cards)
        {
            foreach (CardBehaviour card in cards)
            {
                AddCard(card);
            }
        }

        public void AddCard(CardBehaviour card)
        {
            List<CardBehaviour> cards = new List<CardBehaviour>();
            cards.AddRange(m_Cards);
            if (!cards.Contains(card))
            {
                cards.Add(card);
            }
            m_Cards = cards.ToArray();
        }

        public void RemoveCards(CardBehaviour[] cards)
        {
            foreach (CardBehaviour card in cards)
            {
                RemoveCard(card);
            }
        }

        public void RemoveCard(CardBehaviour card)
        {
            List<CardBehaviour> cards = new List<CardBehaviour>();
            cards.AddRange(m_Cards);
            cards.Remove(card);
            m_Cards = cards.ToArray();
        }
    }
}

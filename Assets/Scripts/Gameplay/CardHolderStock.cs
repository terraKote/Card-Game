using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Cards.Gameplay
{
    public class CardHolderStock : CardStock
    {
        [SerializeField] float m_DemonstrationTime = 3f;
        [SerializeField] Vector3 m_DemoHeight;
        [SerializeField] Vector3 m_PickRotation;
        [Range(0, 1)]
        [SerializeField] float m_LerpTime = 0.23f;

        [SerializeField] float m_check;

        private void OnEnable()
        {
            for (int i = 0; i < m_Cards.Length; i++)
            {
                m_Cards[i].transform.position = transform.position + m_CardPadding * i;
                m_Cards[i].transform.rotation = transform.rotation;
            }
        }

        public void OrderCards(int amount, CardStock destination, bool demonstrate)
        {
            if (m_Cards.Length <= 0 || m_Cards.Length < amount)
            {
                GameManager.instance.ResumeGame();
                return;
            }

            List<CardBehaviour> cards = new List<CardBehaviour>();

            for (int i = 0; i < amount; i++)
            {
                CardBehaviour card = m_Cards[Random.Range(0, m_Cards.Length)];
                cards.Add(card);
                RemoveCard(card);
                card.stock = destination;
              //  destination.AddCard(card);
            }

            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < cards.Count; i++)
            {
                //   cards[i].stock = destination;

                if (demonstrate)
                {
                    sequence.Append(cards[i].transform.DOMove(Camera.main.transform.position + m_DemoHeight, GameManager.lerpSpeed));
                    sequence.Join(cards[i].transform.DORotate(m_PickRotation, GameManager.lerpSpeed));

                    sequence.AppendInterval(m_DemonstrationTime);
                }

                sequence.Append(cards[i].transform.DOMove(destination.transform.position, GameManager.lerpSpeed));
                sequence.Join(cards[i].transform.DORotate(destination.transform.eulerAngles, GameManager.lerpSpeed));
            }

            sequence.OnComplete(() =>
            {
                destination.AddCards(cards.ToArray());
                destination.AllignCards();
                GameManager.instance.ResumeGame();
            });
        }
    }
}

using UnityEngine;
using Cards.Gameplay;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;

namespace Cards
{
    public class EnemyBehaviour : Player
    {
        [SerializeField] BattleStock m_PlayerBattleDeck;
        GlitchEffect glitch;
        private bool m_Moved = false;

        private void Awake()
        {
            glitch = Camera.main.GetComponent<GlitchEffect>();
            glitch.enabled = false;
        }

        public void Unlock()
        {
            m_Moved = false;
        }

        private void Update()
        {
            if (GameManager.instance.gameHasBegun && GameManager.instance.IsMyTurn(this))
            {
                // Decide whether we sould make move or beat existing cards
                // if we have no cards to move, then put some
                if (m_CardStock.cards.Length > 0 && m_BattleStock.cards.Length == 0)
                {
                    // Select cards to move
                    List<CardBehaviour> selectedCards = new List<CardBehaviour>();
                    int potentialEnergy = m_CurrentEnergy;

                    while (potentialEnergy > 0 || selectedCards.Count != m_CardStock.cards.Length)
                    {
                        CardBehaviour card = m_CardStock.cards[Random.Range(0, m_CardStock.cards.Length)];

                        if (!selectedCards.Contains(card))
                        {
                            selectedCards.Add(card);
                            potentialEnergy -= card.data.requiredEnenergy;

                            if (potentialEnergy < 0)
                            {
                                selectedCards.Remove(card);
                                break;
                            }
                        }
                        else if (m_CardStock.cards.Length <= 1)
                        {
                            break;
                        }
                    }

                    // Make move
                    if (selectedCards.Count > 0)
                    {
                        Sequence sequence = DOTween.Sequence();

                        for (int i = 0; i < selectedCards.Count; i++)
                        {
                            m_CardStock.RemoveCard(selectedCards[i]);
                            selectedCards[i].stock = m_BattleStock;
                            m_BattleStock.AddCard(selectedCards[i]);

                            m_CurrentEnergy -= selectedCards[i].data.requiredEnenergy;

                            sequence.Append(selectedCards[i].transform.DOMove(m_BattleStock.transform.position, GameManager.lerpSpeed).OnComplete(() => SoundManager.instance.PlaySound(SoundManager.SoundType.CardPlace)));
                            sequence.Join(selectedCards[i].transform.DORotate(m_BattleStock.transform.eulerAngles, GameManager.lerpSpeed));
                        }

                        sequence.OnComplete(() => { m_BattleStock.AllignCards(); GameManager.instance.EndTurn(this); });
                        m_Moved = true;
                    }
                }
                // Beat enemy's cards
                else if (m_BattleStock.cards.Select(e => !e.isSleeping).Count() > 0)
                {
                    if (m_BattleStock.cards.Length > 0 && m_PlayerBattleDeck.cards.Length > 0)
                    {
                        print("Battle cards");

                        Sequence sequence = DOTween.Sequence();
                        CardBehaviour target = m_PlayerBattleDeck.cards.FirstOrDefault(element => element.data.health > 0);

                        foreach (CardBehaviour card in m_BattleStock.cards)
                        {
                            if (!card.isSleeping && m_CurrentEnergy >= card.data.requiredEnenergy)
                            {
                                sequence.Append(card.transform.DOPunchPosition(target.transform.position - card.transform.position, GameManager.lerpSpeed, 0, 0).OnComplete(() =>
                                {
                                    glitch.enabled = true;
                                    target.DealDamage(card.data.damage, m_BeatenDeck);
                                    SoundManager.instance.PlaySound(SoundManager.SoundType.PlayerHit);
                                    Camera.main.transform.DOShakePosition(GameManager.lerpSpeed, 0.1f).OnComplete(() => glitch.enabled = false);
                                    m_CurrentEnergy -= card.data.requiredEnenergy;
                                    card.isSleeping = true;
                                }));
                            }

                            if (target.data.health <= 0 || m_CurrentEnergy <= 0)
                                break;
                        }

                        sequence.OnComplete(() => { glitch.enabled = false; m_BattleStock.AllignCards(); ; GameManager.instance.EndTurn(this); });
                        m_Moved = true;
                    }
                    else
                    {
                        m_Moved = true;
                    }
                }
                else
                {
                    m_Moved = true;
                }
            }

            if (m_Moved)
                GameManager.instance.EndTurn(this);


            DrawUI();
        }
    }
}

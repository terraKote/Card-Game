using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Cards.Gameplay
{
    public class WorldInteractor : Player
    {
        [SerializeField] ParticleSystem m_SelectParticles;

        private CardBehaviour m_SelectedCard;
        private Vector3 m_CardInitialPosition;

        private void Update()
        {
            DrawUI();

            if (!GameManager.instance.gameHasBegun)
                return;

            ClaimCards(CardBehaviour.Status.Usable);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                CardBehaviour card = hit.collider.GetComponent<CardBehaviour>();
                BattleStock battleStock = hit.collider.GetComponent<BattleStock>();

                if (Input.GetButtonDown("Fire1"))
                {
                    if (GameManager.instance.IsMyTurn(this))
                    {
                        if (card && card.status == CardBehaviour.Status.Usable && IsCardMine(card))
                        {
                            m_SelectedCard = card;
                            m_CardInitialPosition = card.transform.position;
                            m_SelectedCard.gameObject.layer = 2;

                            m_SelectParticles.Stop();
                        }
                    }
                }

                if (Input.GetButton("Fire1"))
                {
                    if (GameManager.instance.IsMyTurn(this))
                    {
                        if (m_SelectedCard)
                        {
                            m_SelectedCard.transform.position = Vector3.Lerp(m_SelectedCard.transform.position, hit.point + new Vector3(0, 1, 0), GameManager.lerpSpeed * 3);
                        }
                    }
                }
                else
                {
                    if (card && card.status == CardBehaviour.Status.Usable && IsCardMine(card))
                    {
                        if (GameManager.instance.IsMyTurn(this))
                        {
                            m_SelectParticles.transform.position = card.transform.position;
                            m_SelectParticles.transform.rotation = card.transform.rotation;

                            if (!m_SelectParticles.isPlaying)
                                m_SelectParticles.Play();
                        }
                    }
                    else
                    {
                        m_SelectParticles.Stop();
                    }
                }

                if (Input.GetButtonUp("Fire1"))
                {
                    if (GameManager.instance.IsMyTurn(this))
                    {
                        if (m_SelectedCard)
                        {
                            // if we found battle deck and it's ours
                            if (battleStock && battleStock == m_BattleStock && m_CurrentEnergy >= m_SelectedCard.data.requiredEnenergy)
                            {
                                m_CardStock.RemoveCard(m_SelectedCard);
                                m_BattleStock.AddCard(m_SelectedCard);
                                m_SelectedCard.stock = m_BattleStock;
                                SoundManager.instance.PlaySound(SoundManager.SoundType.CardPlace);
                                m_SelectedCard.gameObject.layer = 9;
                                m_BattleStock.AllignCards();

                                m_CurrentEnergy -= m_SelectedCard.data.requiredEnenergy;
                            }
                            //if we found enemy's card
                            else if (card && !m_SelectedCard.isSleeping && !IsCardMine(card) && m_SelectedCard.stock == m_BattleStock && m_CurrentEnergy >= m_SelectedCard.data.requiredEnenergy)
                            {
                                card.DealDamage(m_SelectedCard.data.damage, m_BeatenDeck);
                                SoundManager.instance.PlaySound(SoundManager.SoundType.CardHit);
                                card.transform.DOShakeRotation(GameManager.lerpSpeed);
                                m_SelectedCard.transform.DOMove(m_CardInitialPosition, GameManager.lerpSpeed);
                                m_SelectedCard.isSleeping = true;

                                m_CurrentEnergy -= m_SelectedCard.data.requiredEnenergy;
                            }
                            // if nothing
                            else
                            {
                                m_SelectedCard.transform.DOMove(m_CardInitialPosition, GameManager.lerpSpeed);
                            }

                            m_SelectedCard.gameObject.layer = 9;
                            m_SelectedCard = null;
                        }
                    }
                }

                if (!card)
                    m_SelectParticles.Stop();
            }
            else
            {
                if (m_SelectedCard)
                {
                    m_SelectedCard.gameObject.layer = 9;
                    m_SelectedCard.transform.DOMove(m_CardInitialPosition, GameManager.lerpSpeed);
                    m_SelectedCard = null;
                }
            }

            if (!m_SelectedCard)
            {
                m_BattleStock.gameObject.layer = 2;
            }
            else
            {
                m_BattleStock.gameObject.layer = 0;
            }

            if (Input.GetButtonDown("Jump"))
            {
                GameManager.instance.EndTurn(this);
            }
        }
    }
}

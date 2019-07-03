using UnityEngine;

namespace Cards
{
    public class SoundManager : MonoBehaviour
    {
        public enum SoundType
        {
            CardHit,
            PlayerHit,
            CardPlace,
        }

        public static SoundManager instance { get => m_Instance; }
        private static SoundManager m_Instance;

        [SerializeField] AudioClip m_CardHit;
        [SerializeField] AudioClip m_CardPlace;
        [SerializeField] AudioClip m_PlayerHit;

        private AudioSource m_Source;

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

            m_Source = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.CardHit:
                    m_Source.clip = m_CardHit;
                    break;

                case SoundType.CardPlace:
                    m_Source.clip = m_CardPlace;
                    break;

                case SoundType.PlayerHit:
                    m_Source.clip = m_PlayerHit;
                    break;
            }
            m_Source.Play();
        }
    }
}

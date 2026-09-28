using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace MoonMonster.Codetest
{
    public class TankHealth : MonoBehaviour
    {
        [SerializeField] private float _startingHealth = 100f;
        [SerializeField, Required] private Slider _slider;
        [SerializeField, Required] private Image _fillImage;
        [SerializeField] private Color _fullHealthColor = Color.green;
        [SerializeField] private Color _zeroHealthColor = Color.red;
        [SerializeField, Required] private GameObject _explosionPrefab;

        private AudioSource _explosionAudio;
        private ParticleSystem _explosionParticles;
        private float _currentHealth;
        private bool _dead;

        private void Awake()
        {
            _explosionParticles = Instantiate(_explosionPrefab).GetComponent<ParticleSystem>();
            _explosionAudio = _explosionParticles.GetComponent<AudioSource>();
            _explosionParticles.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _currentHealth = _startingHealth;
            _dead = false;

            SetHealthUI();
        }

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;

            SetHealthUI();

            if (_currentHealth <= 0f && !_dead)
            {
                OnDeath();
            }
        }

        private void SetHealthUI()
        {
            _slider.value = _currentHealth;

            _fillImage.color = Color.Lerp(_zeroHealthColor, _fullHealthColor, _currentHealth / _startingHealth);
        }

        private void OnDeath()
        {
            _dead = true;

            _explosionParticles.transform.position = transform.position;
            _explosionParticles.gameObject.SetActive(true);

            _explosionParticles.Play();

            _explosionAudio.Play();

            gameObject.SetActive(false);
        }
    }
}
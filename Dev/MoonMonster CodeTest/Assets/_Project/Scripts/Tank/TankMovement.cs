using NaughtyAttributes;
using UnityEngine;

namespace MoonMonster.Codetest
{
    public class TankMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 12f;
        [SerializeField] private float _turnSpeed = 180f;
        [SerializeField, Required] private AudioSource _movementAudio;
        [SerializeField, Required] private AudioClip _engineIdling;
        [SerializeField, Required] private AudioClip _engineDriving;
        [SerializeField] private float _pitchRange = 0.2f;
        [SerializeField, Required] private GameObject _tankBody;
        [SerializeField, Required] private Rigidbody _rigidbody;

        private float _verticalInputValue;
        private float _horizontalInputValue;
        private float _originalPitch;
        private ParticleSystem[] _particleSystems;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _rigidbody.isKinematic = false;

            _verticalInputValue = 0f;
            _horizontalInputValue = 0f;

            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < _particleSystems.Length; ++i)
            {
                _particleSystems[i].Play();
            }
        }

        private void OnDisable()
        {
            _rigidbody.isKinematic = true;

            for (int i = 0; i < _particleSystems.Length; ++i)
            {
                _particleSystems[i].Stop();
            }
        }

        private void Start()
        {
            _originalPitch = _movementAudio.pitch;
        }

        private void Update()
        {
            EngineAudio();
        }

        public void SetMoveInput(float movement, float turn)
        {
            _verticalInputValue = movement;
            _horizontalInputValue = turn;
        }

        private void EngineAudio()
        {
            if (Mathf.Abs(_verticalInputValue) < 0.1f && Mathf.Abs(_horizontalInputValue) < 0.1f)
            {
                if (_movementAudio.clip == _engineDriving)
                {
                    _movementAudio.clip = _engineIdling;
                    _movementAudio.pitch = Random.Range(_originalPitch - _pitchRange, _originalPitch + _pitchRange);
                    _movementAudio.Play();
                }
            }
            else
            {
                if (_movementAudio.clip == _engineIdling)
                {
                    _movementAudio.clip = _engineDriving;
                    _movementAudio.pitch = Random.Range(_originalPitch - _pitchRange, _originalPitch + _pitchRange);
                    _movementAudio.Play();
                }
            }
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (_verticalInputValue == 0 && _horizontalInputValue == 0)
                return;

            var forward = _camera.transform.forward;
            var right = _camera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 movement = (_verticalInputValue * forward + _horizontalInputValue * right) * (_speed * Time.fixedDeltaTime);

            _rigidbody.MovePosition(_rigidbody.position + movement);

            _tankBody.transform.forward = Vector3.MoveTowards(_tankBody.transform.forward, movement.normalized,
                _turnSpeed * Time.fixedDeltaTime);
        }
    }
}
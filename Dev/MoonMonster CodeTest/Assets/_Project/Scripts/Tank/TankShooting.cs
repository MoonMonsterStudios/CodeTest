using NaughtyAttributes;
using UnityEngine;

namespace MoonMonster.Codetest
{
    public class TankShooting : MonoBehaviour
    {
        [SerializeField] private bool _lookAtMouse;
        [SerializeField, Required] private Rigidbody _shell;
        [SerializeField, Required] private Transform _fireTransform;
        [SerializeField, Required] private AudioSource _shootingAudio;
        [SerializeField, Required] private AudioClip _fireClip;
        [SerializeField] private float _launchForce = 15f;
        [SerializeField] private float _fireDelay = 0.3f;
        [SerializeField, Required] private GameObject _turret;
        [SerializeField] private float _angleOffset = 90f;

        private float _reloadCountdown;
        private bool _fired;
        private Camera _camera;
        
        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if(_lookAtMouse)
                LookAtMousePosition();
            
            if (_fired)
            {                
                if(_reloadCountdown <= 0)
                    _fired = false;
                else
                    _reloadCountdown -= Time.deltaTime;
            }
        }
        
        public void LookAtTarget(Transform target)
        {
            if (target == null)
                return;
            
            var dir = target.position - _turret.transform.position;
            dir.y = 0;
            dir.Normalize();
            
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - _angleOffset;
            _turret.transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);
        }

        public void Fire()
        {
            if(_fired)
                return;
            
            Rigidbody shellInstance =
                Instantiate(_shell, _fireTransform.position, _fireTransform.rotation) as Rigidbody;

            shellInstance.linearVelocity = _launchForce * _fireTransform.forward;

            _shootingAudio.clip = _fireClip;
            _shootingAudio.Play();
            
            _fired = true;
            _reloadCountdown = _fireDelay;
        }
        
        private void LookAtMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _camera.ScreenPointToRay(mousePos);
            
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 targetPos = ray.GetPoint(distance);
                var dir = targetPos - _turret.transform.position;
                dir.y = 0;
                dir.Normalize();
            
                var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - _angleOffset;
                _turret.transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);
            }
        }
    }
}
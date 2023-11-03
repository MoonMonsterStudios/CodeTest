using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoonMonster.Codetest
{
    public class TankShooting : MonoBehaviour
    {
        public bool LookAtMouse = false;
        public Rigidbody Shell;
        public Transform FireTransform;
        public AudioSource ShootingAudio;
        public AudioClip FireClip;
        public float LaunchForce = 15f;
        public float FireDelay = 0.1f;
        public GameObject Turret;
        public float AngleOffset = 90f; 
        
        private float _reloadCountdown;
        private bool _fired;
        private Camera _camera;
        
        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if(LookAtMouse == true)
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
            
            var dir = target.position - Turret.transform.position;
            dir.y = 0;
            dir.Normalize();
            
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - AngleOffset;
            Turret.transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);
        }

        public void Fire()
        {
            if(_fired)
                return;
            
            Rigidbody shellInstance =
                Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;

            shellInstance.velocity = LaunchForce * FireTransform.forward;

            ShootingAudio.clip = FireClip;
            ShootingAudio.Play();
            
            _fired = true;
            _reloadCountdown = FireDelay;
        }
        
        private void LookAtMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _camera.ScreenPointToRay(mousePos);
            
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 targetPos = ray.GetPoint(distance);
                var dir = targetPos - Turret.transform.position;
                dir.y = 0;
                dir.Normalize();
            
                var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - AngleOffset;
                Turret.transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);
            }
        }
    }
}
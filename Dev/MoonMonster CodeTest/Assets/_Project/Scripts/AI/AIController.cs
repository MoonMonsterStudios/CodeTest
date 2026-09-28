using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoonMonster.Codetest
{
    public class AIController : MonoBehaviour
    {
        public float AggressionDistance = 15;

        private TankShooting _shooting;
        
        private string _movementAxisName;
        private string _turnAxisName;
        private string _fireButtonName;

        public Transform Target { get; set; }
        
        void Start()
        {
            _shooting = gameObject.GetComponent<TankShooting>();
        }
        
        void Update()
        {
            _shooting.LookAtTarget(Target);
            
            if(Vector3.Distance(transform.position, Target.position) < AggressionDistance)
            {
                _shooting.Fire();
            }
        }
    }
}
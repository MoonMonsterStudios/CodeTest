using NaughtyAttributes;
using UnityEngine;

namespace MoonMonster.Codetest
{
    public class AIController : MonoBehaviour
    {
        [SerializeField, Required] private TankShooting _shooting;
        [SerializeField] private float _aggressionDistance = 15;

        public Transform Target { get; set; }
        
        void Update()
        {
            _shooting.LookAtTarget(Target);
            
            if(Vector3.Distance(transform.position, Target.position) < _aggressionDistance)
            {
                _shooting.Fire();
            }
        }
    }
}
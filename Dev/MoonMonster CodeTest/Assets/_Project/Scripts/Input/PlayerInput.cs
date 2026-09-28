using NaughtyAttributes;
using UnityEngine;

namespace MoonMonster.Codetest
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField, Required] private TankMovement _movement;
        [SerializeField, Required] private TankShooting _shooting;

        public int PlayerNumber { get; set; }

        private string _movementAxisName;
        private string _turnAxisName;
        private string _fireButtonName;
        
        void Start()
        {            
            _movementAxisName = "Vertical" + PlayerNumber;
            _turnAxisName = "Horizontal" + PlayerNumber;
            _fireButtonName = "Fire" + PlayerNumber;
        }

        void Update()
        {
            _movement.SetMoveInput(Input.GetAxis (_movementAxisName),Input.GetAxis (_turnAxisName));
            
            if (Input.GetButton(_fireButtonName))
            {
                _shooting.Fire();
            }
            else if (Input.GetButtonUp(_fireButtonName))
            {
                _shooting.Fire();
            }
        }
    }
}
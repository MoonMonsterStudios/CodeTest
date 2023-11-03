using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoonMonster.Codetest
{
    public class PlayerInput : MonoBehaviour
    {
        public int PlayerNumber = 1; 
        private TankMovement _movement;
        private TankShooting _shooting;

        
        private string _movementAxisName;         
        private string _turnAxisName;    
        private string _fireButtonName;
        
        void Start()
        {
            _movement = gameObject.GetComponent<TankMovement>();
            _shooting = gameObject.GetComponent<TankShooting>();
            
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
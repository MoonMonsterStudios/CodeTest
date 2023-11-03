using UnityEngine;

namespace MoonMonster.Codetest
{
    public class UIDirectionControl : MonoBehaviour
    {
        public bool UseRelativeRotation = true;       

        private Quaternion _relativeRotation;
        
        private void Start ()
        {
            _relativeRotation = transform.parent.localRotation;
        }
        
        private void Update ()
        {
            if (UseRelativeRotation)
                transform.rotation = _relativeRotation;
        }
    }
}
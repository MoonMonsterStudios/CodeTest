using UnityEngine;

namespace MoonMonster.Codetest
{
    public class UIDirectionControl : MonoBehaviour
    {
        private Quaternion _relativeRotation;
        
        private void Start ()
        {
            _relativeRotation = transform.parent.localRotation;
        }
        
        private void Update ()
        {
            transform.rotation = _relativeRotation;
        }
    }
}
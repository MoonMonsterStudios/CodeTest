using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoonMonster.Codetest
{
    [Serializable]
    public class TankManager
    {
        public Color TankColor;
        public Transform SpawnPoint;
        [HideInInspector] public int TankNumber;
        [HideInInspector] public string ColoredPlayerText;
        [HideInInspector] public GameObject Instance;
        [HideInInspector] public int Wins;

        private TankMovement _movement;
        private TankShooting _shooting;
        private PlayerInput _playerInput;
        private AIController _aiController;
        private GameObject _canvasGameObject;

        public void Setup()
        {
            _movement = Instance.GetComponent<TankMovement>();
            _shooting = Instance.GetComponent<TankShooting>();
            _playerInput = Instance.GetComponent<PlayerInput>();
            _aiController = Instance.GetComponent<AIController>();
            _canvasGameObject = Instance.GetComponentInChildren<Canvas>().gameObject;

            if(_playerInput)
                _playerInput.PlayerNumber = TankNumber;
            if(_aiController)
                _aiController.Target = Object.FindObjectOfType<PlayerInput>().transform;

            if(_playerInput)
                ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(TankColor) + ">PLAYER " + TankNumber + "</color>";
            else if(_aiController)
            {
                ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(TankColor) + ">AI</color>";
            }

            MeshRenderer[] renderers = Instance.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = TankColor;
            }
        }

        public void DisableControl()
        {
            _movement.enabled = false;
            _shooting.enabled = false;
            if (_playerInput)
                _playerInput.enabled = false;
            if (_aiController)
                _aiController.enabled = false;

            _canvasGameObject.SetActive(false);
        }

        public void EnableControl()
        {
            _movement.enabled = true;
            _shooting.enabled = true;
            if(_playerInput)
                _playerInput.enabled = true;
            if(_aiController)
                _aiController.enabled = true;
            
            _canvasGameObject.SetActive(true);
        }

        public void Reset()
        {
            Instance.transform.position = SpawnPoint.position;
            Instance.transform.rotation = SpawnPoint.rotation;

            Instance.SetActive(false);
            Instance.SetActive(true);
        }
    }
}
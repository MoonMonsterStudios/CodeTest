using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoonMonster.Codetest
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _numRoundsToWin = 3;
        [SerializeField] private float _startDelay = 1;
        [SerializeField] private float _endDelay = 3;
        [SerializeField, Required] private CameraControl _cameraControl;
        [SerializeField, Required] private Text _messageText;
        [SerializeField, Required] private GameObject _playerTankPrefab;
        [SerializeField, Required] private GameObject _aiTankPrefab;
        [SerializeField] private TankManager[] _playerTanks;
        [SerializeField] private TankManager[] _aiTanks;
        
        private int _roundNumber;        
        private WaitForSeconds _startWait;
        private WaitForSeconds _endWait;
        private TankManager _roundWinner;
        private TankManager _gameWinner;

        private void Start()
        {
            _startWait = new WaitForSeconds(_startDelay);
            _endWait = new WaitForSeconds(_endDelay);

            SpawnPlayerTanks();
            SpawnAITanks();
            SetCameraTargets();

            StartCoroutine(GameLoop());
        }

        private void SpawnPlayerTanks()
        {
            for (int i = 0; i < _playerTanks.Length; i++)
            {
                _playerTanks[i].Instance =
                    Instantiate(_playerTankPrefab, _playerTanks[i].SpawnPoint.position, _playerTanks[i].SpawnPoint.rotation);
                _playerTanks[i].TankNumber = i + 1;
                _playerTanks[i].Setup();
            }
        }
        
        private void SpawnAITanks()
        {
            for (int i = 0; i < _aiTanks.Length; i++)
            {
                _aiTanks[i].Instance =
                    Instantiate(_aiTankPrefab, _aiTanks[i].SpawnPoint.position, _aiTanks[i].SpawnPoint.rotation);
                _aiTanks[i].TankNumber = -1;
                _aiTanks[i].Setup();
            }
        }

        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[_playerTanks.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = _playerTanks[i].Instance.transform;
            }

            _cameraControl.Targets = targets;
        }

        private IEnumerator GameLoop()
        {
           yield return StartCoroutine(RoundStarting());
           
           yield return StartCoroutine(RoundPlaying());
           
           yield return StartCoroutine(RoundEnding());
           
           if (_gameWinner != null)
           {
               SceneManager.LoadScene(0);
           }
           else
           {
               StartCoroutine(GameLoop());
           }
        }

        private IEnumerator RoundStarting()
        {
            ResetAllTanks();
            DisableTankControl();

            _cameraControl.SetStartPositionAndSize();

            _roundNumber++;
            _messageText.text = "ROUND " + _roundNumber;

            yield return _startWait;
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl();

            _messageText.text = string.Empty;

            while (!OneTankLeft() && !AllPlayersDied())
            {
                yield return null;
            }
        }

        private IEnumerator RoundEnding()
        {
            DisableTankControl();

            _roundWinner = null;

            _roundWinner = GetRoundWinner();
            
            if (_roundWinner == null)
            {
                foreach (var tank in _aiTanks)
                {
                    tank.Wins++;
                    _roundWinner = tank;
                }
            }
            else
                _roundWinner.Wins++;

            _gameWinner = GetGameWinner();

            string message = EndMessage();
            _messageText.text = message;

            yield return _endWait;
        }

        private bool OneTankLeft()
        {
            int numTanksLeft = 0;

            for (int i = 0; i < _playerTanks.Length; i++)
            {
                if (_playerTanks[i].Instance.activeSelf)
                    numTanksLeft++;
            }
            
            for (int i = 0; i < _aiTanks.Length; i++)
            {
                if (_aiTanks[i].Instance.activeSelf)
                    numTanksLeft++;
            }

            return numTanksLeft <= 1;
        }
        
        private bool AllPlayersDied()
        {
            int numTanksLeft = 0;

            for (int i = 0; i < _playerTanks.Length; i++)
            {
                if (_playerTanks[i].Instance.activeSelf)
                    numTanksLeft++;
            }

            return numTanksLeft <= 0;
        }

        private TankManager GetRoundWinner()
        {
            for (int i = 0; i < _playerTanks.Length; i++)
            {
                if (_playerTanks[i].Instance.activeSelf)
                    return _playerTanks[i];
            }
            
            return null;
        }

        private TankManager GetGameWinner()
        {
            for (int i = 0; i < _playerTanks.Length; i++)
            {
                if (_playerTanks[i].Wins == _numRoundsToWin)
                    return _playerTanks[i];
            }

            for (int i = 0; i < _aiTanks.Length; i++)
            {
                if (_aiTanks[i].Wins == _numRoundsToWin)
                    return _aiTanks[i];
            }
            
            return null;
        }

        private string EndMessage()
        {
            string message = "DRAW!";

            if (_roundWinner != null)
                message = _roundWinner.ColoredPlayerText + " WINS THE ROUND!";

            message += "\n\n\n\n";

            for (int i = 0; i < _playerTanks.Length; i++)
            {
                message += _playerTanks[i].ColoredPlayerText + ": " + _playerTanks[i].Wins + " WINS\n";
            }
            if(_aiTanks.Length > 0)
                message += _aiTanks[0].ColoredPlayerText + ": " + _aiTanks[0].Wins + " WINS\n";

            if (_gameWinner != null)
                message = _gameWinner.ColoredPlayerText + " WINS THE GAME!";

            return message;
        }

        private void ResetAllTanks()
        {
            for (int i = 0; i < _playerTanks.Length; i++)
            {
                _playerTanks[i].Reset();
            }
            
            for (int i = 0; i < _aiTanks.Length; i++)
            {
                _aiTanks[i].Reset();
            }
        }

        private void EnableTankControl()
        {
            for (int i = 0; i < _playerTanks.Length; i++)
            {
                _playerTanks[i].EnableControl();
            }
            
            for (int i = 0; i < _aiTanks.Length; i++)
            {
                _aiTanks[i].EnableControl();
            }
        }

        private void DisableTankControl()
        {
            for (int i = 0; i < _playerTanks.Length; i++)
            {
                _playerTanks[i].DisableControl();
            }
            
            for (int i = 0; i < _aiTanks.Length; i++)
            {
                _aiTanks[i].DisableControl();
            }
        }
    }
}
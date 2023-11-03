using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoonMonster.Codetest
{
    public class GameManager : MonoBehaviour
    {
        public int NumRoundsToWin = 5;
        public float StartDelay = 3f;
        public float EndDelay = 3f;
        public CameraControl CameraControl;
        public Text MessageText;
        public GameObject PlayerTankPrefab;
        public GameObject AITankPrefab;
        public TankManager[] PlayerTanks;
        public TankManager[] AITanks;
        
        private int _roundNumber;
        
        private WaitForSeconds _startWait;
        private WaitForSeconds _endWait;
        private TankManager _roundWinner;
        private TankManager _gameWinner;

        private void Start()
        {
            _startWait = new WaitForSeconds(StartDelay);
            _endWait = new WaitForSeconds(EndDelay);

            SpawnPlayerTanks();
            SpawnAITanks();
            SetCameraTargets();

            StartCoroutine(GameLoop());
        }

        private void SpawnPlayerTanks()
        {
            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                PlayerTanks[i].Instance =
                    Instantiate(PlayerTankPrefab, PlayerTanks[i].SpawnPoint.position, PlayerTanks[i].SpawnPoint.rotation) as
                        GameObject;
                PlayerTanks[i].TankNumber = i + 1;
                PlayerTanks[i].Setup();
            }
        }
        
        private void SpawnAITanks()
        {
            for (int i = 0; i < AITanks.Length; i++)
            {
                AITanks[i].Instance =
                    Instantiate(AITankPrefab, AITanks[i].SpawnPoint.position, AITanks[i].SpawnPoint.rotation) as
                        GameObject;
                AITanks[i].TankNumber = -1;
                AITanks[i].Setup();
            }
        }

        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[PlayerTanks.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = PlayerTanks[i].Instance.transform;
            }

            CameraControl.Targets = targets;
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

            CameraControl.SetStartPositionAndSize();

            _roundNumber++;
            MessageText.text = "ROUND " + _roundNumber;

            yield return _startWait;
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl();

            MessageText.text = string.Empty;

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
                foreach (var tank in AITanks)
                {
                    tank.Wins++;
                    _roundWinner = tank;
                }
            }
            else
                _roundWinner.Wins++;

            _gameWinner = GetGameWinner();

            string message = EndMessage();
            MessageText.text = message;

            yield return _endWait;
        }

        private bool OneTankLeft()
        {
            int numTanksLeft = 0;

            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                if (PlayerTanks[i].Instance.activeSelf)
                    numTanksLeft++;
            }
            
            for (int i = 0; i < AITanks.Length; i++)
            {
                if (AITanks[i].Instance.activeSelf)
                    numTanksLeft++;
            }

            return numTanksLeft <= 1;
        }
        
        private bool AllPlayersDied()
        {
            int numTanksLeft = 0;

            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                if (PlayerTanks[i].Instance.activeSelf)
                    numTanksLeft++;
            }

            return numTanksLeft <= 0;
        }

        private TankManager GetRoundWinner()
        {
            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                if (PlayerTanks[i].Instance.activeSelf)
                    return PlayerTanks[i];
            }
            
            return null;
        }

        private TankManager GetGameWinner()
        {
            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                if (PlayerTanks[i].Wins == NumRoundsToWin)
                    return PlayerTanks[i];
            }

            for (int i = 0; i < AITanks.Length; i++)
            {
                if (AITanks[i].Wins == NumRoundsToWin)
                    return AITanks[i];
            }
            
            return null;
        }

        private string EndMessage()
        {
            string message = "DRAW!";

            if (_roundWinner != null)
                message = _roundWinner.ColoredPlayerText + " WINS THE ROUND!";

            message += "\n\n\n\n";

            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                message += PlayerTanks[i].ColoredPlayerText + ": " + PlayerTanks[i].Wins + " WINS\n";
            }
            if(AITanks.Length > 0)
                message += AITanks[0].ColoredPlayerText + ": " + AITanks[0].Wins + " WINS\n";

            if (_gameWinner != null)
                message = _gameWinner.ColoredPlayerText + " WINS THE GAME!";

            return message;
        }

        private void ResetAllTanks()
        {
            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                PlayerTanks[i].Reset();
            }
            
            for (int i = 0; i < AITanks.Length; i++)
            {
                AITanks[i].Reset();
            }
        }

        private void EnableTankControl()
        {
            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                PlayerTanks[i].EnableControl();
            }
            
            for (int i = 0; i < AITanks.Length; i++)
            {
                AITanks[i].EnableControl();
            }
        }

        private void DisableTankControl()
        {
            for (int i = 0; i < PlayerTanks.Length; i++)
            {
                PlayerTanks[i].DisableControl();
            }
            
            for (int i = 0; i < AITanks.Length; i++)
            {
                AITanks[i].DisableControl();
            }
        }
    }
}
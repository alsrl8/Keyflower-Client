using System;
using System.Collections;
using System.Collections.Generic;
using Piece;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string UserID { get; set; }

    public Queue<Action> Actions;
    public GameObject chestObject;
    private Animator _startAnimator;
    public GameObject turnIcon;
    private Image _turnIconImage;
    public Sprite[] turnImages;
    private int _currentTurn;
    private int _myTurn;
    private static readonly int StartTrigger = Animator.StringToHash("Start");

    // Singleton pattern
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _startAnimator = chestObject.GetComponent<Animator>();
            _turnIconImage = turnIcon.GetComponent<Image>();
            DontDestroyOnLoad(gameObject); // Retain the object when switching scenes
        }
        else
        {
            Destroy(gameObject); // Destroy additional instance
        }
    }

    private void Start()
    {
        Actions = new Queue<Action>();
    }

    private void Update()
    {
        while (Actions.Count > 0)
        {
            Actions.Dequeue().Invoke();
        }
    }

    public void GameStart(int myTurn)
    {
        Debug.Log("Game Start!");
        chestObject.SetActive(true);
        turnIcon.SetActive(true);
        _startAnimator.SetTrigger(StartTrigger);
        _myTurn = myTurn;
        RoundManager.Instance.StartSpring();
    }

    private static IEnumerator ActivatePlayButton()
    {
        yield return new WaitForSeconds(2f);
        PlayButton.Instance.SetButtonEnable(true);
    }

    private static void InactivatePlayButton()
    {
        PlayButton.Instance.SetButtonEnable(false);
    }

    public void SetCurrentTurn(int turn)
    {
        _currentTurn = turn;
        if (IsMyTurn())
        {
            StartCoroutine(ActivatePlayButton());
        }
        else
        {
            InactivatePlayButton();
        }

        if (turnImages.Length < turn) return;
        _turnIconImage.sprite = turnImages[turn];
    }

    public bool IsMyTurn()
    {
        return _currentTurn == _myTurn && _currentTurn > 0;
    }

    public void BindMeepleAndTile(string meepleID, string tileID)
    {
        MeepleManager.Instance.BindToTile(meepleID, tileID);
        TileManager.Instance.BindMeepleToTile(tileID, meepleID);
    }

    public void BindMeepleAndActiveTile(string meepleID)
    {
        var tileID = TileManager.Instance.GetActiveTileID();
        BindMeepleAndTile(meepleID, tileID);
    }

    public void UnbindMeepleFromTile(string meepleID, string tileID)
    {
        MeepleManager.Instance.UnbindMeeple(meepleID);
        TileManager.Instance.UnbindMeepleFromTile(tileID, meepleID);
    }

    public void HandlePlayButton()
    {
        // TODO 플레이 버튼을 눌렀을 때 
        // Meeple Action을 확인하여 유효한 Meeple Action이라면
        // 포함된 Meeple들을 그룹화하고 다음 턴으로 넘기는 작업까지 구현
    }
}
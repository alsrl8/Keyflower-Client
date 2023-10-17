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
    private Animator _startAnimator;
    public GameObject turnIcon;
    private Image _turnIconImage;
    public Sprite[] turnImages;
    private int _currentTurn;
    private int _myTurn;

    // Singleton pattern
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Actions = new Queue<Action>();
            _turnIconImage = turnIcon.GetComponent<Image>();
            DontDestroyOnLoad(gameObject); // Retain the object when switching scenes
        }
        else
        {
            Destroy(gameObject); // Destroy additional instance
        }
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
        turnIcon.SetActive(true);
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
    
    public void BidOtherFirstMeepleToTile(string playerID, string meepleID, string tileID)
    {
        MeepleManager.Instance.PutOnTile(meepleID, tileID);
        TileManager.Instance.BidFirstOtherMeepleOnTile(playerID, meepleID, tileID);
    }

    public void BidOtherMoreMeepleToTile(string playerID, string tileID, int bidNum)
    {
        TileManager.Instance.BidMoreOtherMeepleOnTile(playerID, tileID, bidNum);
    }

    public void BidFirstMeepleToTile(string meepleID, string tileID)
    {
        MeepleManager.Instance.PutOnTile(meepleID, tileID);
        TileManager.Instance.BidFirstMeepleOnTile(meepleID, tileID);
    }

    public void BidFirstMeepleToActiveTile(string meepleID)
    {
        var tileID = TileManager.Instance.GetTriggeredTileID();
        BidFirstMeepleToTile(meepleID, tileID);
    }

    public void UnBidMeepleFromTile(string meepleID, string tileID)
    {
        MeepleManager.Instance.ReleaseFromTile(meepleID);
        TileManager.Instance.UnBidFromTile(tileID);
    }

    public void HandlePlayButton()
    {
        MeepleActionManager.Instance.SendMeepleActions();
    }
}
using System.Collections;
using Piece;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoundManager : MonoBehaviour
    {
        public static RoundManager Instance { get; private set; }
        public string CurrentSeason { get; private set; }
        public GameObject roundImageCenter;
        private Image _roundImageCenter;
        public GameObject roundLogo;
        private Image _roundLogo;
        public Sprite springWordImage;
        public Sprite summerWordImage;
        public Sprite autumnWordImage;
        public Sprite winterWordImage;
        public Sprite springImage;
        public Sprite summerImage;
        public Sprite autumnImage;
        public Sprite winterImage;
        private Animator _wordAnimator;
        private Animator _logoAnimator;
        private static readonly int Start = Animator.StringToHash("Start");

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _roundImageCenter = roundImageCenter.GetComponent<Image>();
                _wordAnimator = roundImageCenter.GetComponent<Animator>();
                _roundLogo = roundLogo.GetComponent<Image>();
                _logoAnimator = roundLogo.GetComponent<Animator>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartSpring()
        {
            CurrentSeason = "Spring";
            ShowRoundWordImage(springWordImage);
        }

        public void StartSummer()
        {
            CurrentSeason = "Summer";
            ShowRoundWordImage(summerWordImage);
            
            TileManager.Instance.SetInactiveSeasonTiles("Spring");
        }

        public void StartAutumn()
        {
            CurrentSeason = "Autumn";
            ShowRoundWordImage(autumnWordImage);
            
            TileManager.Instance.SetInactiveSeasonTiles("Summer");
        }

        public void StartWinter()
        {
            CurrentSeason = "Winter";
            ShowRoundWordImage(winterWordImage);
            
            TileManager.Instance.SetInactiveSeasonTiles("Autumn");
        }

        public void InactiveRoundImage()
        {
            StartCoroutine(_InactiveRoundImage());
        }

        private void ShowRoundWordImage(Sprite wordImage)
        {
            roundImageCenter.SetActive(true);
            _roundImageCenter.sprite = wordImage;
            _wordAnimator.SetTrigger(Start);
        }

        public void ShowRoundLogo(Sprite logo)
        {
            StartCoroutine(ShowRoundLogWithInterval(logo));
        }

        private IEnumerator ShowRoundLogWithInterval(Sprite logo)
        {
            yield return new WaitForSeconds(0.5f);

            roundLogo.SetActive(true);
            _roundLogo.sprite = logo;
            _logoAnimator.SetTrigger(Start);
        }

        private IEnumerator _InactiveRoundImage()
        {
            yield return new WaitForSeconds(1f);
            roundImageCenter.SetActive(false);
        }
    }
}
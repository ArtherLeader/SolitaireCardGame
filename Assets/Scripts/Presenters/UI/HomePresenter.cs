using DG.Tweening;
using Solitaire.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Solitaire.Presenters
{
    public class HomePresenter : OrientationAwarePresenter
    {
        [SerializeField] Button _buttonNewMatch;
        [SerializeField] Button _buttonContinue;
        [SerializeField] Button _buttonOptions;
        [SerializeField] RectTransform _rectCards;
        [SerializeField] RectTransform _rectSuitsCenter;
        [SerializeField] RectTransform _rectSuitsLeft;
        [SerializeField] RectTransform _rectSuitsRight;

        [Inject] Game _game;
        [Inject] GamePopup _gamePopup;
        [Inject] GameState _gameState;

        protected override void Start()
        {
            base.Start();

            _game.NewMatchCommand.BindTo(_buttonNewMatch).AddTo(this);
            _game.ContinueCommand.BindTo(_buttonContinue).AddTo(this);
            _gamePopup.OptionsCommand.BindTo(_buttonOptions).AddTo(this);

            // Play animation sequence on state change
            _gameState.State.Where(state => state == Game.State.Home).Subscribe(_ => 
                PlayAnimationSequence(_orientation.State.Value == Orientation.Landscape)).AddTo(this);
        }

        protected override void OnOrientationChanged(bool isLandscape)
        {
            _rectCards.anchoredPosition = isLandscape ? new Vector2(0, -120) : Vector2.zero;

            PlayAnimationSequence(isLandscape);
        }

        private void PlayAnimationSequence(bool isLandscape)
        {
            AnimateCards();

            if (isLandscape)
            {
                AnimateSuits(_rectSuitsLeft, false);
                AnimateSuits(_rectSuitsRight, true);
            }
            else
            {
                AnimateSuits(_rectSuitsCenter, false);
            }
        }

        private void AnimateCards()
        {
            Sequence sequence = DOTween.Sequence();

            for (int i = 1; i < _rectCards.childCount; i++)
            {
                Transform rect = _rectCards.GetChild(i);
                rect.localEulerAngles = new Vector3(0f, 0f, 37.5f);

                Tweener tween = rect.DOLocalRotate(new Vector3(0f, 0f, -i * 25f), i * 0.3333f,
                    RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

                sequence.Insert(0, tween);
            }
        }

        private void AnimateSuits(RectTransform rectSuits, bool isReverse)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(1f);

            for (int i = isReverse ? rectSuits.childCount - 1 : 0;
                isReverse ? i >= 0 : i < rectSuits.childCount;
                i += isReverse ? -1 : 1)
            {
                Transform rect = rectSuits.GetChild(i);
                rect.transform.localScale = Vector3.zero;

                sequence.Append(rect.DOScale(Vector3.one, 0.125f).SetEase(Ease.InCubic))
                    .Append(rect.DOPunchScale(Vector3.one * 0.5f, 0.125f).SetEase(Ease.OutCubic));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using FSM.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Roulette
{
    public class RoulettePopup : MonoBehaviour, IRouletteView
    {
        private ILoggerService _logger;
        private Camera _mainCamera;

        [Header("Spin Animation")] 
        [SerializeField] private int slotCount = 12;

        [SerializeField] private float rotationLoops = 5;
        [SerializeField] private float spinDuration = 5f;

        [Header("Reward Animation")] 
        [SerializeField] private int rewardPoolSize = 20;

        [SerializeField] private float defaultScale = 0.7f;
        [SerializeField] private float appearDuration = 0.7f;
        [SerializeField] private float moveDuration = 0.9f;
        [SerializeField] private int minRadius = 1;
        [SerializeField] private int maxRadius = 2;
        [SerializeField] private float minStayDuration = 1f;
        [SerializeField] private float maxStayDuration = 2.5f;
        [SerializeField] private float postRewardDelay = 2f;

        [Header("Ui")] 
        [SerializeField] private Button spinBtn;

        [SerializeField] private TextMeshProUGUI spinButtonText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite disabledSprite;
        [SerializeField] private Transform rewardTextsContainer;
        [SerializeField] private TextMeshProUGUI rewardCounter;
        private TextMeshProUGUI[] _rewardTexts;
        [SerializeField] private Transform wheel;
        [SerializeField] private Transform rewardAnimContainer;
        private RectTransform _rewardAnimContainerRect;

        [Header("Reward Icons")] [SerializeField]
        private List<RewardTypeGameObject> rewardIconMappings;

        private Dictionary<RewardType, GameObject> _iconDictionary;
        private Dictionary<RewardType, ObjectPool<RewardGO>> _pools;

        [Serializable]
        private class RewardTypeGameObject
        {
            public RewardType Type;
            public GameObject IconObject;
        }

        private class RewardGO
        {
            public GameObject GameObject { get; }
            public RectTransform RectTransform { get; }

            public RewardGO(GameObject go)
            {
                GameObject = go;
                RectTransform = go.GetComponent<RectTransform>();
            }
        }

        public event Action OnSpinButtonClicked;
        public event Action OnDestroyed;

        #region Unity lifecycle

        private void Awake()
        {
            GetComponent<RouletteSlotLabelGenerator>().AlignText();

            _logger = ServiceLocator.Get<ILoggerService>();

            _mainCamera = Camera.main;

            _rewardAnimContainerRect = rewardAnimContainer.GetComponent<RectTransform>();

            _rewardTexts = rewardTextsContainer.GetComponentsInChildren<TextMeshProUGUI>();

            if (_rewardTexts.Length != slotCount)
            {
                _logger?.LogError(
                    $"[Roulette] Error: Expected 12 reward text objects, but found {_rewardTexts.Length}.");
            }

            InitializePools();
        }
        
        private void OnEnable()
        {
            spinBtn.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            spinBtn.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            OnSpinButtonClicked?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            spinBtn.onClick.RemoveAllListeners();
        }

        #endregion

        public void SetRewards(RouletteReward[] rewards)
        {
            if (rewards == null || rewards.Length == 0)
            {
                _logger.LogError($"[Roulette] Rewards array is null or empty. Cannot set rewards.");
                return;
            }

            for (int i = 0; i < rewards.Length; i++)
            {
                if (i < _rewardTexts.Length)
                {
                    _rewardTexts[i].text = rewards[i].Value.ToString();
                }
            }

            ShowRewardIcon(rewards[0].Type);
        }

        public void SetButtonInteractable(bool interactable)
        {
            spinBtn.interactable = interactable;
            spinBtn.image.sprite = interactable ? activeSprite : disabledSprite;
        }

        public void ShowActiveText()
        {
            spinButtonText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(false);
        }

        public void ShowTimerText(string timerValue)
        {
            spinButtonText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(true);
            timerText.SetText(timerValue);
        }

        #region Animation

        public void PlaySpinAnimation(RouletteReward reward, Action onComplete = null)
        {
            float anglePerSlot = 360f / slotCount;
            float offsetAngle = anglePerSlot / 2f;

            float targetAngle = (reward.SlotIndex * anglePerSlot) + offsetAngle;

            float finalRotation = (rotationLoops * 360f) + targetAngle;

            wheel.DORotate(new Vector3(0, 0, finalRotation), spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => PlayRewardAnimation(reward, onComplete));
        }

        public void PlayRewardAnimation(RouletteReward reward, Action onComplete = null)
        {
            ShowRewardCounter();

            var rewards = PrepareRewardVisuals(reward.Value, rewardPoolSize);
            var slotTransform = _rewardTexts[reward.SlotIndex].transform;

            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_mainCamera, slotTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rewardAnimContainerRect,
                screenPos,
                _mainCamera,
                out var localPos
            );

            int finishedCount = 0;
            int currentCount = 0;

            for (int i = 0; i < rewards.Count; i++)
            {
                int rewardValue = rewards[i];
                
                var rewardObj = _pools[reward.Type].Get();

                var rect = rewardObj.RectTransform;

                float randomRadius = Random.Range(minRadius, maxRadius);
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                Vector2 offset = randomDir * randomRadius;

                rect.anchoredPosition = localPos + offset;
                rect.localScale = Vector3.zero;

                float stayTime = Random.Range(minStayDuration, maxStayDuration);

                var seq = DOTween.Sequence();
                seq.SetTarget(rect);
                seq.Append(rect.DOScale(defaultScale, appearDuration).SetEase(Ease.InQuad));
                seq.AppendInterval(stayTime);
                seq.Append(rect.DOAnchorPos(Vector2.zero, moveDuration).SetEase(Ease.InQuad));
                seq.Join(rect.DOScale(0f, moveDuration).SetEase(Ease.InQuad));
                seq.OnComplete(() =>
                {
                    currentCount += rewardValue;
                    rewardCounter.SetText("{0}", currentCount);
                    _pools[reward.Type].Release(rewardObj);

                    finishedCount++;
                    if (finishedCount == rewards.Count)
                    {
                        DOVirtual.DelayedCall(postRewardDelay, () => onComplete?.Invoke());
                    }
                });
            }
        }

        private List<int> PrepareRewardVisuals(int totalReward, int maxObjsPerAnimation)
        {
            int numberOfObjs = Mathf.Min(totalReward, maxObjsPerAnimation);
            int valuePerObject = totalReward / numberOfObjs;
            int remainder = totalReward % numberOfObjs;

            var rewards = new List<int>(numberOfObjs);
            for (int i = 0; i < numberOfObjs; i++)
            {
                int value = valuePerObject + (i < remainder ? 1 : 0);
                rewards.Add(value);
            }

            return rewards;
        }
        
        #endregion
        
        private void SetAllIconsActive(bool active)
        {
            foreach (var pair in _iconDictionary)
            {
                pair.Value.SetActive(active);
            }
        }

        private void ShowRewardIcon(RewardType type)
        {
            rewardCounter.gameObject.SetActive(false);
            SetAllIconsActive(false);

            if (_iconDictionary.ContainsKey(type))
            {
                _iconDictionary[type].SetActive(true);
            }
        }

        private void ShowRewardCounter()
        {
            rewardCounter.gameObject.SetActive(true);
            rewardCounter.text = 0.ToString();
            SetAllIconsActive(false);
        }

        #region ObjectPool
        private void InitializePools()
        {
            _iconDictionary = new Dictionary<RewardType, GameObject>();
            _pools = new Dictionary<RewardType, ObjectPool<RewardGO>>();

            foreach (var mapping in rewardIconMappings)
            {
                var prefab = mapping.IconObject;
                var type = mapping.Type;

                _iconDictionary[type] = prefab;

                var pool = new ObjectPool<RewardGO>(
                    createFunc: () =>
                    {
                        var go = Instantiate(prefab, rewardAnimContainer);
                        go.SetActive(false);
                        return new RewardGO(go);
                    },
                    actionOnGet: rewardGO => rewardGO.GameObject.SetActive(true),
                    actionOnRelease: rewardGO => rewardGO.GameObject.SetActive(false),
                    actionOnDestroy: rewardGO => Destroy(rewardGO.GameObject),
                    collectionCheck: false,
                    defaultCapacity: rewardPoolSize,
                    maxSize: rewardPoolSize
                );
                
                for (int i = 0; i < rewardPoolSize; i++)
                {
                    var obj = pool.Get();     
                    pool.Release(obj);        
                }
                
                _pools[type] = pool;
            }
        }
        #endregion

    }
}
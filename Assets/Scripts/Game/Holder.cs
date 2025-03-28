using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using WaterSort;

namespace WaterSort
{
    public class Holder : MonoBehaviour
    {
        private float _heightTube = 4.3f;
        private float _unitSizeWater;

        [SerializeField] private float _speed = 2;


        [SerializeField] private Liquid _liquidPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private Transform _leftSideDeliverPoint;
        [SerializeField] private Transform _rightSideDeliverPoint;

        [SerializeField] private Vector2 _transferNearOffset;
        [SerializeField] private SpriteRenderer _liquidLine;
        [SerializeField] private SpriteRenderer skinRenderer;
        [SerializeField] private SpriteMask mask;

        private Transform parentRoot;
        public Transform _Transform { private set; get; }
        public int waterSortingOrderID { private set; get; }
        public int skinSortingOrderID { private set; get; }


        private readonly List<Liquid> _liquids = new List<Liquid>();
        private Coroutine _moveCoroutine;
        private bool _isFront;
        private bool _isFinish;
        public bool IsTransfer { set; get; }
        public bool IsReceiving => ListIDBottlePouring.Count > 0;
        public int ID { private set; get; }
        private bool isChangeSkin;
        public bool IsFull => _liquids.Sum(l => l.ValueReal) >= MAXValue;
        public bool IsFourFull => _liquids.Sum(l => l.ValueTarget) >= MAXValue;
        public Liquid TopLiquid => _liquids.LastOrDefault();
        public Liquid BotLiquid => _liquids.FirstOrDefault();
        public IEnumerable<Liquid> Liquids => _liquids;

        public int MAXValue { private set; get; }

        public int CurrentTotal => Liquids.Sum(l => l.ValueReal);
        public int CurrentTotalTarget => Liquids.Sum(l => l.ValueTarget);
        public bool IsPending { get; private set; }

        public bool Initialized { get; private set; }
        public Vector2 PendingPoint { get; private set; }

        public Vector3 OriginalPoint { get; private set; }

        public List<int> ListIDBottlePouring { get; set; }

        public bool IsLastPour
        {
            get => ListIDBottlePouring.Count <= 1 ? false : true;
        }

        public bool IsFront
        {
            get => _isFront;
            set
            {
                _isFront = value;
                SetPropertiesLiquidWhenCHangeHolderState(_isFront);
            }
        }

        public bool IsFinish => Mathf.RoundToInt(_liquids.Sum(l => l.ValueReal)) >= MAXValue && _liquids.Count == 1;

        private void Awake()
        {
            _unitSizeWater = _heightTube / MAXValue;
            ListIDBottlePouring = new List<int>();
            _Transform = transform;
        }


        public void UpdateColorMode()
        {
            foreach (var liquid in _liquids)
                liquid.UpdateColor();
        }

        public void StartPending()
        {
            if (IsPending)
                throw new InvalidOperationException();
            IsPending = true;
            IsFront = true;
            MoveTo(PendingPoint, speed: 5);
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_BOTTLE_SELECT, .5f);
        }

        public void ClearPending()
        {
            IsPending = false;
            IsFront = false;
            MoveTo(OriginalPoint, speed: 5);
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_BOTTLE_UNSELECT, .35f);
        }

        private IEnumerator MoveNearToHolderForTransfer(Holder holder)
        {
            var targetPoint = holder._Transform.TransformPoint(_Transform.position.x > holder._Transform.position.x
                ? _transferNearOffset.WithX(Mathf.Abs(_transferNearOffset.x))
                : _transferNearOffset.WithX(-Mathf.Abs(_transferNearOffset.x)));

            var speed = GetSpeedForDistance((_Transform.position - targetPoint).magnitude);
            StopMoveIfAlready();
            yield return MoveToEnumerator(targetPoint, Mathf.Max(speed, 3));
        }


        private float GetSpeedForDistance(float distance)
        {
            return 30 / distance;
        }

        private IEnumerator ReturnToOriginalPoint()
        {
            StopMoveIfAlready();
            var speed = GetSpeedForDistance((_Transform.position - OriginalPoint).magnitude);
            yield return MoveToEnumerator(OriginalPoint, Mathf.Max(speed, 3));
        }

        private void StopMoveIfAlready()
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        }

        public IEnumerator MoveAndTransferLiquid(Holder receiveHolder, Action onLiquidTransferComplete = null)
        {
            IsPending = false;
            IsTransfer = true;

            int indexAngle = CurrentTotal - 1;
            float deliverAbsAngle = 0;
            if (MAXValue == 4)
                deliverAbsAngle = GameManager.HolderDataPour.listFour[indexAngle].startAngle;
            else
                deliverAbsAngle = GameManager.HolderDataPour.listFive[indexAngle].startAngle;
            if (receiveHolder.IsFourFull)
            {
                IsFront = false;
                IsTransfer = false;
                yield return ReturnToOriginalPoint();
                yield break;
            }
            if (receiveHolder.IsFull || !_liquids.Any() || receiveHolder.Liquids.Any() && receiveHolder.Liquids.Last().GroupId != Liquids.Last().GroupId)
            {
                yield break;
            }
            LevelManager.Instance.SetUndoStack(this, receiveHolder);
            receiveHolder.ListIDBottlePouring.Add(ID);
            LevelManager.Instance.InvokeActionChangeHolderState();
            ChangeSidePosition();
            yield return MoveNearToHolderForTransfer(receiveHolder);

            ChangeScaleContentWhenPour(true);

            var isRightSide = receiveHolder._Transform.position.x > _Transform.position.x;
            var sidePoint = isRightSide ? _rightSideDeliverPoint : _leftSideDeliverPoint;
            var deliverAngle = isRightSide ? -deliverAbsAngle : deliverAbsAngle;


            float positionContentAngle = GameManager.HolderDataPour.contentPoin;
            _content.transform.localPosition = isRightSide ? new Vector3(positionContentAngle, 0, 0) : new Vector3(-positionContentAngle, 0, 0);

            var targetDeltaStartHolderPoint = Vector3.zero;
            if (MAXValue == 4)
                targetDeltaStartHolderPoint = new Vector3(isRightSide ? -GameManager.HolderDataPour.listFour[indexAngle].deltaStartPoint.x :
                    GameManager.HolderDataPour.listFour[indexAngle].deltaStartPoint.x,
                    GameManager.HolderDataPour.listFour[indexAngle].deltaStartPoint.y,
                    GameManager.HolderDataPour.listFour[indexAngle].deltaStartPoint.z);
            else
                targetDeltaStartHolderPoint = new Vector3(isRightSide ? -GameManager.HolderDataPour.listFive[indexAngle].deltaStartPoint.x :
                    GameManager.HolderDataPour.listFive[indexAngle].deltaStartPoint.x,
                    GameManager.HolderDataPour.listFive[indexAngle].deltaStartPoint.y,
                    GameManager.HolderDataPour.listFive[indexAngle].deltaStartPoint.z);


            var targetHolderPoint = receiveHolder.OriginalPoint + targetDeltaStartHolderPoint;
            var targetHolderRotation = Quaternion.AngleAxis(deliverAngle, Vector3.forward);

            var startPoint = _Transform.position;
            var startRotation = _Transform.rotation;

            var thisLiquid = _liquids.Last();
            if (receiveHolder.Liquids.LastOrDefault() == null)
            {
                receiveHolder.AddLiquid(thisLiquid.GroupId);
            }
            Liquid targetLiquid = receiveHolder.Liquids.Last();

            
            int transferValue = Mathf.Min(thisLiquid.ValueReal, receiveHolder.MAXValue - receiveHolder.CurrentTotalTarget);

            thisLiquid.ValueTarget -= transferValue;
            targetLiquid.ValueTarget += transferValue;
            
            

            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                _Transform.position = Vector3.Lerp(startPoint, targetHolderPoint, n);
                _Transform.rotation = Quaternion.Lerp(startRotation, targetHolderRotation, n);
                SetPropertiesLiquidWhenChangeHolderRotate();
            }, speed: _speed);

           
            Transform parentTempTrans = ParentTempPour.Instance.GetTransform(sidePoint.position, _Transform.rotation);
            _Transform.parent = parentTempTrans;


            //int indexpPour = CurrentTotal - Liquids.Last().ValueReal;
            int indexpPour = CurrentTotal - transferValue;
            float pourAngle = 0;
            if (MAXValue == 4)
                pourAngle = isRightSide ? -GameManager.HolderDataPour.listFour[indexpPour].endAngle : GameManager.HolderDataPour.listFour[indexpPour].endAngle;
            else
                pourAngle = isRightSide ? -GameManager.HolderDataPour.listFive[indexpPour].endAngle : GameManager.HolderDataPour.listFive[indexpPour].endAngle;
            var pourHolderRotation = Quaternion.AngleAxis(pourAngle, Vector3.forward);

            
            int thisLiquidStartValue = thisLiquid.ValueReal;

            int targetLiquidStartValue = targetLiquid.ValueReal;


            _liquidLine.transform.position = sidePoint.position;
            _liquidLine.sortingOrder = (receiveHolder.waterSortingOrderID - 1);
            _liquidLine.gameObject.SetActive(true);
            _liquidLine.size = new Vector2(0.1f, sidePoint.position.y - receiveHolder._Transform.position.y - 0.15f);
            _liquidLine.color = thisLiquid.Renderer.color;
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_WATER, (float)transferValue / 5);
            _liquidLine.transform.rotation = Quaternion.identity;

            thisLiquid.ActiveEffectFour(true, false);
            targetLiquid.ActiveEffectFour(true, true);

            float temptargetLiquidStartValue = targetLiquidStartValue;
            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                //_Transform.position = Vector3.Lerp(targetHolderPoint, targetPourHolderPoint, n);
                parentTempTrans.rotation = Quaternion.Lerp(targetHolderRotation, pourHolderRotation, n);
                SetPropertiesLiquidWhenChangeHolderRotate();

                thisLiquid.ValueRenderer = Mathf.Lerp(thisLiquidStartValue, thisLiquidStartValue - transferValue, n);

                targetLiquid.ValueRenderer += Mathf.Lerp(targetLiquidStartValue, targetLiquidStartValue + transferValue, n) - temptargetLiquidStartValue;
                temptargetLiquidStartValue = Mathf.Lerp(targetLiquidStartValue, targetLiquidStartValue + transferValue, n);

                _liquidLine.size = new Vector2(0.1f, sidePoint.position.y - receiveHolder._Transform.position.y - 0.15f);
                _liquidLine.transform.rotation = Quaternion.identity;

            }, speed: _speed / 2 / (float)transferValue);
            targetLiquid.ValueReal += transferValue;
            thisLiquid.ActiveEffectFour(false, false);

            if (!receiveHolder.IsLastPour)
            {
                float timeDelay = 0.5f;
                targetLiquid.Renderer.gameObject.SetActive(true);
                DOVirtual.DelayedCall(timeDelay, () => { targetLiquid.ActiveEffectFour(false, true); });
                targetLiquid.EffectAffterFour(timeDelay);

            }
            receiveHolder.ListIDBottlePouring.Remove(ID);
            thisLiquid.ValueReal -= transferValue;
            if (thisLiquid.ValueReal == 0)
            {
                RemoveLiquid(thisLiquid);
            }
            receiveHolder.SetStateObjectCover();

            SoundController.Instance.StopSound(AUDIO_KEY.SOUND_WATER);
            _liquidLine.gameObject.SetActive(false); ;
            onLiquidTransferComplete?.Invoke();
            _Transform.parent = parentRoot;
            //Destroy(parentTemp);
            parentTempTrans.gameObject.SetActive(false);
            var targetPourHolderPoint = _Transform.position;
            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                _Transform.position = Vector3.Lerp(targetPourHolderPoint, startPoint, n);
                _Transform.rotation = Quaternion.Lerp(pourHolderRotation, startRotation, n);
                SetPropertiesLiquidWhenChangeHolderRotate();
            }, speed: _speed);

            yield return ReturnToOriginalPoint();
            IsFront = false;
            ChangeScaleContentWhenPour(false);
            IsTransfer = false;

        }


        private void RemoveLiquid(Liquid liquid)
        {
            bool isBottom = liquid.IsBottomLiquid;
            _liquids.Remove(liquid);
            Destroy(liquid.gameObject);
            if (isBottom && _liquids.Count != 0)
            {
                _liquids.First().IsBottomLiquid = true;
            }
        }

        public void ChangeSkin(Sprite spriteSkin, Sprite spriteMask)
        {
            mask.sprite = spriteMask;
            skinRenderer.sprite = spriteSkin;
            isChangeSkin = false;
        }

        public void AddLiquid(int groupId, float value = 0)
        {
            var topPoint = GetTopPoint();
            var liquid = Instantiate(_liquidPrefab, _content);

            liquid.IsBottomLiquid = !Liquids.Any();
            liquid.GroupId = groupId;
            liquid.transform.position = topPoint;
            liquid.ValueRenderer = value;
            liquid.ValueReal = (int)value;
            liquid.ValueTarget = (int)value;
            liquid.SetUnitSize(_unitSizeWater);
            liquid.sortingOrder(waterSortingOrderID);
            int indexLiquid = Liquids.Sum(l => l.ValueReal);
            _liquids.Add(liquid);
        }

        public Vector2 GetTopPoint()
        {
            return transform.TransformPoint(Liquids.Sum(l => l.Size) * Vector2.up);
        }

        public void ChangePosition(Vector2 newPos)
        {
            _Transform.position = newPos;
            OriginalPoint = newPos;
            PendingPoint = newPos + 0.5f * Vector2.up;
        }
        public void Init(int maxWater, IEnumerable<LiquidData> liquidDatas, int ID)
        {
            this.ID = ID;
            this.MAXValue = maxWater;
            this._unitSizeWater = _heightTube / MAXValue;

            var list = liquidDatas.ToList();
            if (Initialized)
                return;

            list.ForEach(l => AddLiquid(l.groupId, l.value));
            PendingPoint = _Transform.position + 0.5f * Vector3.up;
            OriginalPoint = _Transform.position;


            SetPropertiesLiquidWhenCHangeHolderState(false);
            Initialized = true;

        }

        private bool isShowedEffect = false;
        public void SetStateObjectCover(bool activeEff = true)
        {
            if (IsFinish)
            {
                if (!isShowedEffect)
                {
                    isShowedEffect = true;
                    if (activeEff)
                    {
                        DOVirtual.DelayedCall(0.1f, () =>
                        {
                            //Color tempColor = ResourceManager.Instance._normalColors[_liquids[0].GroupId];
                            //fullBottleEffect.SorttingLayerId(waterSortingOrderID - 1, tempColor);
                            //fullBottleEffect.SetActive(true);
                            EffectFullBottleManager.Instance.CreateEffectFull(transform.position);
                        });
                        SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_BOTTLE_COMPLETE);
                    }

                }
            }
        }
        public void UndoLiquid(List<(int GroupId, float Value)> liquids)
        {
            for (int count = 0; count < _liquids.Count; count++)
            {
                Destroy(_liquids[count].gameObject);
            }
            _liquids.Clear();

            for (int count = 0; count < liquids.Count; count++)
            {
                AddLiquid(liquids[count].GroupId, liquids[count].Value);
            }
            SetPropertiesLiquidWhenCHangeHolderState(_isFront);

        }
        public void MoveTo(Vector2 point, float speed = 1, Action onFinished = null)
        {
            StopMoveIfAlready();

            _moveCoroutine = StartCoroutine(SimpleCoroutine.CoroutineEnumerator(MoveToEnumerator(point, speed), onFinished));
        }
        private IEnumerator MoveToEnumerator(Vector2 toPoint, float speed = 1)
        {
            var startPoint = _Transform.position;
            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                _Transform.position = Vector3.Lerp(startPoint, toPoint, n);
            }, speed: speed);
        }

        public void SwapOut(float time = 1.0f)
        {
            /*
            float xPosTarget = _Transform.position.x + GameConfig.WidthScreen();
            _Transform.DOMoveX(xPosTarget, time).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(gameObject);
            });
            */
            Destroy(gameObject);
        }

        public void SwapIn()
        {
            float xPosTarget = _Transform.position.x;
            _Transform.position = _Transform.position - new Vector3(GameConfig.WidthScreen(), 0, 0);
            _Transform.DOMoveX(xPosTarget, 1.0f).SetEase(Ease.OutBack);
        }


        private void ChangeSidePosition()
        {
            if (isChangeSkin) return;
            isChangeSkin = true;
            _leftSideDeliverPoint.localPosition = GameManager.HolderDataPour.lineLeftPoin;
            _rightSideDeliverPoint.localPosition = GameManager.HolderDataPour.lineRightPoin;
        }

        private void SetPropertiesLiquidWhenChangeHolderRotate()
        {
            float num = _Transform.rotation.eulerAngles.z;
            num = num < 0 ? -num : num;
            num = num > 180 ? 360 - num : num;
            _content.transform.eulerAngles = Vector3.zero;
            _content.transform.localScale = new Vector3(5f, 1f - (num) / 180f, 1f);
        }
        private void SetPropertiesLiquidWhenCHangeHolderState(bool isSelected)
        {
            /*
             * 1 chai có 7 layer là back(0), line(1), water(2),surface(3), alpha_surface(4), skin(5) , item_bottom(6)
             */
            waterSortingOrderID = ID * 7 + 2;
            int anphaSurfaceSortingOrderID = ID * 7 + 4;
            skinSortingOrderID = ID * 7 + 5;



            foreach (var liquid in _liquids)
            {
                liquid.sortingLayerName(isSelected ? "Front" : "Default");
                liquid.sortingOrder(waterSortingOrderID);
            }

            mask.frontSortingLayerID = isSelected ? SortingLayer.NameToID("Front") : SortingLayer.NameToID("Default");
            mask.frontSortingOrder = anphaSurfaceSortingOrderID;
            mask.backSortingLayerID = isSelected ? SortingLayer.NameToID("Front") : SortingLayer.NameToID("Default");
            mask.backSortingOrder = waterSortingOrderID - 1;

            skinRenderer.sortingLayerName = isSelected ? "Front" : "Default";
            skinRenderer.sortingOrder = skinSortingOrderID;


        }

        private void ChangeScaleContentWhenPour(bool isSelected)
        {
            _content.localScale = isSelected ? new Vector3(5, 1, 1) : Vector3.one;
            _content.transform.localPosition = isSelected ? new Vector3(0.5f, 0, 0) : Vector3.zero;
            _content.transform.localRotation = Quaternion.identity;
        }

        public void SetParentRoot(Transform parent)
        {
            parentRoot = parent;
            _Transform.parent = parent;
            _Transform.localScale = Vector3.one;
        }
    }
}
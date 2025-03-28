using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WaterSort;

namespace LevelEditor
{
    public class HolderEditor : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _liquidLine;
        [SerializeField] Liquid _liquidPrefab;
        [SerializeField] Transform _content;
        [SerializeField] private Vector2 _transferNearOffset;
        [SerializeField] private Transform _leftSideDeliverPoint;
        [SerializeField] private Transform _rightSideDeliverPoint;
        private readonly List<Liquid> _liquids = new List<Liquid>();
        public IEnumerable<Liquid> Liquids => _liquids;

        private List<LiquidData> _listLiquidData = new List<LiquidData>();
        public IEnumerable<LiquidData> listLiquidData => _listLiquidData;

        [SerializeField] private int _maxValue = 4;


        private const float heightTube = 3.8f;
        private float unitSizeWater;

        public bool IsFull => Mathf.RoundToInt(_liquids.Sum(l => l.ValueRenderer)) >= _maxValue;

        private Coroutine _moveCoroutine;
        public bool IsPending { get; private set; }
        private bool _isFront;

        public bool Initialized { get; private set; }

        public Vector2 PendingPoint
        {
            get;
            private set;
        }

        public Vector3 OriginalPoint { get; private set; }

        private void Awake()
        {
            unitSizeWater = heightTube / _maxValue;
        }
        private void Start()
        {
            if (Initialized)
                return;

            Initialized = true;
        }


        public void SetPosition(Vector2 position)
        {
            transform.position = position;
            PendingPoint = transform.position + 0.5f * Vector3.up;
            OriginalPoint = transform.position;
        }
        public bool IsFront
        {
            get => _isFront;
            set
            {
                _isFront = value;
                foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>().Except(new[] { _liquidLine }))
                {
                    spriteRenderer.sortingLayerName = value ? "Front" : "Default";
                }
            }
        }
        public void PickLiquid(int groupId, int value)
        {
            if (IsFull)
                return;
            SetLiquidData(groupId, value);
            ClearLiquid();
            _listLiquidData.ForEach(l => AddLiquid(l.groupId, l.value));
        }
        public void ClearColor()
        {
            ClearLiquid();
            _listLiquidData.Clear();
        }
        public IEnumerator MoveAndTransferLiquid(HolderEditor holder, Action onLiquidTransferComplete = null)
        {
            IsPending = false;

            var deliverAbsAngle = 82;
            var deliverTopPosition = holder.transform.TransformPoint(5 * Vector3.up);

            if (holder.IsFull || !_liquids.Any())
            {
                yield break;
            }

            yield return MoveNearToHolderForTransfer(holder);

            var isRightSide = holder.transform.position.x > transform.position.x;
            var sidePoint = isRightSide ? _rightSideDeliverPoint : _leftSideDeliverPoint;
            var deliverAngle = isRightSide ? -deliverAbsAngle : deliverAbsAngle;

            var relativePoint = transform.position - sidePoint.position;
            var rotatedRelativePoint = Quaternion.AngleAxis(deliverAngle, Vector3.forward) * relativePoint;

            var targetHolderPoint = rotatedRelativePoint + deliverTopPosition;
            var targetHolderRotation = Quaternion.AngleAxis(deliverAngle, Vector3.forward);

            var startPoint = transform.position;
            var startRotation = transform.rotation;

            var thisLiquid = _liquids.Last();
            var thisDataLiquid = _listLiquidData.Last();

            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                transform.position = Vector3.Lerp(startPoint, targetHolderPoint, n);
                transform.rotation = Quaternion.Lerp(startRotation, targetHolderRotation, n);
            }, speed: 2);

            var thisLiquidStartValue = thisLiquid.ValueRenderer;
            var transferValue = 1;
            if (holder.Liquids.LastOrDefault() == null || holder.Liquids.LastOrDefault().GroupId != thisLiquid.GroupId)
            {
                holder.AddLiquid(thisLiquid.GroupId);
            }
            var targetLiquid = holder.Liquids.Last();
            var targetLiquidStartValue = targetLiquid.ValueRenderer;

            _liquidLine.transform.position = sidePoint.position;
            _liquidLine.gameObject.SetActive(true);
            _liquidLine.transform.localScale =
                _liquidLine.transform.localScale.WithY(sidePoint.transform.position.y - holder.transform.position.y);
            _liquidLine.color = thisLiquid.Renderer.color;
            _liquidLine.transform.rotation = Quaternion.identity;

            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                thisLiquid.ValueRenderer = Mathf.Lerp(thisLiquidStartValue, thisLiquidStartValue - transferValue, n);
                targetLiquid.ValueRenderer = Mathf.Lerp(targetLiquidStartValue, targetLiquidStartValue + transferValue, n);
            }, speed: 2);

            if (Mathf.RoundToInt(thisLiquid.ValueRenderer) == 0)
            {
                _listLiquidData.Remove(thisDataLiquid);
                _liquids.Remove(thisLiquid);
                Destroy(thisLiquid.gameObject);
            }
            else
            {
                thisDataLiquid.value -= 1;
                _listLiquidData[_listLiquidData.Count - 1] = thisDataLiquid;
                thisLiquid.ValueRenderer = Mathf.RoundToInt(thisLiquid.ValueRenderer);
            }
            holder.SetLiquidData(thisLiquid.GroupId, transferValue);

            _liquidLine.gameObject.SetActive(false);
            targetLiquid.ValueRenderer = Mathf.RoundToInt(targetLiquid.ValueRenderer);
            onLiquidTransferComplete?.Invoke();

            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                transform.position = Vector3.Lerp(targetHolderPoint, startPoint, n);
                transform.rotation = Quaternion.Lerp(targetHolderRotation, startRotation, n);
            }, speed: 2);

            yield return ReturnToOriginalPoint();
        }

        private IEnumerator ReturnToOriginalPoint()
        {
            StopMoveIfAlready();
            var speed = GetSpeedForDistance((transform.position - OriginalPoint).magnitude);
            yield return MoveToEnumerator(OriginalPoint, Mathf.Max(speed, 3));
        }

        private IEnumerator MoveNearToHolderForTransfer(HolderEditor holder)
        {
            var targetPoint = holder.transform.TransformPoint(transform.position.x > holder.transform.position.x
                ? _transferNearOffset.WithX(Mathf.Abs(_transferNearOffset.x))
                : _transferNearOffset.WithX(-Mathf.Abs(_transferNearOffset.x)));

            var speed = GetSpeedForDistance((transform.position - targetPoint).magnitude);
            StopMoveIfAlready();
            yield return MoveToEnumerator(targetPoint, Mathf.Max(speed, 3));
        }
        private float GetSpeedForDistance(float distance)
        {
            return 5 / distance;
        }

        private void AddLiquidData(int groupId, int value)
        {
            LiquidData liquidData = new LiquidData();
            liquidData.groupId = groupId;
            liquidData.value = value;
            _listLiquidData.Add(liquidData);
        }
        private void SetLiquidData(int groupId, int value)
        {
            int countLiquid = _listLiquidData.Count;
            if (countLiquid <= 0)
            {
                AddLiquidData(groupId, value);
            }
            else
            {
                LiquidData liquidDataLast = _listLiquidData.Last();
                if (liquidDataLast.groupId == groupId)
                {
                    liquidDataLast.value++;
                    _listLiquidData[countLiquid - 1] = liquidDataLast;
                }
                else
                    AddLiquidData(groupId, value);
            }
        }
        private void AddLiquid(int groupId, float value = 0)
        {

            var topPoint = GetTopPoint();
            var liquid = Instantiate(_liquidPrefab, _content);

            liquid.IsBottomLiquid = !Liquids.Any();

            liquid.GroupId = groupId;
            liquid.transform.position = topPoint;
            liquid.gameObject.SetActive(true);
            liquid.ValueRenderer = value;
            liquid.SetUnitSize(unitSizeWater);
            _liquids.Add(liquid);
        }
        private void ClearLiquid()
        {
            for (int count = _liquids.Count - 1; count >= 0; count--)
            {
                Destroy(_liquids[count].gameObject);
            }
            _liquids.Clear();
        }
        private Vector2 GetTopPoint()
        {
            return transform.TransformPoint(Liquids.Sum(l => l.Size) * Vector2.up);
        }
        public Vector2 GetTopPoint(float totalValue)
        {
            return transform.TransformPoint(totalValue * Vector2.up * unitSizeWater);
        }

        public void StartPending()
        {
            if (IsPending)
                throw new InvalidOperationException();
            IsPending = true;
            IsFront = true;
            MoveTo(PendingPoint, speed: 5);
        }

        public void ClearPending()
        {
            IsPending = false;
            IsFront = false;
            MoveTo(OriginalPoint, speed: 5);
        }

        private void MoveTo(Vector2 point, float speed = 1, Action onFinished = null)
        {
            StopMoveIfAlready();

            _moveCoroutine = StartCoroutine(SimpleCoroutine.CoroutineEnumerator(MoveToEnumerator(point, speed), onFinished));
        }
        private void StopMoveIfAlready()
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        }
        private IEnumerator MoveToEnumerator(Vector2 toPoint, float speed = 1)
        {
            var startPoint = transform.position;
            yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
            {
                transform.position = Vector3.Lerp(startPoint, toPoint, n);
            }, speed: speed);
        }

        public void SetMaxWater(int maxWater)
        {
            _maxValue = maxWater;

            unitSizeWater = heightTube / _maxValue;

            float countLiquid = 0;
            int idRemove = -1;
            for (int i = 0; i < _liquids.Count; i++)
            {
                if (countLiquid + _liquids[i].ValueRenderer <= _maxValue)
                {
                    _liquids[i].SetUnitSize(unitSizeWater);
                    _liquids[i].transform.position = GetTopPoint(countLiquid);
                    countLiquid += _liquids[i].ValueRenderer;
                }
                else if (countLiquid < _maxValue)
                {
                    _liquids[i].SetValue(_maxValue - countLiquid, unitSizeWater);
                    _liquids[i].transform.position = GetTopPoint(countLiquid);
                    countLiquid = _maxValue;
                }
                else
                {
                    idRemove = i;
                    break;
                }
            }
            if (idRemove != -1)
            {
                for (int i = _liquids.Count - 1; i >= idRemove; i--)
                {
                    _listLiquidData.RemoveAt(i);
                    var thisLiquid = _liquids[i];
                    _liquids.Remove(thisLiquid);
                    Destroy(thisLiquid.gameObject);

                }
            }
        }
    }
}

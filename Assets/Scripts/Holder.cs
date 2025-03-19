using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Holder : MonoBehaviour
{
    [SerializeField] private int _maxValue = 4;
    [SerializeField] private float _ballRadius;
 
    [SerializeField] private AudioClip _popClip,_putClip;
    [SerializeField] private Liquid _liquidPrefab;
    [SerializeField] private Transform _content;
    [SerializeField] private Transform _leftSideDeliverPoint;
    [SerializeField] private Transform _rightSideDeliverPoint;
    [SerializeField] private Vector2 _transferNearOffset;
    [SerializeField] private SpriteRenderer _liquidLine;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _liquidTransferClip;

    [SerializeField] SpriteRenderer sprHole;
    [SerializeField] SpriteMask _spriteMask;
    [SerializeField] Transform _contence;

    public Transform pHoleMin;
    public Transform pHoleMax;

    public int IndexLayerMash = 0;
    
    private readonly List<Liquid> _liquids = new List<Liquid>();
    private Coroutine _moveCoroutine;
    private bool _isFront;

    public bool IsFull => Mathf.RoundToInt(_liquids.Sum(l=>l.Value))>=_maxValue;
    public Liquid TopLiquid => _liquids.LastOrDefault();
    public IEnumerable<Liquid> Liquids => _liquids;

    public int MAXValue => _maxValue;

    public float CurrentTotal => Liquids.Sum(l => l.Value);
    public bool IsPending { get;private set; }

    public bool Initialized { get; private set; }
    public Vector2 PendingPoint
    {
        get;
        private set;
    }
    
    public Vector3 OriginalPoint { get; private set; }

    public bool IsFront
    {
        get => _isFront;
        set
        {
            _isFront = value;
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>().Except(new []{_liquidLine}))
            {
               // spriteRenderer.sortingLayerName = value ? "Front" : "Default";
            }
        }
    }

    public void SetLayerMask(int layerMask)
    {
        sprHole.sortingOrder = layerMask + 1;
        _spriteMask.frontSortingOrder = layerMask;
        _spriteMask.backSortingOrder = layerMask - 2;

        int index = 1;

        foreach (var item in Liquids)
        {
            item.SetOrderLayerMask(layerMask - 1);
        }
    }

    public void StartPending()
    {
        if (IsPending)
            throw new InvalidOperationException();

        SetLayerMask(3000);

        IsPending = true;
        IsFront = true;
        MoveTo(PendingPoint,speed:5);
        PlayClipIfCan(_popClip);
    }

    public void ClearPending()
    {
        IsPending = false;
        IsFront = false;
        MoveTo(OriginalPoint,speed:5);
        PlayClipIfCan(_putClip);
        SetLayerMask(IndexLayerMash);
    }

    private IEnumerator MoveNearToHolderForTransfer(Holder holder)
    {
        var targetPoint = holder.transform.TransformPoint(transform.position.x > holder.transform.position.x
            ? _transferNearOffset.WithX(Mathf.Abs(_transferNearOffset.x))
            : _transferNearOffset.WithX(-Mathf.Abs(_transferNearOffset.x)));

        var speed = GetSpeedForDistance((transform.position - targetPoint).magnitude);
        StopMoveIfAlready();
        yield return MoveToEnumerator(targetPoint,Mathf.Max(speed,3));
    }

    private float GetSpeedForDistance(float distance)
    {
        return 5 / distance;
    }

    private IEnumerator ReturnToOriginalPoint()
    {
        StopMoveIfAlready();
        var speed = GetSpeedForDistance((transform.position - OriginalPoint).magnitude);
        yield return MoveToEnumerator(OriginalPoint, Mathf.Max(speed,3));
    }

    private void StopMoveIfAlready()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
    }

    private Vector3 pointConetceCurrent = Vector3.zero;
    float height = 0;
    float scaleY = 0;

    private void Start()
    {
        height = Vector3.Distance(pHoleMin.transform.position, pHoleMax.transform.position);
        pointConetceCurrent = _contence.transform.localPosition;
        scaleY = _contence.localScale.y;
    }

    private void Update()
    {
        _contence.rotation = Quaternion.identity;
        _liquidLine.transform.rotation = Quaternion.identity;

        Vector3 angles = transform.eulerAngles;

        float Mashcode = Mathf.Cos(angles.z * Mathf.Deg2Rad);

        Debug.Log("hihihi_" + angles.z + "////" + height * (1 - Mashcode));

        _contence.transform.localPosition = pointConetceCurrent + new Vector3(0, height * (1 - Mashcode), 0);

     //  _contence.transform.localScale = new Vector3(_contence.transform.localScale.x, scaleY * Mashcode, _contence.transform.localScale.z);
    }

    void SetParent(Transform tr)
    {
        foreach(var lq in _liquids)
        {
            lq.transform.parent = tr;
        }    
    }    

    public IEnumerator MoveAndTransferLiquid(Holder holder,Action onLiquidTransferComplete=null)
    {
        float hightHole = Vector2.Distance(pHoleMin.position, pHoleMax.position);

        IsPending = false;
        
        var deliverTopPosition = holder.transform.TransformPoint(5 * Vector3.up);

        
        if(holder.IsFull || !_liquids.Any() || holder.Liquids.Any() && holder.Liquids.Last().GroupId != Liquids.Last().GroupId)
        {
            yield break;
        }

        yield return MoveNearToHolderForTransfer(holder);

      //  SetParent(null);

        var thisLiquid = _liquids.Last();

        var hightLayer = Vector2.Distance(pHoleMin.position, thisLiquid.p2.position);
        var hightLayer1 = Vector2.Distance(pHoleMin.position, thisLiquid.p1.position);

        float egular = Mathf.Acos(hightLayer / hightHole) * Mathf.Rad2Deg;
        
        float egular1 = Mathf.Acos(hightLayer1 / hightHole) * Mathf.Rad2Deg;

        var deliverAbsAngle = egular;
        var deliverAbsAngle1 = egular1;

        var isRightSide = holder.transform.position.x > transform.position.x;
        var sidePoint = isRightSide ? _rightSideDeliverPoint : _leftSideDeliverPoint;

        var deliverAngle = isRightSide ? -deliverAbsAngle : deliverAbsAngle;
        var relativePoint = transform.position - sidePoint.position;
        var rotatedRelativePoint = Quaternion.AngleAxis(deliverAngle, Vector3.forward) * relativePoint;
        var targetHolderPoint = rotatedRelativePoint + deliverTopPosition;
        var targetHolderRotation = Quaternion.AngleAxis(deliverAngle, Vector3.forward);


        var deliverAngle1 = isRightSide ? -deliverAbsAngle1 : deliverAbsAngle1;
        var rotatedRelativePoint1 = Quaternion.AngleAxis(deliverAngle1, Vector3.forward) * relativePoint;
        var targetHolderPoint1 = rotatedRelativePoint1 + deliverTopPosition;
        var targetHolderRotation1 = Quaternion.AngleAxis(deliverAngle1, Vector3.forward);

        var startPoint = transform.position;
        var startRotation = transform.rotation;


        yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
        {
            transform.position = Vector3.Lerp(startPoint, targetHolderPoint, n);
            transform.rotation = Quaternion.Lerp(startRotation, targetHolderRotation, n);
        }, speed: 2);


        var startRotation1 = transform.rotation;
        var startPoint1 = transform.position;

        var thisLiquidStartValue = thisLiquid.Value;
        var transferValue = Mathf.Min(thisLiquid.Value,holder.MAXValue - holder.CurrentTotal);

        
        if (holder.Liquids.LastOrDefault() == null)
        {
            holder.AddLiquid(thisLiquid.GroupId);
        }
        var targetLiquid = holder.Liquids.Last();
        var targetLiquidStartValue = targetLiquid.Value;

        _liquidLine.transform.position = sidePoint.position;
        _liquidLine.gameObject.SetActive(true);
        _liquidLine.transform.localScale =
            _liquidLine.transform.localScale.WithY(sidePoint.transform.position.y - holder.transform.position.y);
        _liquidLine.color = thisLiquid.Renderer.color;
       // _liquidLine.transform.rotation = Quaternion.identity;
        _audio.clip = _liquidTransferClip;
        _audio.Play();
        _audio.volume = transferValue / 5;

      // SetParent(_contence);

        yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
        {
            transform.position = Vector3.Lerp(startPoint1, targetHolderPoint1, n);
            transform.rotation = Quaternion.Lerp(startRotation1, targetHolderRotation1, n);

            thisLiquid.Value = Mathf.Lerp(thisLiquidStartValue, thisLiquidStartValue - transferValue, n);
            targetLiquid.Value = Mathf.Lerp(targetLiquidStartValue, targetLiquidStartValue + transferValue, n);
        }, speed: 2);


        var currentCon = _contence.transform.localPosition;

      //  _contence.DOLocalMoveY(currentCon.y + 2.75f, 0.5f);

      //  yield return StartCoroutine(Setvalue(thisLiquid, targetLiquid, thisLiquidStartValue - transferValue, targetLiquidStartValue + transferValue));

        if (thisLiquid.Value <= 0.05f)
        {
            _liquids.Remove(thisLiquid);
            Destroy(thisLiquid.gameObject);
        }
        else
        {
            thisLiquid.Value = Mathf.RoundToInt(thisLiquid.Value);
        }

       // SetParent(_contence);

        _audio.Stop();
        _liquidLine.gameObject.SetActive(false);
        targetLiquid.Value = Mathf.RoundToInt(targetLiquid.Value);
        onLiquidTransferComplete?.Invoke();
      //  SetParent(_contence);
        // _contence.DOLocalMoveY(currentCon.y, 0.5f);
        yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
        {
            if(_contence.transform.localPosition.y >= currentCon.y)
            {
              //  _contence.transform.localPosition += new Vector3(0, -n, 0);
            }

            transform.position = Vector3.Lerp(targetHolderPoint, startPoint, n);
            transform.rotation = Quaternion.Lerp(targetHolderRotation, startRotation, n);
        }, speed: 2);

        //   _contence.transform.localPosition.WithY(currentCon.y);
        yield return ReturnToOriginalPoint();
        IsFront = false;
        ResetLayerMash();
        holder.ResetLayerMash();
    }

    IEnumerator Setvalue(Liquid lq1, Liquid lq2, float value1, float value2)
    {
        var start1 = lq1.Value;
        var start2 = lq2.Value;

        while (true)
        {
            start1 -= Time.deltaTime;
            start2 += Time.deltaTime;

         
            bool isbreak = false;
            bool isbreak1 = false;

            if (!isbreak)
            {
               // _contence.transform.localPosition += new Vector3(0, Time.deltaTime, 0);
            }


            if (start1 <= value1 && !isbreak)
            {
                start1 = value1;
                isbreak = true;
            }

            if (start2 >= value2 && !isbreak1)
            {
                start2 = value2;
                isbreak1 = true;
            }
            lq1.Value = start1;
            lq2.Value = start2;


            if (isbreak && isbreak1)
            {
                yield break;
            }

            yield return null;
        }
    }

    public void ResetLayerMash()
    {
        SetLayerMask(IndexLayerMash);
    }



    public void AddLiquid(int groupId, float value = 0)
    {
        var topPoint = GetTopPoint();
        var liquid = Instantiate(_liquidPrefab,_content);

        liquid.IsBottomLiquid = !Liquids.Any();
        
        liquid.GroupId = groupId;
        liquid.transform.position = topPoint;
        
        
        liquid.Value = value;
        _liquids.Add(liquid);
    }

    public Vector2 GetTopPoint()
    {
        return transform.TransformPoint(Liquids.Sum(l => l.Size) * Vector2.up);
    }

    private void PlayClipIfCan(AudioClip clip,float volume=0.35f)
    {
        if(!AudioManager.IsSoundEnable || clip==null)
            return;
        AudioSource.PlayClipAtPoint(clip,Camera.main.transform.position,volume);
    }

    public void Init(IEnumerable<LiquidData> liquidDatas)
    {
        var list = liquidDatas.ToList();
        if(Initialized)
            return;
    
        list.ForEach(l=>AddLiquid(l.groupId,l.value));
        PendingPoint = transform.position + 0.5f * Vector3.up;
        OriginalPoint = transform.position ;
        Initialized = true;
    }


    public void MoveTo(Vector2 point, float speed = 1,Action onFinished=null)
    {
       StopMoveIfAlready();
        
        _moveCoroutine = StartCoroutine(SimpleCoroutine.CoroutineEnumerator(MoveToEnumerator(point, speed),onFinished));
    }
    
    private IEnumerator MoveToEnumerator(Vector2 toPoint,float speed=1)
    {
        var startPoint = transform.position;
        yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
        {
            transform.position = Vector3.Lerp(startPoint, toPoint, n);
        },speed:speed);
    }

}
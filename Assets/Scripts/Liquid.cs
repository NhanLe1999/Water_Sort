using WaterSort;
using Spine.Unity;
using System;
using Unity.Collections;
using UnityEngine;
using DG.Tweening;
namespace WaterSort
{
    public class Liquid : MonoBehaviour
    {
        [SerializeField] private int _groupId;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private float _unitSize;
        private float _valueRenderer;
        private int _valueReal;

        private bool _isBottomLiquid;
        [SerializeField] private SpriteRenderer _surfaceRenderer;
        [SerializeField] private SpriteRenderer _surfaceDeepRenderer;
        public int GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                UpdateColor();
            }
        }

        public SpriteRenderer Renderer => _renderer;

        public bool IsBottomLiquid
        {
            get => _isBottomLiquid;
            set
            {
                _isBottomLiquid = value;
                ValueReal = ValueReal;
            }
        }

        public float ValueRenderer
        {
            set
            {
                _surfaceRenderer.transform.localPosition = Vector3.down * 0.02f + new Vector3(0, 0, 1);
                //_surfaceRenderer.transform.localScale = _surfaceRenderer.transform.localScale.WithY(_unitSize * value);
                _surfaceRenderer.size = _surfaceRenderer.size.WithY(_unitSize * value / 2 + 0.1f);

                _surfaceDeepRenderer.transform.localPosition = Vector3.down * 0.02f;
                _surfaceDeepRenderer.transform.localScale = _surfaceDeepRenderer.transform.localScale.WithY(_unitSize * value);

                _valueRenderer = value;
            }

            get => _valueRenderer;
        }

        public int ValueReal
        {
            set
            {
                _renderer.transform.localPosition = Vector3.zero;
                _renderer.transform.localScale = _renderer.transform.localScale.WithY(_unitSize * value);
                _valueReal = value;
            }
            get => _valueReal;
        }

        public void UpdateValueNotUpdateSuface(int value)
        {
            _valueReal += value;
        }

        public int ValueTarget { set; get; }


        public float Size => ValueReal * _unitSize;

        public float SizeRender => ValueRenderer * _unitSize;

        public void UpdateColor()
        {
            Color tempColor;
            if (GameConfig.BLIND_MODE)
            {
                tempColor = ResourceManager.Instance._blindColors[GroupId];
            }
            else
            {
                tempColor = ResourceManager.Instance._normalColors[GroupId];
            }
            _renderer.color = tempColor;
            _surfaceRenderer.color = tempColor;
            _surfaceDeepRenderer.color = tempColor;

        }


        public void ActiveEffectFour(bool isActive, bool isReceiver)
        {
            _surfaceRenderer.transform.localScale = new Vector2(1, 2);
            _surfaceRenderer.size = new Vector2(1.5f, 0);
            _renderer.gameObject.SetActive(!isActive);
            _surfaceRenderer.gameObject.SetActive(isActive && isReceiver);
            _surfaceDeepRenderer.gameObject.SetActive(isActive && !isReceiver);

        }

        public void EffectAffterFour(float time)
        {
            _surfaceRenderer.transform.DOLocalMoveY(-0.18f, time).SetEase(Ease.Linear);
            _surfaceRenderer.transform.DOScaleX(5, time).SetEase(Ease.Linear);
        }

        public void sortingOrder(int idLayer)
        {
            _renderer.sortingOrder = idLayer;
            _surfaceRenderer.sortingOrder = idLayer;
            _surfaceDeepRenderer.sortingOrder = idLayer;
        }
        public void sortingLayerName(string nameLayer)
        {
            _renderer.sortingLayerName = nameLayer;
            _surfaceRenderer.sortingLayerName = nameLayer;
            _surfaceDeepRenderer.sortingLayerName = nameLayer;
        }


        public void SetValue(float _newValue, float _newSize)
        {
            this._unitSize = _newSize;
            ValueReal = (int)_newValue;
        }

        public void SetUnitSize(float _newSize)
        {
            this._unitSize = _newSize;
            ValueReal = ValueReal;
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace WaterSort
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] public float _minXDistanceBetweenHolders = 2f;
        [SerializeField] public float _offsetXDistanceBetweenHolders = 1;
        [SerializeField] private Camera _camera;
        [SerializeField] private Holder _holderPrefab;
        [SerializeField] private GameObject tutorialObject;
        [SerializeField] private Transform parentHolder;

        public event Action OnChangeHolderState;
        public event Action<bool> OnNotificationMove;
        public event Action OnUserTapHolder;
        public event Action OnResetExpandState;
        public event Action UpdateNumberItem;

        public GameMode GameMode { get; private set; } = GameMode.Easy;
        public Level Level { get; private set; }

        private readonly List<Holder> _holders = new List<Holder>();

        private readonly Stack<MoveData> _undoStack = new Stack<MoveData>();
        private readonly List<Holder> _holdersTransferList = new List<Holder>();

        public State CurrentState { get; set; } = State.None;

        public bool HaveUndo => _undoStack.Count > 0 && _holders.All(_holder => !_holder.IsReceiving);

        public bool canExpand { private set; get; }

        public bool isActiveBottom { private set; get; }

        private int defaultWater = 4;
        private int maxWater;
        public int countTutorial { set; get; }
        private bool isTutorialFromComplete;
        private void Awake()
        {
            Instance = this;
            GameManager.OnFinishLoadData += GameManager_OnFinishLoadData;
            ShopPanel.OnUpdateSkin += ShopPanel_OnUpdateSkin;
            Application.runInBackground = false;
        }

        public void ShopPanel_OnUpdateSkin()
        {
            GameConfig.ID_BOTTLE_SELECT = GameConfig.ID_BOTTLE;
            Sprite skinSprite = ResourceManager.LoadBottle(GameConfig.ID_BOTTLE_SELECT);
            Sprite maskSprite = ResourceManager.LoadMask(GameConfig.ID_BOTTLE_SELECT);
            for (int count = 0; count < _holders.Count; count++)
            {
                _holders[count].ChangeSkin(skinSprite, maskSprite);
            }
        }

        private void ShopPanel_OnUpdateSkin(int idBottle)
        {
            GameConfig.ID_BOTTLE_SELECT = idBottle;
            Sprite skinSprite = ResourceManager.LoadBottle(GameConfig.ID_BOTTLE_SELECT);
            Sprite maskSprite = ResourceManager.LoadMask(GameConfig.ID_BOTTLE_SELECT);
            for (int count = 0; count < _holders.Count; count++)
            {
                _holders[count].ChangeSkin(skinSprite, maskSprite);
            }
        }

        private void GameManager_OnFinishLoadData()
        {

            //Awake

            var loadGameData = GameManager.LoadGameData;
            GameMode = loadGameData.GameMode;
            Level = loadGameData.Level;
            maxWater = Level.maxWaterInTube;
            if (maxWater == 0) maxWater = defaultWater;


            LoadLevel();
            CurrentState = State.Playing;
            countTutorial = 0;
            UIManager.Instance.PlayPaneSetup();

            // Start    
           // GetBottleTry(Level.no);
            SetTutorial();
            Invoke("CheckMoveWhenStartGame", 1);

        }

        public void HandelInvokeUpdateNumberItem()
        {
            UpdateNumberItem?.Invoke();
        }

        private void OnDestroy()
        {
            GameManager.OnFinishLoadData -= GameManager_OnFinishLoadData;
            ShopPanel.OnUpdateSkin -= ShopPanel_OnUpdateSkin;
        }

        private void GetBottleTry(int level)
        {
            if (GameMode == GameMode.Undefined)
            {
                if (level < 10 || GameConfig.IsRequestLevel(level))
                {
                    ShopPanel_OnUpdateSkin();
                    return;
                }
                if (level % 10 == 0 || level % 10 == 1 || level % 10 == 2)
                {
                    for (int idBottle = 0; idBottle < GameConfig.TOTAL_BOTTLE; idBottle++)
                    {
                        if (!GameConfig.IsUnlock(ItemShop.Type.Bottle, idBottle) && !GameConfig.IsRequestBottle(idBottle))
                        {
                            ShopPanel_OnUpdateSkin(idBottle);
                            if (level % 10 == 2)
                            {
                                GameConfig.RequestBottle(idBottle);
                                GameConfig.RequestLevel(level);
                                //SharedUIManager.PopupGetSkin.Show(idBottle);
                                UIManager.Instance.ShowGetBottle(idBottle);
                            }
                            return;
                        }
                    }
                    ShopPanel_OnUpdateSkin();
                }
                else
                {
                    ShopPanel_OnUpdateSkin();
                }
            }
            else
            {
                ShopPanel_OnUpdateSkin();
            }
        }

        private void CheckMoveWhenStartGame()
        {
            OnNotificationMove?.Invoke(CheckMove());

        }

        private void LoadLevel()
        {
            DeleteHolder();
            OnResetExpandState?.Invoke();
            canExpand = true;
            UIManager.Instance.SetStateSkip();
            var list = PositionsForHolders(Level.map.Count).ToList();
            //_camera.orthographicSize = 0.5f * width * 1.2f * Screen.height / Screen.width;

            var levelMap = Level.LiquidDataMap;
            for (var i = 0; i < levelMap.Count; i++)
            {
                var levelColumn = levelMap[i];
                var holder = Instantiate(_holderPrefab, list[i], Quaternion.identity);
                holder.Init(maxWater, levelColumn, _holders.Count);
                _holders.Add(holder);
                holder.SetParentRoot(parentHolder);
            }
        }

        public void OnClickUndo()
        {
            // Debug.Log("UNDO :" + _undoStack.Count);
            if (CurrentState != State.Playing || _undoStack.Count <= 0 || _holdersTransferList.Count > 0)
                return;

            var moveData = _undoStack.Pop();
            // MoveBallFromOneToAnother(moveData.ToHolder, moveData.FromHolder);
        }
        public bool IsHavePouring => _holders.Any(holder => holder.IsReceiving);
        public void InvokeActionChangeHolderState()
        {
            OnChangeHolderState?.Invoke();
        }
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.C))
            {
                OverTheGame();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Help_Expand();
            }
#endif

            if (CurrentState != State.Playing)
                return;



            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUIObject()) return;
                var collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(Input.mousePosition));
                if (collider != null)
                {
                    var holder = collider.GetComponent<Holder>();

                    if (holder != null)
                        OnClickHolder(holder);
                }
            }

        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public void UpdateColorMode()
        {
            foreach (var holder in _holders)
                holder.UpdateColorMode();
        }

        private void OnClickHolder(Holder holder)
        {
            if (holder.IsFinish || holder.IsTransfer)
                return;
            if (GameMode != GameMode.DailyChallenge)
            {
                if (Level.no == 1)
                {
                    if (countTutorial == 0)
                    {
                        if (!SetInforTutorial(holder, 1)) return;
                    }
                    else if (countTutorial == 1)
                    {
                        if (!SetInforTutorial(holder, 0)) return;
                    }
                    else
                        return;
                }
                else if (Level.no == 2)
                {
                    if (countTutorial == 0)
                    {
                        if (!SetInforTutorial(holder, 0)) return;
                    }
                    else if (countTutorial == 1)
                    {
                        if (!SetInforTutorial(holder, 2)) return;
                    }
                    else if (countTutorial == 3)
                    {
                        if (!SetInforTutorial(holder, 1)) return;
                    }
                    else if (countTutorial == 4)
                    {
                        if (!SetInforTutorial(holder, 0)) return;
                    }
                    else if (countTutorial == 6)
                    {
                        if (!SetInforTutorial(holder, 1)) return;
                    }
                    else if (countTutorial == 7)
                    {
                        if (!SetInforTutorial(holder, 2)) return;
                    }
                    else
                        return;
                }
            }

            OnUserTapHolder?.Invoke();
            var pendingHolder = _holders.FirstOrDefault(h => h.IsPending);

            if (pendingHolder != null && pendingHolder != holder)
            {
                if (holder.TopLiquid == null || (pendingHolder.TopLiquid.GroupId == holder.TopLiquid.GroupId && !holder.IsFull))
                {
                    AddListHolderMove(pendingHolder);
                    Holder tempPendingHolder = pendingHolder;
                    Holder tempHolder = holder;
                    //SetUndoStack(pendingHolder, holder);
                    StartCoroutine(SimpleCoroutine.CoroutineEnumerator(pendingHolder.MoveAndTransferLiquid(holder, CheckAndGameOver), () =>
                     {
                         //Debug.Log(pendingHolder.CurrentTotal + "___" + tempPendingHolder.CurrentTotal);
                         if (pendingHolder.CurrentTotal != tempPendingHolder.CurrentTotal)
                         {
                             
                         }
                         RemoveListHolderMove(pendingHolder);
                         if (Level.no == 1)
                         {

                         }
                         else if (Level.no == 2)
                         {
                             if (countTutorial == 2)
                             {

                                 countTutorial++;
                                 SetTutorial();
                                 OnUserTapHolder?.Invoke();
                             }
                             else if (countTutorial == 5)
                             {

                                 countTutorial++;
                                 SetTutorial();
                                 OnUserTapHolder?.Invoke();
                             }
                             else if (countTutorial == 8)
                             {

                             }
                         }
                         OnChangeHolderState?.Invoke();
                     }));
                }
                else
                {
                    pendingHolder.ClearPending();
                    holder.StartPending();
                }
            }
            else if (holder.Liquids.Any())
            {
                if (holder.IsReceiving)
                {
                    return;
                }
                else
                {
                    if (!holder.IsPending)
                    {
                        holder.StartPending();
                    }
                    else
                    {
                        holder.ClearPending();
                    }
                }
            }
        }

        private void CheckAndGameOver()
        {
            if (
                _holders.All(holder =>
            {
                var liquids = holder.Liquids.ToList();
                return liquids.Count == 0 || liquids.Count == 1;
            }) &&
            _holders.Where(holder => holder.Liquids.Any()).GroupBy(holder => holder.Liquids.First().GroupId)
                .All(holders => holders.Count() == 1))
            {
                OverTheGame();
            }
            else
            {
                OnNotificationMove?.Invoke(CheckMove());
            }

        }

        public void OverTheGame()
        {
            if (CurrentState != State.Playing)
                return;
            CurrentState = State.Over;
            _holdersTransferList.Clear();
            DeleteSaveGame();
            ResourceManager.CompleteLevel(GameMode, Level.no);
            if (GameMode == GameMode.DailyChallenge)
                DailyChallenge.Instance.CompletePartOfChallenge();
            DOVirtual.DelayedCall(1.5f, () =>
            {
                float orthographicSize = Screen.height * 0.005f;
                orthographicSize = orthographicSize > 10.0f ? 10.0f : orthographicSize;
                //_camera.orthographicSize = orthographicSize;
                UIManager.Instance.ShowLevelCompleted();
                SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_GAME_VICTORY);
            });
        }
        public IEnumerable<Vector2> PositionsForHolders(int count)
        {
            var identifier = SystemInfo.deviceModel;


            if (count <= 5)
            {
                _minXDistanceBetweenHolders = ResourceManager.XDistanceBottle()[count - 1];
                var minPoint = transform.position - ((count - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right - Vector3.up * GameConfig.HIEGHT_IMAGE_BOTTLE / 2;
                return Enumerable.Range(0, count)
                    .Select(i => (Vector2)minPoint + i * _minXDistanceBetweenHolders * Vector2.right);
            }
            else
            {
                var maxCountInRow = Mathf.CeilToInt(count / 2f);

                var list = new List<Vector2>();

                _minXDistanceBetweenHolders = ResourceManager.XDistanceBottle()[maxCountInRow - 1];
                var topRowMinPoint = transform.position - ((maxCountInRow - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right ;
                list.AddRange(Enumerable.Range(0, maxCountInRow).Select(i => (Vector2)topRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

                _minXDistanceBetweenHolders = ResourceManager.XDistanceBottle()[count - maxCountInRow - 1];
                var lowRowMinPoint = transform.position - (((count - maxCountInRow) - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right - Vector3.up * 7 ;
                list.AddRange(Enumerable.Range(0, count - maxCountInRow).Select(i => (Vector2)lowRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

                return list;
            }
        }


        private (Holder pendingHolder, Holder tranferHolder) GetHoldersEnable()
        {
            List<(Holder pendingHolder, Holder tranferHolder)> listHolderToWater = new List<(Holder pendingHolder, Holder tranferHolder)>();
            List<(Holder pendingHolder, Holder tranferHolder)> listHolderNoWater = new List<(Holder pendingHolder, Holder tranferHolder)>();
            int countListHolder = _holders.Count;
            for (int count = 0; count < countListHolder; count++)
            {
                Holder pendingHolder = _holders[count];
                if (pendingHolder.Liquids.Any() && !pendingHolder.IsFinish)
                {
                    for (int temp = 0; temp < countListHolder; temp++)
                    {
                        if (count != temp)
                        {
                            Holder tranferHolder = _holders[temp];
                            if (tranferHolder.TopLiquid != null &&
                                pendingHolder.TopLiquid.GroupId == tranferHolder.TopLiquid.GroupId &&
                                pendingHolder.TopLiquid.ValueReal <= maxWater - tranferHolder.CurrentTotal
                                && !tranferHolder.IsFull)
                            {
                                listHolderToWater.Add((pendingHolder, tranferHolder));
                            }
                        }
                    }

                    for (int temp = 0; temp < countListHolder; temp++)
                    {
                        if (count != temp)
                        {
                            Holder tranferHolder = _holders[temp];
                            if (tranferHolder.TopLiquid == null)
                                listHolderNoWater.Add((pendingHolder, tranferHolder));
                        }
                    }
                }
            }

            if (listHolderToWater.Count > 0)
            {
                return listHolderToWater[Random.Range(0, listHolderToWater.Count)];
            }
            if (listHolderNoWater.Count > 0)
            {
                return listHolderNoWater[Random.Range(0, listHolderNoWater.Count)];
            }
            return (null, null);
        }
        public void Help_Undo()
        {
            if (CurrentState != State.Playing || _undoStack.Count <= 0 || _holdersTransferList.Count > 0)
                return;
            var moveData = _undoStack.Pop();
            _holders[moveData.idFromHolder].UndoLiquid(moveData.listLiquidFromHolder);
            _holders[moveData.idToHolder].UndoLiquid(moveData.listLiquidToHolder);
            OnNotificationMove?.Invoke(CheckMove());
        }
        public bool Help_Hint()
        {
            if (_holdersTransferList.Count > 0)
                return false;
            (Holder fromHolder, Holder toHolder) holderHint = GetHoldersEnable();
            if (holderHint.fromHolder != null && holderHint.toHolder != null)
            {
                SetUndoStack(holderHint.fromHolder, holderHint.toHolder);
                AddListHolderMove(holderHint.fromHolder);
                holderHint.fromHolder.IsFront = true;

                StartCoroutine(SimpleCoroutine.CoroutineEnumerator(holderHint.fromHolder.MoveAndTransferLiquid(holderHint.toHolder, CheckAndGameOver), () =>
                {
                    RemoveListHolderMove(holderHint.fromHolder);

                    OnChangeHolderState?.Invoke();
                }));
                return true;
            }
            return false;

        }
        public void Help_Bottom()
        {

            if (!isActiveBottom)
            {

            }
        }

        private List<(int GroupId, float Value)> ConvertLiquidDataFromLiquid(IEnumerable<Liquid> Liquids, bool isToHolder)
        {
            List<(int GroupId, float Value)> listLiquid = new List<(int GroupId, float Value)>();
            if (!isToHolder)
            {
                for (int count = 0; count < Liquids.Count(); count++)
                {
                    Liquid liquid = Liquids.ToList()[count];
                    (int GroupId, float Value) convertLiquid = (liquid.GroupId, liquid.ValueReal);
                    listLiquid.Add(convertLiquid);
                }
            }
            else
            {
                for (int count = 0; count < Liquids.Count(); count++)
                {
                    Liquid liquid = Liquids.ToList()[count];
                    (int GroupId, float Value) convertLiquid = (liquid.GroupId, liquid.ValueTarget);
                    listLiquid.Add(convertLiquid);
                }
                /*
                if (Liquids.Count() > 1)
                {
                    for (int count = 0; count < Liquids.Count() - 1; count++)
                    {
                        Liquid liquid = Liquids.ToList()[count];
                        (int GroupId, float Value) convertLiquid = (liquid.GroupId, liquid.ValueReal);
                        listLiquid.Add(convertLiquid);
                    }

                    (int GroupId, float Value) convertLiquidLast = (Liquids.Last().GroupId, Liquids.Last().ValueTarget);
                    listLiquid.Add(convertLiquidLast);
                }
                else 
                {
                    (int GroupId, float Value) convertLiquidLast = (Liquids.Last().GroupId, Liquids.Last().ValueTarget);
                    listLiquid.Add(convertLiquidLast);
                }
                */
            }
            return listLiquid;
        }
        public void Help_Expand()
        {

            if (!canExpand)
            {
                return;
            }

            //canExpand = false;

            // int total = Level.map.Count + 1;
            int total = _holders.Count + 1;
            var list = PositionsForHolders(total).ToList();


            for (var i = 0; i < total; i++)
            {
                if (_holders.Count > i)
                {
                    _holders[i].ChangePosition(list[i]);
                }
                else
                {
                    var holder = Instantiate(_holderPrefab, list[i], Quaternion.identity);
                    /*
                    Sprite skinSprite = ResourceManager.LoadBottle(GameConfig.ID_BOTTLE_SELECT);
                    Sprite maskSprite = ResourceManager.LoadMask(GameConfig.ID_BOTTLE_SELECT);
                    holder.ChangeSkin(skinSprite, maskSprite);
                    */
                    holder.Init(maxWater, new List<LiquidData>(), _holders.Count);
                    holder.SetParentRoot(parentHolder);
                    _holders.Add(holder);
                    OnNotificationMove?.Invoke(CheckMove());
                }
            }
        }
        public void SetUndoStack(Holder fromHolder, Holder toHolder)
        {
            _undoStack.Push(new MoveData
            {
                idFromHolder = _holders.IndexOf(fromHolder),
                listLiquidFromHolder = ConvertLiquidDataFromLiquid(fromHolder.Liquids, false),
                idToHolder = _holders.IndexOf(toHolder),
                listLiquidToHolder = ConvertLiquidDataFromLiquid(toHolder.Liquids, true)
            });

        }

        private void AddListHolderMove(Holder fromHolder)
        {
            _holdersTransferList.Add(fromHolder);
        }

        private void RemoveListHolderMove(Holder fromHolder)
        {
            if (_holdersTransferList.Contains(fromHolder))
            {
                _holdersTransferList.Remove(fromHolder);
            }
        }

        public void SaveGame()
        {
            if (_holders.Count <= 0 || (GameMode != GameMode.DailyChallenge && Level.no <= 2) || CurrentState != State.Playing)
                return;
            Level level = new Level();
            level.no = Level.no;
            level.maxWaterInTube = Level.maxWaterInTube;
            List<LevelColumn> listLevelColumn = new List<LevelColumn>();
            for (int count = 0; count < _holders.Count; count++)
            {
                List<int> values = new List<int>();
                List<Liquid> listLiquidData = _holders[count].Liquids.ToList();
                for (int temp = 0; temp < listLiquidData.Count; temp++)
                {
                    int groupId = listLiquidData[temp].GroupId;
                    float value = listLiquidData[temp].ValueReal;
                    for (int countValue = 0; countValue < value; countValue++)
                    {
                        values.Add(groupId);
                    }
                }
                LevelColumn levelColumn = new LevelColumn();
                levelColumn.values = values;
                listLevelColumn.Add(levelColumn);
            }
            level.map = listLevelColumn;
            string jsonText = JsonUtility.ToJson(level, true);
            if (GameMode != GameMode.DailyChallenge)
                GameStatics.DATA_NOMAL_LEVEL = jsonText;
            else
                GameStatics.DATA_DAILY_LEVEL = jsonText;
        }

        public void DeleteSaveGame()
        {
            if (GameMode != GameMode.DailyChallenge)
                GameStatics.DATA_NOMAL_LEVEL = "";
            else
                GameStatics.DATA_DAILY_LEVEL = "";
        }

        public void Restart()
        {
            DeleteSaveGame();
            if (GameMode == GameMode.DailyChallenge)
                GameManager.Instance.LoadDataDaily();
            else
                GameManager.Instance.LoadData();

            /*
            OnNotificationMove?.Invoke(true);
            DeleteSaveGame();
            LoadLevel();
            CurrentState = State.Playing;
            GetBottleTry(Level.no);
            Invoke("CheckMoveWhenStartGame", 1);
            */
        }

        private void DeleteHolder()
        {
            for (int count = 0; count < _holders.Count; count++)
            {
                _holders[count].SwapOut();
            }
            _holders.Clear();
            _undoStack.Clear();
        }

        public void Skip()
        {
            CurrentState = State.Over;
            _holdersTransferList.Clear();
            DeleteSaveGame();
            ResourceManager.CompleteLevel(GameMode, Level.no);
            if (GameMode == GameMode.DailyChallenge)
                DailyChallenge.Instance.CompletePartOfChallenge();
            NextGame();

        }

        private void HoldersIn()
        {
            for (int count = 0; count < _holders.Count; count++)
            {
                _holders[count].SwapIn();
            }
        }
        public void NextGame()
        {
            if (GameMode == GameMode.Undefined)
                UIManager.Instance.ResetCountRestart();
            else
                UIManager.Instance.ResetCountRestartChallege();
            isTutorialFromComplete = true;
            DeleteSaveGame();
            if (GameMode == GameMode.DailyChallenge && DailyChallenge.Instance.LevelInOrder <= 5)
            {
                GameManager.Instance.LoadDataDaily();
            }
            else
            {
                GameManager.Instance.LoadData();
            }
        }

        public void SelectLevel(int level)
        {
            DeleteSaveGame();
            ResourceManager.SelectLevel(GameMode.Undefined, level - 1);
            for (int count = _holders.Count - 1; count >= 0; count--)
            {
                Destroy(_holders[count].gameObject);
            }
            _holders.Clear();
            GameManager.Instance.LoadData();
        }



        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGame();
            }
        }
        public enum State
        {
            None,
            Playing,
            Over
        }

        public struct MoveData
        {
            public int idFromHolder { get; set; }
            public List<(int GroupId, float Value)> listLiquidFromHolder { get; set; }
            public int idToHolder { get; set; }
            public List<(int GroupId, float Value)> listLiquidToHolder { get; set; }
        }

        private bool CheckMove()
        {
            for (int count = 0; count < _holders.Count; count++)
            {
                if (!_holders[count].Liquids.Any())
                {
                    return true;
                }
            }

            for (int count = 0; count < _holders.Count; count++)
            {
                for (int temp = 0; temp < _holders.Count; temp++)
                {
                    if (count != temp && _holders[count].Liquids.Any() && !_holders[count].IsFinish && !_holders[temp].IsFull &&
                    _holders[count].TopLiquid.GroupId == _holders[temp].TopLiquid.GroupId &&
                   _holders[count].TopLiquid.ValueReal <= maxWater - _holders[temp].CurrentTotal
                        )
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        private void SetTutorial()
        {
            if (GameMode != GameMode.DailyChallenge)
            {
                if (Level.no == 1)
                {
                    if (countTutorial == 0)
                    {
                        tutorialObject.SetActive(true);
                        tutorialObject.transform.position = _holders[1].OriginalPoint;
                    }
                    else if (countTutorial == 1)
                    {
                        tutorialObject.SetActive(true);
                        tutorialObject.transform.position = _holders[0].OriginalPoint;

                    }
                    else
                    {
                        tutorialObject.SetActive(false);
                    }
                }
                else if (Level.no == 2)
                {
                    if (countTutorial == 0)
                    {
                        if (isTutorialFromComplete)
                        {
                            tutorialObject.SetActive(true);
                            tutorialObject.transform.position = _holders[0].OriginalPoint;
                        }
                        else
                        {
                            tutorialObject.SetActive(true);
                            tutorialObject.transform.position = _holders[0].OriginalPoint;
                        }

                    }
                    else if (countTutorial == 1)
                    {
                        tutorialObject.SetActive(true);
                        tutorialObject.transform.position = _holders[2].OriginalPoint;
                    }
                    else if (countTutorial == 2)
                    {
                        tutorialObject.SetActive(false);
                    }
                    else if (countTutorial == 3)
                    {
                        tutorialObject.SetActive(true);
                        tutorialObject.transform.position = _holders[1].OriginalPoint;
                    }
                    else if (countTutorial == 4)
                    {
                        tutorialObject.SetActive(true);
                        tutorialObject.transform.position = _holders[0].OriginalPoint;
                    }
                    else if (countTutorial == 5)
                    {
                        tutorialObject.SetActive(false);

                    }
                    else if (countTutorial == 6)
                    {
                        tutorialObject.SetActive(true);
                        tutorialObject.transform.position = _holders[1].OriginalPoint;
                    }
                    else if (countTutorial == 7)
                    {
                        tutorialObject.SetActive(true);
                        tutorialObject.transform.position = _holders[2].OriginalPoint;
                    }
                    else if (countTutorial == 8)
                    {
                        tutorialObject.SetActive(false);
                    }
                }
                else
                {
                    tutorialObject.SetActive(false);
                }
            }
        }

        private bool SetInforTutorial(Holder holder, int index)
        {
            if (_holders.IndexOf(holder) != index)
                return false;
            countTutorial++;
            SetTutorial();
            return true;
        }
    }

    [Serializable]
    public struct LevelGroup : IEnumerable<Level>
    {
        public List<Level> levels;

        public IEnumerator<Level> GetEnumerator()
        {
            return levels?.GetEnumerator() ?? Enumerable.Empty<Level>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    public struct Level
    {
        public int no;
        public int maxWaterInTube;
        public List<LevelColumn> map;

        public List<IEnumerable<LiquidData>> LiquidDataMap => map.Select(GetLiquidDatas).ToList();

        public static IEnumerable<LiquidData> GetLiquidDatas(LevelColumn column)
        {
            var list = column.ToList();

            for (var j = 0; j < list.Count; j++)
            {
                var currentGroup = list[j];
                var count = 0;
                for (; j < list.Count; j++)
                {
                    if (currentGroup == list[j])
                    {
                        count++;
                    }
                    else
                    {
                        j--;
                        break;
                    }
                }

                yield return new LiquidData
                {
                    groupId = currentGroup,
                    value = count
                };
            }
        }
    }

    public struct LiquidData
    {
        public int groupId;
        public float value;
    }

    [Serializable]
    public struct LevelColumn : IEnumerable<int>
    {
        public List<int> values;

        public IEnumerator<int> GetEnumerator()
        {
            return values?.GetEnumerator() ?? Enumerable.Empty<int>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
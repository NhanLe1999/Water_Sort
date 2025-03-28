using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WaterSort;

namespace LevelEditor
{
    public class LevelEditorManager : MonoBehaviour
    {
        [SerializeField] Button btnAddTube;
        [SerializeField] Button btnRemoveTube;
        [SerializeField] Button btnPickColor;
        [SerializeField] Button btnShuffleColor;
        [SerializeField] Button btnLoadLevel;
        [SerializeField] Button btnSaveLevel;
        [SerializeField] Button btnReset;
        [SerializeField] Button btnValidate;
        [SerializeField] ButtonPickColorEditor[] arrayButtonCollor;
        [SerializeField] HolderEditor holderEditorPrefab;
        [SerializeField] PopupInfor popupInfor;
        [SerializeField] PopupNotification popupNotification;
        [SerializeField] PopupWarning popupWarning;
        private List<HolderEditor> listHolderEditor = new List<HolderEditor>();
        [SerializeField] float _minXDistanceBetweenHolders;
        [SerializeField] Camera _camera;
        [SerializeField] GameObject pickColorPanel;

        private const int defaultWater = 4;
        private int totalWater;
        [SerializeField] private InputField numberWaterInTube;

        private HolderEditor holderEditorSelected;
        private enum State
        {
            pickColor = 0,
            shuffleColor = 1
        }
        private State gameState;

        private bool IsTransfer { get; set; }



        private void Start()
        {
            totalWater = defaultWater;
            btnAddTube.onClick.AddListener(ButtonAddTubeListener);
            btnRemoveTube.onClick.AddListener(buttonRemoveTubeListener);
            btnPickColor.onClick.AddListener(ButtonStatePickColorListener);
            btnShuffleColor.onClick.AddListener(ButtonStateShuffleColorListener);
            btnLoadLevel.onClick.AddListener(ButtonLoadLevelListener);
            btnSaveLevel.onClick.AddListener(ButtonSaveLevelListener);
            btnReset.onClick.AddListener(ButtonResetListener);
            btnValidate.onClick.AddListener(ButtonValidateListener);
            numberWaterInTube.onEndEdit.AddListener(OnEndEditNumberWater);

            for (int count = 0; count < arrayButtonCollor.Length; count++)
            {
                if (arrayButtonCollor[count].gameObject.activeInHierarchy)
                {
                    arrayButtonCollor[count].SetAction(count);
                    arrayButtonCollor[count].OnClick += ButtonPickColorOnClick;
                }
            }
            ButtonStatePickColorListener();
            popupInfor.OnClick += PopupInforOKButton_OnClick;
            popupWarning.OnClick += PopupWarning_OnClick;
        }
        private void OnEndEditNumberWater(string endStr)
        {
            int endValue = int.Parse(endStr);
            if (endValue < 4) endValue = 4;
            totalWater = endValue;
            foreach (var holder in listHolderEditor)
            {
                holder.SetMaxWater(endValue);
            }
        }
        private void ButtonValidateListener()
        {
            string Error = string.Empty;
            bool isError = false;
            Dictionary<int, int> dictColors = new Dictionary<int, int>();

            var listTube = listHolderEditor;
            foreach (var tube in listTube)
            {
                var values = tube.listLiquidData.ToList();

                foreach (var value in values)
                {
                    if (dictColors.ContainsKey(value.groupId))
                        dictColors[value.groupId] += Mathf.RoundToInt(value.value);
                    else dictColors.Add(value.groupId, Mathf.RoundToInt(value.value));
                }
            }

            for (int i = 0; i < dictColors.Count; i++)
            {
                if (dictColors.ElementAt(i).Value % totalWater != 0)
                {
                    isError = true;
                    Error += string.Format("[{0}:{1}] ", dictColors.ElementAt(i).Key, dictColors.ElementAt(i).Value);
                }
            }
            if (isError)
            {
                Debug.LogError("Có màu đang chưa vừa đủ! " + Error);
            }
            else
            {
                Debug.Log("Levels đã đủ màu ở các bình!");
            }

        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                var collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(Input.mousePosition));
                if (collider != null)
                {
                    var holderEditor = collider.GetComponent<HolderEditor>();
                    if (holderEditor != null)
                        OnClickHolder(holderEditor);
                }
            }
        }
        private void CheckAndGameOver()
        {

        }

        private void OnClickHolder(HolderEditor holder)
        {
            if (IsTransfer)
                return;

            var pendingHolder = listHolderEditor.FirstOrDefault(h => h.IsPending);


            if (pendingHolder != null && pendingHolder != holder)
            {
                if (gameState == State.pickColor)
                {
                    holder.StartPending();
                    pendingHolder.ClearPending();
                    holderEditorSelected = holder;
                }
                else if (gameState == State.shuffleColor)
                {

                    if (!holder.IsFull && pendingHolder.Liquids.Any())
                    {
                        IsTransfer = true;
                        StartCoroutine(SimpleCoroutine.CoroutineEnumerator(pendingHolder.MoveAndTransferLiquid(holder, CheckAndGameOver), () =>
                        {
                            IsTransfer = false;
                        }));
                    }
                    else
                    {
                        pendingHolder.ClearPending();
                    }
                }
            }
            else //if (listHolderEditor.Liquids.Any())
            {
                if (!holder.IsPending)
                {
                    holder.StartPending();
                    holderEditorSelected = holder;
                }
                else
                {
                    holder.ClearPending();
                    holderEditorSelected = null;
                }
            }

        }

        private IEnumerable<Vector2> PositionsForHolders(int count)
        {
            if (count
                <= 6)
            {
                var minPoint = transform.position - ((count - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                               Vector3.up * 1f;
                return Enumerable.Range(0, count).Select(i => (Vector2)minPoint + i * _minXDistanceBetweenHolders * Vector2.right);
            }
            else
            {
                var maxCountInRow = Mathf.CeilToInt(count / 2f);
                float expectWidth = 4 * _minXDistanceBetweenHolders;
                var aspect = (float)Screen.width / Screen.height;
                if ((maxCountInRow + 1) * _minXDistanceBetweenHolders > expectWidth)
                {
                    expectWidth = (maxCountInRow + 1) * _minXDistanceBetweenHolders;
                }
                var height = expectWidth / aspect;

                var list = new List<Vector2>();
                var topRowMinPoint = transform.position + Vector3.up * height / 6f -
                            ((maxCountInRow - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right - Vector3.up * 1f;
                list.AddRange(Enumerable.Range(0, maxCountInRow)
                    .Select(i => (Vector2)topRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

                var lowRowMinPoint = transform.position - Vector3.up * height / 6f -
                                     (((count - maxCountInRow) - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                                     Vector3.up * 1f;
                list.AddRange(Enumerable.Range(0, count - maxCountInRow)
                    .Select(i => (Vector2)lowRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));
                return list;
            }
        }

        private void CreateHolders()
        {
            var holder = Instantiate(holderEditorPrefab);
            holder.SetMaxWater(totalWater);
            listHolderEditor.Add(holder);
            var listPositionsForHolders = PositionsForHolders(listHolderEditor.Count).ToList();
            for (int count = 0; count < listHolderEditor.Count; count++)
            {
                listHolderEditor[count].SetPosition(listPositionsForHolders[count]);
            }
            holder.gameObject.SetActive(true);
        }

        private void ChangeSizeCamera()
        {
            int count = listHolderEditor.Count;
            float width = 4 * _minXDistanceBetweenHolders;
            if (count <= 6)
                width = Mathf.Max(count * _minXDistanceBetweenHolders, width);
            else
            {
                var maxCountInRow = Mathf.CeilToInt(count / 2f);
                var aspect = (float)Screen.width / Screen.height;
                if ((maxCountInRow + 1) * _minXDistanceBetweenHolders > width)
                    width = (maxCountInRow + 1) * _minXDistanceBetweenHolders;
            }
            _camera.orthographicSize = 0.5f * width * Screen.height / Screen.width;
        }

        private void ResetSelected()
        {
            if (holderEditorSelected != null)
                holderEditorSelected.ClearPending();
            holderEditorSelected = null;

        }

        private void ButtonAddTubeListener()
        {
            CreateHolders();
            ChangeSizeCamera();
        }

        private void buttonRemoveTubeListener()
        {
            if (holderEditorSelected != null)
            {
                GameObject holder = holderEditorSelected.gameObject;
                listHolderEditor.Remove(holderEditorSelected);
                Destroy(holder);
                ChangeSizeCamera();
                var listPositionsForHolders = PositionsForHolders(listHolderEditor.Count).ToList();
                for (int count = 0; count < listHolderEditor.Count; count++)
                {
                    listHolderEditor[count].SetPosition(listPositionsForHolders[count]);
                }
            }
            else
            {
                string info = "Chọn 01 chai bằng cách click vào chai.";
                ShowInfor(info);
            }
        }

        private void ButtonStatePickColorListener()
        {
            pickColorPanel.SetActive(true);
            gameState = State.pickColor;
        }

        private void ButtonStateShuffleColorListener()
        {
            ResetSelected();
            pickColorPanel.SetActive(false);
            gameState = State.shuffleColor;
        }

        private void ButtonLoadLevelListener()
        {
            if (listHolderEditor.Count > 0)
            {
                string warning = "Sẽ bị mất hết số ống hiện tại. Chắc chắn load level mới?";
                ShowWarning(PopupWarning.TypePopup.warningLoadLevel, warning);
            }
            else
            {
                popupInfor.SetActive(true, PopupInfor.Type.loadLevel, "Load Level");
            }
        }

        private void ButtonSaveLevelListener()
        {
            if (listHolderEditor.Count <= 0)
            {
                string info = "Cần có TỐI THIỂU 01 chai.";
                ShowInfor(info);
                return;
            }
            popupInfor.SetActive(true, PopupInfor.Type.saveLevel, "Save Level");
        }

        private void ButtonResetListener()
        {
            for (int count = 0; count < listHolderEditor.Count; count++)
            {
                Destroy(listHolderEditor[count].gameObject);
            }
            listHolderEditor.Clear();

        }
        private void ButtonPickColorOnClick(ButtonPickColorEditor.Type type, int value, Color color)
        {

            if (holderEditorSelected != null)
            {
                if (type != ButtonPickColorEditor.Type.clearColor)
                {
                    holderEditorSelected.PickLiquid(value, 1);
                }
                else
                {
                    holderEditorSelected.ClearColor();
                }
            }
            else
            {
                string info = "Chọn 01 chai bằng cách click vào chai.";
                ShowInfor(info);
            }
        }

        private int idLevelSave;
        private void PopupInforOKButton_OnClick(PopupInfor.Type type, int levelID)
        {
            if (levelID <= 0)
            {
                string info = "Level phải được điền và giá trị phải LỚN HƠN 0.";
                ShowInfor(info);
                return;
            }

            if (type == PopupInfor.Type.loadLevel)
            {
                LoadLevel(levelID);
            }
            else if (type == PopupInfor.Type.saveLevel)
            {
                if (CheckFileExist(levelID))
                {
                    string warning = "Đã tồn tại level. Chắc chắn muốn ghi đè?";
                    ShowWarning(PopupWarning.TypePopup.warningSaveLevel, warning);
                    idLevelSave = levelID;
                }
                else
                {
                    popupInfor.SetActive(false);
                    SaveLevel(levelID);
                }
            }
        }


        private void PopupWarning_OnClick(PopupWarning.TypePopup typePopup, PopupWarning.ButtonType buttonType)
        {
            if (typePopup == PopupWarning.TypePopup.warningLoadLevel)
            {
                if (buttonType == PopupWarning.ButtonType.okButton)
                {
                    popupInfor.SetActive(true, PopupInfor.Type.loadLevel, "Load Level");
                }
                popupWarning.SetActive(false);
            }
            else
            {
                if (buttonType == PopupWarning.ButtonType.okButton)
                {
                    SaveLevel(idLevelSave);
                }

                popupWarning.SetActive(false);
            }
        }

        private void LoadLevel(int levelID)
        {
            if (!CheckFileExist(levelID))
            {
                string info = "Chưa tồn tại LEVEL " + levelID + ".";
                ShowInfor(info);
                return;
            }
            ButtonResetListener();
            string filePath = GetFilePath(levelID);
            string txt = File.ReadAllText(filePath);

            Level level = JsonUtility.FromJson<Level>(txt);
            if (level.maxWaterInTube == 0) level.maxWaterInTube = defaultWater;
            totalWater = level.maxWaterInTube;
            numberWaterInTube.text = totalWater.ToString();

            List<LevelColumn> map = level.map;
            for (int count = 0; count < map.Count; count++)
            {
                CreateHolders();
                ChangeSizeCamera();

                LevelColumn levelColumn = map[count];
                List<int> values = levelColumn.values;
                for (int temp = 0; temp < values.Count; temp++)
                {
                    listHolderEditor[count].PickLiquid(values[temp], 1);
                }
            }
            popupInfor.SetActive(false);
        }
        private void SaveLevel(int levelID)
        {
            List<LevelColumn> map = new List<LevelColumn>();
            for (int count = 0; count < listHolderEditor.Count; count++)
            {
                List<int> values = new List<int>();
                List<LiquidData> listLiquidData = listHolderEditor[count].listLiquidData.ToList();
                for (int temp = 0; temp < listLiquidData.Count; temp++)
                {
                    int groupId = listLiquidData[temp].groupId;
                    float value = listLiquidData[temp].value;
                    for (int countValue = 0; countValue < value; countValue++)
                    {
                        values.Add(groupId);
                    }
                }
                LevelColumn levelColumn = new LevelColumn(values);
                map.Add(levelColumn);
            }

            Level level = new Level(levelID, map);
            level.maxWaterInTube = totalWater;

            string jsonText = JsonUtility.ToJson(level, true);
            string filePath = GetFilePath(levelID);
            File.WriteAllText(filePath, jsonText);
            popupInfor.SetActive(false);
            string info = "Đã lưu level " + levelID + " thành công!";
            ShowInfor(info);
        }

        private bool CheckFileExist(int levelID)
        {
            string filePath = GetFilePath(levelID);
            return File.Exists(filePath);

        }

        private string GetFilePath(int levelID)
        {
            return Path.Combine(Application.dataPath, "LevelCreated/lv" + levelID + ".json");
        }

        private void ShowInfor(string info)
        {
            popupNotification.SetActive(true, info);
        }

        private void ShowWarning(PopupWarning.TypePopup typePopup, string warnig)
        {
            popupWarning.SetActive(true, typePopup, warnig);
        }
        private void OnDestroy()
        {
            for (int count = 0; count < arrayButtonCollor.Length; count++)
            {
                arrayButtonCollor[count].OnClick -= ButtonPickColorOnClick;
            }
        }


    }
    [Serializable]
    public class Level
    {
        public int no;
        public int maxWaterInTube;
        public List<LevelColumn> map;
        public Level(int no, List<LevelColumn> map)
        {
            this.no = no;
            this.map = map;
        }
    }

    [Serializable]
    public class LevelColumn
    {
        public List<int> values;
        public LevelColumn(List<int> values)
        {
            this.values = values;
        }
    }
}

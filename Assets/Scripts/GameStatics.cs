using System;
using UnityEngine;

namespace WaterSort
{
    public class GameStatics
    {
        private const string KEY_ITEM_UNDO = "user_item_undo";
        private const string KEY_ITEM_HINT = "user_item_hint";
        private const string KEY_ITEM_BOTTOM = "user_item_bottom";
        private const string KEY_ITEM_EXPAND = "user_item_expand";
        private const string KEY_DATA_NOMAL_LEVEL = "user_data_nomal_level";
        private const string KEY_DATA_DAILY_LEVEL = "user_data_daily_level";
        private const string KEY_DATA_SAVE_BACK_LEVEL_MODE = "user_data_save_back_level_mode";
        private static int? _undo;
        public static int ITEM_UNDO
        {
            get
            {
                if (_undo == null)
                    _undo = PlayerPrefs.GetInt(KEY_ITEM_UNDO, 5);
                _undo = _undo < 0 ? 0 : _undo;
                return (int)_undo;
            }
            set
            {
                _undo = value;
                _undo = _undo < 0 ? 0 : _undo;
                PlayerPrefs.SetInt(KEY_ITEM_UNDO, (int)_undo);
            }
        }

        private static int? _hint;
        public static int ITEM_HINT
        {
            get
            {
                if (_hint == null)
                    _hint = PlayerPrefs.GetInt(KEY_ITEM_HINT, 0);
                _hint = _hint < 0 ? 0 : _hint;
                return (int)_hint;
            }
            set
            {
                _hint = value;
                _hint = _hint < 0 ? 0 : _hint;
                PlayerPrefs.SetInt(KEY_ITEM_HINT, (int)_hint);
            }
        }

        private static int? _bottom;
        public static int ITEM_BOTTOM
        {
            get
            {
                if (_bottom == null)
                    _bottom = PlayerPrefs.GetInt(KEY_ITEM_BOTTOM, 0);
                _bottom = _bottom < 0 ? 0 : _bottom;
                return (int)_bottom;
            }
            set
            {
                _bottom = value;
                _bottom = _bottom < 0 ? 0 : _bottom;
                PlayerPrefs.SetInt(KEY_ITEM_BOTTOM, (int)_bottom);
            }
        }

        private static int? _expand;
        public static int ITEM_EXPAND
        {
            get
            {
                if (_expand == null)
                    _expand = PlayerPrefs.GetInt(KEY_ITEM_EXPAND, 0);
                _expand = _expand < 0 ? 0 : _expand;
                return (int)_expand;
            }
            set
            {
                _expand = value;
                _expand = _expand < 0 ? 0 : _expand;
                PlayerPrefs.SetInt(KEY_ITEM_EXPAND, (int)_expand);
            }
        }

        private static string _data_nomal_level;
        public static string DATA_NOMAL_LEVEL
        {
            get
            {
                if (_data_nomal_level == null)
                    _data_nomal_level = PlayerPrefs.GetString(KEY_DATA_NOMAL_LEVEL, "");
                return _data_nomal_level;
            }
            set
            {
                _data_nomal_level = value;
                PlayerPrefs.SetString(KEY_DATA_NOMAL_LEVEL, _data_nomal_level);

            }
        }

        private static string _data_daily_level;
        public static string DATA_DAILY_LEVEL
        {
            get
            {
                if (_data_daily_level == null)
                    _data_daily_level = PlayerPrefs.GetString(KEY_DATA_DAILY_LEVEL, "");
                return _data_daily_level;
            }
            set
            {
                _data_daily_level = value;
                PlayerPrefs.SetString(KEY_DATA_DAILY_LEVEL, _data_daily_level);

            }
        }

        private static int? _back_level_mode;
        public static int BACK_LEVEL_MODE
        {
            get
            {
                if (_back_level_mode == null)
                    _back_level_mode = PlayerPrefs.GetInt(KEY_ITEM_BOTTOM, 0);
                _back_level_mode = _back_level_mode < 0 ? 0 : _back_level_mode;
                return (int)_back_level_mode;
            }
            set
            {
                if (_back_level_mode == null)
                    _back_level_mode = PlayerPrefs.GetInt(KEY_ITEM_BOTTOM, 0);
                    _back_level_mode = _back_level_mode < value ? value : _back_level_mode;
                _back_level_mode = _back_level_mode < 0 ? 0 : _back_level_mode;

                PlayerPrefs.SetInt(KEY_ITEM_BOTTOM, (int)_back_level_mode);
            }
        }
    }
}
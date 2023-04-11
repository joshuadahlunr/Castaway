using System;
using UnityEngine;

/// <summary>
/// Class for storing upgrade information (resources, 
/// resource cost, progress towards upgrade, and
/// current upgrade level)
/// </summary>
/// <author>Misha Desear</author>

namespace ResourceMgmt
{
    public class UpgradeData : MonoBehaviour
    {
        [SerializeField] private int _resources;
        public int Resources
        {
            get => _resources;
            set { _resources = value; } 
        }

        [SerializeField] private int _cost;
        public int Cost
        {
            get => _cost;
            set { _cost = value; }
        }

        [SerializeField] private int _progress;
        public int Progress
        {
            get => _progress;
            set { _progress = value; }
        }

        [SerializeField] private int _level;
        public int Level
        {
            get => _level;
            set { _level = value; }
        }

        public void ResetProgress()
        {
            // If progress is equal to cost, reset progress to 0
            if (_progress == _cost)
            {
                _progress = 0;

            }

            // Otherwise, set progress to difference of progress and cost
            else if (_progress > _cost)
            {
                _progress =- _cost;
            }
        }

        public bool CanBuy()
        {
            if (_resources < _cost)
            {
                return false;
            }
            return true;
        }
    }
}
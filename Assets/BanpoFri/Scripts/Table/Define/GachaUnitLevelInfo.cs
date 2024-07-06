using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class GachaUnitLevelInfoData
    {
        [SerializeField]
		private int _level;
		public int level
		{
			get { return _level;}
			set { _level = value;}
		}
		[SerializeField]
		private List<int> _gacha_info;
		public List<int> gacha_info
		{
			get { return _gacha_info;}
			set { _gacha_info = value;}
		}

    }

    [System.Serializable]
    public class GachaUnitLevelInfo : Table<GachaUnitLevelInfoData, int>
    {
    }
}


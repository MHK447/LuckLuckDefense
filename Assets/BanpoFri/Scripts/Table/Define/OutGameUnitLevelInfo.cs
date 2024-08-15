using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class OutGameUnitLevelInfoData
    {
        [SerializeField]
		private int _level;
		public int level
		{
			get { return _level;}
			set { _level = value;}
		}
		[SerializeField]
		private int _cardcount;
		public int cardcount
		{
			get { return _cardcount;}
			set { _cardcount = value;}
		}

    }

    [System.Serializable]
    public class OutGameUnitLevelInfo : Table<OutGameUnitLevelInfoData, int>
    {
    }
}


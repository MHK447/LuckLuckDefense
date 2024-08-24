using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitOutGameSkillInfoData
    {
        [SerializeField]
		private int _skill_idx;
		public int skill_idx
		{
			get { return _skill_idx;}
			set { _skill_idx = value;}
		}
		[SerializeField]
		private int _skill_value;
		public int skill_value
		{
			get { return _skill_value;}
			set { _skill_value = value;}
		}
		[SerializeField]
		private string _name;
		public string name
		{
			get { return _name;}
			set { _name = value;}
		}

    }

    [System.Serializable]
    public class UnitOutGameSkillInfo : Table<UnitOutGameSkillInfoData, int>
    {
    }
}


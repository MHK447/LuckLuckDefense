using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class SkillCardInfoData
    {
        [SerializeField]
		private int _skill_idx;
		public int skill_idx
		{
			get { return _skill_idx;}
			set { _skill_idx = value;}
		}
		[SerializeField]
		private string _image;
		public string image
		{
			get { return _image;}
			set { _image = value;}
		}
		[SerializeField]
		private string _desc_name;
		public string desc_name
		{
			get { return _desc_name;}
			set { _desc_name = value;}
		}

    }

    [System.Serializable]
    public class SkillCardInfo : Table<SkillCardInfoData, int>
    {
    }
}


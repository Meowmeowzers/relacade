using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
	[Serializable]
	public class GridRow
	{
		[SerializeField] public List<GridColumn> row;

		public GridRow(int rows)
		{
			row = new List<GridColumn>(rows);
			
			for(int i  = 0; i < rows; i++)
			{
				row.Add(new GridColumn());
			}
		}
	}

	[Serializable]
	public class GridColumn
	{
		[SerializeField] public List<GameObject> column;

		public GridColumn()
		{
			column = new List<GameObject>();
		}
	}
}

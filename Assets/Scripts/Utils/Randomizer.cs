using System;
using System.Collections.Generic;

public class Randomizer
{
	[Serializable]
	public class Data
	{
		public int capacity;
		public int storedCapacity;
		public List<int> lastIndexes;
	}

	public Data data;

	public Randomizer(int capacity, int storedCapacity = -1)
	{
		data = new Data();
		data.capacity = capacity;

		CountStoredCapacity(storedCapacity);

		data.lastIndexes = new List<int>();
	}

	public int GetNextIndex()
	{
		List<int> freeIndexes = new List<int>();
		for (int i = 0; i < data.capacity; i++)
		{
			if (!data.lastIndexes.Contains(i))
			{
				freeIndexes.Add(i);
			}
		}
		int index = freeIndexes[UnityEngine.Random.Range(0, freeIndexes.Count)];

		data.lastIndexes.Add(index);
		CorrectLastIndecies();

		return index;
	}

	public void AddLastIndex(int index)
	{
		data.lastIndexes.Add(index);
		CorrectLastIndecies();
	}

	public List<int> GetLastIndexes()
	{
		return data.lastIndexes;
	}

	public void SetLastIndexes(List<int> indexes)
	{
		data.lastIndexes = indexes;
	}

	private void CorrectLastIndecies()
    {
		while(data.lastIndexes.Count > data.storedCapacity)
        {
			data.lastIndexes.RemoveAt(0);
		}
	}

	private void UpdateData(Data loadedData)
    {
		data.lastIndexes = loadedData.lastIndexes;
	}

	private void CountStoredCapacity(int initialValue)
    {
		data.storedCapacity = initialValue;
		if (data.storedCapacity == -1)
		{
			data.storedCapacity = data.capacity / 2;
		}
	}
}

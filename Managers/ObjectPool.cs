using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
	[Header("Set Dynamically")]
	// Singleton
	private static ObjectPool _S;
	public static ObjectPool S { get { return _S; } set { _S = value; } }

	public List<GameObject> pooledObjects;
	public List<ObjectPoolItem> itemsToPool;
	public Transform poolAnchor;

	// DontDestroyOnLoad
	private bool exists;

	void Awake() {
		// Singleton
		S = this;

		// DontDestroyOnLoad
		if (!exists) {
			exists = true;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}

		// Pool List (on MainCamera)
		pooledObjects = new List<GameObject>();
		foreach (ObjectPoolItem item in itemsToPool) {
			for (int i = 0; i < item.amountToPool; i++) {
				GameObject obj = (GameObject)Instantiate(item.objectToPool);
				obj.SetActive(false);
				pooledObjects.Add(obj);
				obj.transform.SetParent(poolAnchor);
			}
		}
	}

	// Search by TAG
	public GameObject GetPooledObject(string tag) {
		for (int i = 0; i < pooledObjects.Count; i++) {
			if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag) {
				return pooledObjects[i];
			}
		}
		foreach (ObjectPoolItem item in itemsToPool) {
			if (item.objectToPool.tag == tag) {
				if (item.shouldExpand) {
					GameObject obj = (GameObject)Instantiate(item.objectToPool);
					obj.SetActive(false);
					pooledObjects.Add(obj);
					return obj;
				}
			}
		}
		return null;
	}

	// Set Position
	public void PosAndEnableObj(GameObject tGo, GameObject tPos) {
		if (tGo != null) {
			tGo.transform.position = tPos.transform.position;
			tGo.SetActive(true);
		}
	}

	// Called RPGLevelMan(133)
	public void SpawnObjects(string currentScene) {

		// Deactivate PoolObjects
		foreach (Transform child in poolAnchor) {
			child.gameObject.SetActive(false);
		}

		//////////////////////////////////
		// WAS used to spawn randomly positioned bugs and bats
		//////////////////////////////////

		//int 		qty;
		//float 		minX;
		//float 		minY;
		//Vector2 	location;

		//switch (currentScene) {
		//case "zTown":
		//case "SNES_Overworld":

		//	// Bug_1 //////////////////////////////////////////////////////////
		//	// Random Quantity
		//	qty = Random.Range (1, itemsToPool [0].amountToPool);

		//	for (int i = 0; i < qty; i++) {
		//		// Randomly Assign Location
		//		minX = Random.Range (-10, 10);
		//		minY = Random.Range (-10, 10);
		//		location = new Vector2 (minX, minY);

		//		// Activate & Position GO
		//		GameObject tGO = ObjectPool.S.GetPooledObject ("Bug_1");
		//		PosAndEnableObj (tGO, location);
		//	}

		//	// Bug_2 //////////////////////////////////////////////////////////
		//	// Random Quantity
		//	qty = Random.Range (1, itemsToPool [1].amountToPool);

		//	for (int i = 0; i < qty; i++) {
		//			// Randomly Assign Location
		//			minX = Random.Range(-10, 10);
		//			minY = Random.Range(-10, 10);
		//			location = new Vector2 (minX, minY);

		//		// Activate & Position GO
		//		GameObject tGO = ObjectPool.S.GetPooledObject ("Bug_2");
		//		PosAndEnableObj (tGO, location);
		//	}

		//	// Bug_3 //////////////////////////////////////////////////////////
		//	// Random Quantity
		//	qty = Random.Range (1, 3);

		//	for (int i = 0; i < qty; i++) {
		//			// Randomly Assign Location
		//			minX = Random.Range(-10, 10);
		//			minY = Random.Range(-10, 10);
		//			location = new Vector2 (minX, minY);

		//		// Activate & Position GO
		//		GameObject tGO = ObjectPool.S.GetPooledObject ("Bug_3");
		//		PosAndEnableObj (tGO, location);
		//	}

		//	// Bug_4 //////////////////////////////////////////////////////////
		//	// Random Quantity
		//	qty = Random.Range (1, itemsToPool [3].amountToPool);

		//	for (int i = 0; i < qty; i++) {
		//			// Randomly Assign Location
		//			minX = Random.Range(-10, 10);
		//			minY = Random.Range(-10, 10);
		//			location = new Vector2 (minX, minY);

		//		// Activate & Position GO
		//		GameObject tGO = ObjectPool.S.GetPooledObject ("Bug_4");
		//		PosAndEnableObj (tGO, location);
		//	}

		//	// Bug_5 //////////////////////////////////////////////////////////
		//	// Random Quantity
		//	qty = Random.Range (1, itemsToPool [4].amountToPool);

		//	for (int i = 0; i < qty; i++) {
		//			// Randomly Assign Location
		//			minX = Random.Range(-10, 10);
		//			minY = Random.Range(-10, 10);
		//			location = new Vector2 (minX, minY);

		//		// Activate & Position GO
		//		GameObject tGO = ObjectPool.S.GetPooledObject ("Bug_5");
		//		PosAndEnableObj (tGO, location);
		//	}

		//	// Bug_6 //////////////////////////////////////////////////////////
		//	// Random Quantity
		//	qty = Random.Range (1, itemsToPool [5].amountToPool);

		//	for (int i = 0; i < qty; i++) {
		//			// Randomly Assign Location
		//			minX = Random.Range(-10, 10);
		//			minY = Random.Range(-10, 10);
		//			location = new Vector2 (minX, minY);

		//		// Activate & Position GO
		//		GameObject tGO = ObjectPool.S.GetPooledObject ("Bug_6");
		//		PosAndEnableObj (tGO, location);
		//	}

		//	break;
		//case "Cave_1":
		//              // Get bat positions
		//		Vector2[] tPos = {
		//              new Vector2(-15,-10), new Vector2(-33.5f, -12.5f), new Vector2(-50, 11), new Vector2(-28.5f, 8),
		//              new Vector2(-18,26f), new Vector2(0,42), new Vector2(12,24f), new Vector2(33,3f),
		//              new Vector2(30,-16), new Vector2(53,-9f), new Vector2(-20,-15), new Vector2(-45f, -16f),
		//              new Vector2(-45, 6), new Vector2(-33.5f, 5), new Vector2(-13,23.5f), new Vector2(7,40),
		//              new Vector2(20,20), new Vector2(28,-2.5f), new Vector2(40f,-21), new Vector2(48,-12.5f)

		//              ,new Vector2(-15,-10), new Vector2(-33.5f, -12.5f), new Vector2(-50, 11), new Vector2(-28.5f, 8),
		//		new Vector2(-18,26f), new Vector2(0,42), new Vector2(12,24f), new Vector2(33,3f),
		//		new Vector2(30,-16), new Vector2(53,-9f), new Vector2(-20,-15), new Vector2(-45f, -16f),
		//		new Vector2(-45, 6), new Vector2(-33.5f, 5), new Vector2(-13,23.5f), new Vector2(7,40),
		//		new Vector2(20,20), new Vector2(28,-2.5f), new Vector2(40f,-21), new Vector2(48,-12.5f)

		//		,new Vector2(-15,-10), new Vector2(-33.5f, -12.5f), new Vector2(-50, 11), new Vector2(-28.5f, 8),
		//		new Vector2(-18,26f), new Vector2(0,42), new Vector2(12,24f), new Vector2(33,3f),
		//		new Vector2(30,-16), new Vector2(53,-9f), new Vector2(-20,-15), new Vector2(-45f, -16f),
		//		new Vector2(-45, 6), new Vector2(-33.5f, 5), new Vector2(-13,23.5f), new Vector2(7,40),
		//		new Vector2(20,20), new Vector2(28,-2.5f), new Vector2(40f,-21), new Vector2(48,-12.5f)

		//		,new Vector2(-15,-10), new Vector2(-33.5f, -12.5f), new Vector2(-50, 11), new Vector2(-28.5f, 8),
		//		new Vector2(-18,26f), new Vector2(0,42), new Vector2(12,24f), new Vector2(33,3f),
		//		new Vector2(30,-16), new Vector2(53,-9f), new Vector2(-20,-15), new Vector2(-45f, -16f),
		//		new Vector2(-45, 6), new Vector2(-33.5f, 5), new Vector2(-13,23.5f), new Vector2(7,40),
		//		new Vector2(20,20), new Vector2(28,-2.5f), new Vector2(40f,-21), new Vector2(48,-12.5f)

		//		,new Vector2(-15,-10), new Vector2(-33.5f, -12.5f), new Vector2(-50, 11), new Vector2(-28.5f, 8),
		//		new Vector2(-18,26f), new Vector2(0,42), new Vector2(12,24f), new Vector2(33,3f),
		//		new Vector2(30,-16), new Vector2(53,-9f), new Vector2(-20,-15), new Vector2(-45f, -16f),
		//		new Vector2(-45, 6), new Vector2(-33.5f, 5), new Vector2(-13,23.5f), new Vector2(7,40),
		//		new Vector2(20,20), new Vector2(28,-2.5f), new Vector2(40f,-21), new Vector2(48,-12.5f)


		//		};

		//              // Spawn and position bats
		//		for (int i = 0; i < itemsToPool[2].amountToPool; i++)
		//		{
		//			GameObject tGO = ObjectPool.S.GetPooledObject("Bug_3");
		//			PosAndEnableObj(tGO, tPos[i]);
		//		}
		//		break;
		//}
	}
}
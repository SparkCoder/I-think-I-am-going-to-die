using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public enum OriginPara
{
    Center, NorthEast, NorthWest, SouthEast, SouthWest, RandomCorner, Random
}
public enum OffsetPara
{
    None, Closer, Further, Random
}

[System.Serializable]
public struct PrefabSet
{
    public GameObject Prefab;
    public Vector2 Size;
    public Vector2 StartPos;
    public Vector2 EndPos;
    public float probability;
    public bool UseCustomPos;
    public OriginPara Origin;
    public OffsetPara Offset;
}

public class Level : MonoBehaviour
{
    [Header("Level parts")]
    public Wall_Door Wall_N;
    public Wall_Door Wall_S;
    public Wall_Door Wall_E;
    public Wall_Door Wall_W;

    [Header("Obstacles")]
    public PrefabSet[] Sections;
    public PrefabSet[] Enemies;

    [HideInInspector]
    public bool Obstacle = false;
    [HideInInspector]
    public int start, end;

    private List<GameObject> obstacles;
    private List<GameObject> enemies;
    private float levelUnits;

    void Awake()
    {
        obstacles = new List<GameObject>();
        enemies = new List<GameObject>();
    }

    void Start()
    {
        levelUnits = transform.parent.gameObject.GetComponent<LevelGenerator>().LevelUnits;
    }

    void Update()
    {
        if (Obstacle)
        {
            GenerateObstacles();
            Obstacle = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (enemies.Count > 0)
            {
                foreach (GameObject obj in enemies)
                {
                    obj.GetComponent<Enemy>().Active = true;
                }
            }
            transform.parent.GetComponent<LevelGenerator>().CloseLevel(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (enemies.Count > 0)
            {
                foreach (GameObject obj in enemies)
                {
                    obj.GetComponent<Enemy>().Active = false;
                }
            }
            GameManager.Instance.Score(10.0f);
            transform.parent.GetComponent<LevelGenerator>().RemoveLevel(this);
        }
    }

    [Button]
    public void GenerateObstacles()
    {
        if (obstacles.Count > 0)
        {
            foreach (GameObject obj in obstacles)
            {
                Destroy(obj);
            }
            obstacles.Clear();
        }

        if (enemies.Count > 0)
        {
            foreach (GameObject obj in enemies)
            {
                Destroy(obj);
            }
            enemies.Clear();
        }

        for (int i = 0; i < 4; i++)
        {
            Wall_Door wall = GetWall(i);
            if (wall.Opened)
            {
                if (wall.Entrance)
                {
                    start = i;
                }
                else
                {
                    end = i;
                }
            }
        }

        int selection;
        PrefabSet sectionItem;
        GameObject item;
        GameObject obs;

        if (Lehmer.Next() > 0.5)
        {
            selection = Mathf.FloorToInt((float)Lehmer.Next() * Sections.Length);
            sectionItem = Sections[selection];

            item = sectionItem.Prefab;
            obs = Instantiate(item, transform);
            obs = OffsetPrefab(sectionItem, obs);

            obstacles.Add(obs);
        }

        if (Lehmer.Next() > 0.5)
        {
            selection = Mathf.FloorToInt((float)Lehmer.Next() * Enemies.Length);
            sectionItem = Enemies[selection];

            item = sectionItem.Prefab;
            obs = Instantiate(item, transform);
            obs = OffsetPrefab(sectionItem, obs);

            enemies.Add(obs);
        }
    }

    GameObject OffsetPrefab(PrefabSet sectionItem, GameObject obs)
    {
        Vector3 ori = obs.transform.position;
        Vector3 pos = ori;
        if (sectionItem.Origin != OriginPara.Center)
        {

            float xo = 0;
            float yo = 0;

            switch (sectionItem.Origin)
            {
                case OriginPara.NorthEast:
                    xo = levelUnits / 2;
                    yo = levelUnits / 2;
                    break;
                case OriginPara.NorthWest:
                    xo = -levelUnits / 2;
                    yo = levelUnits / 2;
                    break;
                case OriginPara.SouthEast:
                    xo = levelUnits / 2;
                    yo = -levelUnits / 2;
                    break;
                case OriginPara.SouthWest:
                    xo = -levelUnits / 2;
                    yo = -levelUnits / 2;
                    break;
                case OriginPara.RandomCorner:
                    xo = levelUnits / 2 * ((Lehmer.Next() > 0.5) ? -1 : 1);
                    yo = levelUnits / 2 * ((Lehmer.Next() > 0.5) ? -1 : 1);
                    break;
                case OriginPara.Random:
                    if (Lehmer.Next() > 0.5)
                    {
                        xo = levelUnits / 2 * ((Lehmer.Next() > 0.5) ? -1 : 1);
                        yo = levelUnits / 2 * ((Lehmer.Next() > 0.5) ? -1 : 1);
                    }
                    break;
            }

            float ao = (start == 1 || start == 3) ? yo : xo;
            float bo = (start == 1 || start == 3) ? xo : yo;

            switch (start)
            {
                case 1:
                    ao *= -1;
                    break;
                case 2:
                    ao *= -1;
                    bo *= -1;
                    break;
                case 3:
                    bo *= -1;
                    break;
            }

            pos.x += ao;
            pos.z += bo;
        }

        Vector2 d = Vector2.zero;

        if (sectionItem.UseCustomPos)
            d = (sectionItem.EndPos - sectionItem.StartPos);
        else
            d = (levelUnits * Vector2.one) - sectionItem.Size;

        float x = (int)(Lehmer.Next() * d.x);
        float y = (int)(Lehmer.Next() * d.y);

        float a = (start == 1 || start == 3) ? y : x;
        float b = (start == 1 || start == 3) ? x : y;

        switch (start)
        {
            case 1:
                a *= -1;
                break;
            case 2:
                a *= -1;
                b *= -1;
                break;
            case 3:
                b *= -1;
                break;
        }

        a -= ((pos - ori).x == 0.0f) ? a : (((pos - ori).x > 0) ? (sectionItem.Size.x / 2) : -(sectionItem.Size.y / 2));
        b -= ((pos - ori).z == 0.0f) ? b : (((pos - ori).z > 0) ? (sectionItem.Size.y / 2) : -(sectionItem.Size.y / 2));

        pos.x += a;
        pos.z += b;

        if (sectionItem.Offset != OffsetPara.None)
        {
            float offset = 4.0f;
            int dir = 0;

            switch (sectionItem.Offset)
            {
                case OffsetPara.Closer:
                    dir = 1;
                    break;
                case OffsetPara.Further:
                    dir = -1;
                    break;
                case OffsetPara.Random:
                    dir = (Lehmer.Next() > 0.5) ? -1 : 1;
                    break;
            }

            switch (start)
            {
                case 0:
                    pos.z -= offset * dir;
                    break;
                case 1:
                    pos.x += offset * dir;
                    break;
                case 2:
                    pos.z += offset * dir;
                    break;
                case 3:
                    pos.x -= offset * dir;
                    break;
            }
        }

        pos.y = 0.0f;
        obs.transform.position = pos;

        if (start == 1 || start == 3)
            obs.transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.up);

        return obs;
    }

    Wall_Door GetWall(int side)
    {
        switch (side)
        {
            case 0:
                return Wall_S;
            case 1:
                return Wall_E;
            case 2:
                return Wall_N;
            case 3:
                return Wall_W;
        }
        return null;
    }
}

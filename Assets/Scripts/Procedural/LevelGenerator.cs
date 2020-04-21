using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Parameters")]
    public int MaxSpawn = 5;
    public int CurrentLevel = 0;

    [Header("Assets")]
    public Level g_Level;
    public float LevelUnits = 10.0f;

    private List<Level> levels;
    private int levelCount = 0;

    private int side;
    private int level;
    private bool leveled;
    private int[] availableSides;
    private Level curLevel;
    private Vector3 pos;
    private bool firstWall = true;

    void Awake()
    {
        availableSides = new int[4];
        levels = new List<Level>();
    }

    void Start()
    {
        level = 0;
        leveled = true;
        pos = new Vector3(0.0f, 0.0f, 0.0f);
        curLevel = null;
    }

    void Update()
    {
        if (!GameManager.Instance.dead)
        {
            while (levelCount < MaxSpawn)
            {
                Level s_Level = Instantiate(g_Level.gameObject, transform).GetComponent<Level>();
                s_Level.name = "Level " + level;
                level++;

                if (curLevel != null)
                {
                    s_Level.Obstacle = true;
                    if (!firstWall)
                    {
                        int j = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            if (i != ((side + 2) % 4) && i != ((side + 2) % 4))
                            {
                                availableSides[j] = i;
                                j++;
                            }
                        }
                        side = availableSides[(int)(Lehmer.Next() * j)];
                    }
                    else
                    {
                        side = 2;
                        firstWall = false;
                    }

                    Wall_Door prev = GetWall(curLevel, side);
                    Wall_Door next = GetWall(s_Level, (side + 2) % 4);

                    prev.Opened = true;
                    next.Opened = true;
                    next.Entrance = true;

                    switch (side)
                    {
                        case 1:
                            pos.x += LevelUnits;
                            break;
                        case 2:
                            pos.z += LevelUnits;
                            break;
                        case 3:
                            pos.x -= LevelUnits;
                            break;
                    }
                }

                s_Level.transform.position = pos;
                curLevel = s_Level;

                levels.Add(s_Level);
                levelCount++;
            }
        }
    }

    public void RemoveLevel(Level lev)
    {
        levels.Remove(lev);
        Destroy(lev.gameObject);
        levelCount--;
        leveled = true;
    }

    public void CloseLevel(Level lev)
    {
        StartCoroutine(LevelCloser(lev));
    }

    IEnumerator LevelCloser(Level lev)
    {
        while (!leveled)
            yield return null;
        leveled = false;
        GetWall(lev, lev.start).Opened = false;
    }

    Wall_Door GetWall(Level level, int side)
    {
        switch (side)
        {
            case 0:
                return level.Wall_S;
            case 1:
                return level.Wall_E;
            case 2:
                return level.Wall_N;
            case 3:
                return level.Wall_W;
        }
        return null;
    }
}

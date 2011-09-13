using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;

namespace TowerDefense
{
    public class Pathfinding
    {
        int[][] map;
        List<Param> path;
        Point start, end;
        public Param Start { get { return path.Find(x => x.x == start.X && x.y == start.Y); } }
        public List<Param> Path { get { return path; } }

        private Pathfinding() { }

        public Param getNext(Point current)
        {
            Param p = path.Find(x => x.x == current.X && x.y == current.Y);
            if (p != null)
            {
                return p.parent;
            }
            return null;
        }
        public Param getCurrent(Point current)
        {
            Param p = path.Find(x => x.x == current.X && x.y == current.Y);
            return p;
        }

        public static Pathfinding createPath(int[][] map, Point start, Point end)
        {
            Pathfinding p = new Pathfinding();
            p.map = map;
            p.start = start;
            p.end = end;
            p.path = p.getNextMove(p.map, p.start, p.end);
            return p;
        }

        private List<Param> getNextMove(int[][] map, Point position, Point end)
        {
            List<Param> path = new List<Param>();
            Param start = new Param((int)end.X, (int)end.Y, 0, null);
            path.Add(start);
            int current = 0;
            Param cur = start;
            while (current != path.Count)
            {
                cur = path[current];
                List<Param> tmp = (getAdjacent(cur, map));
                foreach (Param p in path)
                {
                    tmp.RemoveAll(x => x.y == p.y && x.x == p.x && x.counter >= p.counter);
                }
                path.AddRange(tmp);
                current++;
            }
            return path;
        }

        private List<Param> getAdjacent(Param p, int[][] map)
        {
            List<Param> adjacent = new List<Param>();
            //Up
            if (p.y != 0 && map[p.y - 1][p.x] == 1)
                adjacent.Add(new Param(p.x, p.y - 1, p.counter + 1, p));
            //Down
            if (p.y + 1 < map.Length && map[p.y + 1][p.x] == 1)
                adjacent.Add(new Param(p.x, p.y + 1, p.counter + 1, p));
            //Left
            if (p.x != 0 && map[p.y][p.x - 1] == 1)
                adjacent.Add(new Param(p.x - 1, p.y, p.counter + 1, p));
            //Right
            if (p.x + 1 < map[0].Length && map[p.y][p.x + 1] == 1)
                adjacent.Add(new Param(p.x + 1, p.y, p.counter + 1, p));


            return adjacent;
        }
    }



    public class Param
    {
        public int x, y, counter;
        public Param parent;
        public Param(int x, int y, int counter, Param parent)
        {
            this.x = x;
            this.y = y;
            this.counter = counter;
            this.parent = parent;

        }
    }
}

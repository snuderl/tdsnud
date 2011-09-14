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
using SnudsLib;
using System.Xml;

namespace TowerDefense
{
    public class Level : GameComponent
    {
        public int CellSize { get { return cellSize; } }
        private int cellSize;
        public int[][] Map { get { return map; } }
        private int[][] map;
        public IObject[][] ObjectMap { get { return objectMap; } }
        private IObject[][] objectMap;
        public int Rows { get { return map.Length; } }
        public int Columns { get { return map[0].Length; } }
        public int[][] IntObjectMap { get { return createArrayMap(objectMap); } }
        private Point end;
        public Point End { get { return end; } }
        public bool finished = false;

        public Spawner spawner;

        public Pathfinding pathfinding;
        TowerDefense game;

        public TowerManager towerManager;

        public EnemyManager EnemyManager { get { return enemyManager; } }
        private EnemyManager enemyManager;
        private ProjectileManager projectileManager;
        public ProjectileManager ProjectileManager { get { return projectileManager; } }
        List<GameComponent> componentList;




        public Level(TowerDefense game, int cellSize, int rows, int columns, Point end, List<SpawnPoint> spawns)
            : base(game)
        {
            this.cellSize = cellSize;
            this.end = end;
            InitializeMap(rows, columns);
            this.game = game;
            componentList = new List<GameComponent>();

            towerManager = new TowerManager(game);
            AddComponent(towerManager);
            enemyManager = new EnemyManager(game);
            AddComponent(enemyManager);
            projectileManager = new ProjectileManager(game);
            AddComponent(projectileManager);
            spawner = new Spawner(game, enemyManager.enemies, spawns);
            AddComponent(spawner);

            pathfinding = Pathfinding.createPath(IntObjectMap, new Point(0,0), End);

        }

        private void AddComponent(GameComponent component)
        {
            game.Components.Add(component);
            componentList.Add(component);
        }

        public void Finished()
        {
            foreach (GameComponent c in componentList)
            {
                c.Enabled = false;
                game.Components.Remove(c);
            }
        }

        public void Pause()
        {
            foreach (GameComponent c in componentList)
            {
                c.Enabled = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (spawner.finished==true &&   enemyManager.enemies.Count==0)
            {
                finished = true;
            }
            base.Update(gameTime);
        }

        private void InitializeMap(int rows, int columns)
        {
            objectMap = new IObject[rows][];
            map = new int[rows][];
            for (int i = 0; i < map.Length; i++)
            {
                objectMap[i] = new IObject[columns];
            }
            for (int i = 0; i < map.Length; i++)
            {
                int[] tmp = new int[columns];
                for (int y = 0; y < tmp.Length; y++)
                {
                    tmp[y] = 1;
                }
                map[i] = tmp;
            }
            objectMap[end.Y][end.X] = new End();
        }



        private int[][] createArrayMap(IObject[][] i)
        {
            int[][] copy = new int[i.Length][];
            for (int y = 0; y < i.Length; y++)
            {
                copy[y] = new int[i[y].Length];
                for (int x = 0; x < i[y].Length; x++)
                {
                    if (i[y][x] == null || i[y][x].Walkable == true)
                    {
                        copy[y][x] = 1;
                    }
                    else
                    {
                        copy[y][x] = 0;
                    }
                }
            }
            return copy;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    spriteBatch.Draw(game.grass, new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize), Color.White);
                }
            }

            towerManager.Draw(gameTime, spriteBatch);
            EnemyManager.Draw(gameTime, spriteBatch);
            ProjectileManager.Draw(gameTime, spriteBatch);



            spriteBatch.DrawString(game.sf, "X: " + Mouse.GetState().X + " Y: "+"  Money: " +game.money+"  Score: " + game.score + "!"+ "  Lives: "+game.Lives, new Vector2(0, 0), Color.White);
            if (finished == true)
            {
                spriteBatch.DrawString(game.sf, "Level blablabla...", new Vector2(400, 400), Color.White);
            
            }
        }

    }
}

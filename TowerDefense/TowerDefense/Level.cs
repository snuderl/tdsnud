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

namespace TowerDefense
{
    public class Level : GameComponent
    {
        public int money = 2000;
        public int CellSize { get { return cellSize; } }
        private int cellSize;
        public int[][] Map { get { return map; } }
        private int[][] map;
        public IObject[][] ObjectMap { get { return objectMap; } }
        private IObject[][] objectMap;
        public int Rows { get { return map.Length; } }
        public int Columns { get { return map[0].Length; } }
        public int[][] IntObjectMap { get { return createArrayMap(objectMap); } }
        private Point start;
        private Point end;
        public Point Start { get { return start; } }
        public Point End { get { return end; } }
        private Spawner spawner;

        public Pathfinding pathfinding;
        TowerDefense game;

        public TowerManager towerManager;

        public EnemyManager EnemyManager { get { return enemyManager; } }
        private EnemyManager enemyManager;
        private ProjectileManager projectileManager;
        public ProjectileManager ProjectileManager { get { return projectileManager; } }

        public Level(TowerDefense game, int cellSize, int rows, int columns, Point start, Point end) : base(game)
        {
            this.cellSize = cellSize;
            this.start = start;
            this.end = end;
            InitializeMap(rows, columns);
            this.game = game;

            towerManager = new TowerManager(game);

            game.Components.Add(towerManager);
            enemyManager = new EnemyManager(game);
            game.Components.Add(enemyManager);
            projectileManager = new ProjectileManager(game);
            game.Components.Add(projectileManager);
            spawner = new Spawner(game, enemyManager.enemies);
            game.Components.Add(spawner);


            pathfinding = Pathfinding.createPath(IntObjectMap, Start, End);
        }

        public void Loaded()
        {


            SpawnPoint sp = new SpawnPoint(3, 1, 5, new Enemy(new Vector2(0,300), 100, "Creep1", new AnimatedSprite(game.enemy, 32, 32, 3), 32, 0, 100));
            spawner.AddSpawnPoint(sp);
            sp = new SpawnPoint(3, 1, 5, new Enemy(new Vector2(0, 100), 100, "Creep1", new AnimatedSprite(game.enemy, 32, 32, 3), 32, 0, 100));
            spawner.AddSpawnPoint(sp);
            towerManager.Load();
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


            spriteBatch.DrawString(game.sf, "Score: " + game.score + "!", new Vector2(300, 0), Color.White);
            spriteBatch.DrawString(game.sf, "X: " + Mouse.GetState().X + " Y: " + Mouse.GetState().Y, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(game.sf, "Money: " +money, new Vector2(150, 0), Color.White);
        }

    }
}

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

        public Spawner spawner;

        public Pathfinding pathfinding;
        TowerDefense game;
        public float elapsed;
        public int Id { get; set; }

        public TowerManager towerManager;

        public EnemyManager EnemyManager { get { return enemyManager; } }
        private EnemyManager enemyManager;
        private ProjectileManager projectileManager;
        public ProjectileManager ProjectileManager { get { return projectileManager; } }
        List<GameComponent> componentList;

        public Tower Building { get { return towerManager.towerList[buildingTower]; } }
        int buildingTower = 0;
        public TimeSpan pauseStart;
        public TimeSpan totalPauseTime;
        public bool Won { get; set; }
        public bool Lost { get; set; }

        public bool finished = false;
        public bool Paused { get; set; }

        public enum Mode
        {
            normal, build
        }
        Mode mode = Mode.normal;


        float sinceTowerChange = 0;


        IGui selected;




        public Level(TowerDefense game, int cellSize, int rows, int columns, Point end, List<SpawnPoint> spawns,int id)
            : base(game)
        {
            this.cellSize = cellSize;
            this.end = end;
            this.Id = id;
            InitializeMap(rows, columns);
            Lost = false;
            Won = false;
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
            Paused = false;

            pathfinding = Pathfinding.createPath(IntObjectMap, new Point(0, 0), End);

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

        public void Pause(GameTime gameTime)
        {
            foreach (GameComponent c in componentList)
            {
                c.Enabled = false;

            }
            Paused = true;
            pauseStart = gameTime.TotalGameTime;
        }

        public void UnPause(GameTime gameTime)
        {
            foreach (GameComponent c in componentList)
            {
                c.Enabled = true;
            }
            Paused = false;
            totalPauseTime = gameTime.TotalGameTime - pauseStart;
        }

        public override void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (game.Lives <= 0)
            {
                Lost = true;
                finished=true;
            }
            else if(spawner.finished == true && enemyManager.enemies.Count == 0){
                Won=true;
                finished=true;
            }
            if (!finished)
            {
                KeyboardState ks = Keyboard.GetState();
                MouseState ms = Mouse.GetState();
                if (ks.IsKeyDown(Keys.Space))
                {
                    Pause(gameTime);
                }
                if (ks.IsKeyDown(Keys.O))
                {
                    UnPause(gameTime);
                }
                if (!Paused)
                {
                 

                    sinceTowerChange += elapsed;
                    if (mode == Mode.build)
                    {
                        if (ButtonState.Pressed == ms.LeftButton)
                        {
                            int x, y;
                            x = ms.X - (ms.X % 48);
                            y = ms.Y - (ms.Y % 48);
                            if (ObjectMap[y / 48][x / 48] == null && !EnemyManager.isEnemyOnIt(new Point(x / 48, y / 48)) && game.money >= Building.cost)
                            {
                                towerManager.Build(new Vector2(x, y), Building);
                            }
                        }
                        if (sinceTowerChange > 0.1f && ks.IsKeyDown(Keys.Add))
                        {
                            buildingTower++;
                            if (buildingTower == towerManager.towerList.Count)
                                buildingTower = 0;

                            sinceTowerChange = 0;
                        }
                    }
                    else if (mode == Mode.normal && ButtonState.Pressed == ms.LeftButton)
                    {
                        Rectangle mouse = new Rectangle(ms.X - 2, ms.Y - 2, 4, 4);
                        foreach (Enemy e in EnemyManager.enemies)
                        {
                            Rectangle rec = e.destinationRectangle;
                            if (mouse.Intersects(rec))
                            {
                                selected = e;
                            }
                        }
                        foreach (Tower t in towerManager.towers)
                        {
                            Rectangle rec = t.DestinationRectangle;
                            if (mouse.Intersects(rec))
                            {
                                selected = t;
                            }
                        }
                    }

                    if (ks.IsKeyDown(Keys.B))
                    {
                        mode = Mode.build;
                    }
                    if (ks.IsKeyDown(Keys.N))
                    {
                        mode = Mode.normal;
                    }

                }

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
            spriteBatch.Draw(game.boxT, new Vector2(end.X * 48, end.Y * 48), Color.White);

            towerManager.Draw(gameTime, spriteBatch);
            EnemyManager.Draw(gameTime, spriteBatch);
            ProjectileManager.Draw(gameTime, spriteBatch);

            if (!Paused)
            {
                if (mode == Mode.build)
                {
                    MouseState ms = Mouse.GetState();
                    int x, y;
                    x = ms.X - (ms.X % 48);
                    y = ms.Y - (ms.Y % 48);
                    int xCoord, yCoord;
                    xCoord = x / 48;
                    yCoord = y / 48;
                    if (!(xCoord >= ObjectMap[0].Length || xCoord < 0 || yCoord < 0 || yCoord >= ObjectMap.Length))
                    {
                        Rectangle dest = Building.DestinationRectangle;
                        dest.Offset(x, y);
                        Color c = Color.White;
                        if (ObjectMap[yCoord][xCoord] == null && !EnemyManager.isEnemyOnIt(new Point(x / 48, y / 48)) && towerManager.towerList[buildingTower].cost <= game.money)
                        {
                            c = Color.LightSeaGreen;
                        }
                        else
                        {
                            c = Color.Red;
                        }

                        spriteBatch.Draw(Building.s.Texture, dest, Building.s.SourceRec, c * 0.3f);
                    }
                    spriteBatch.DrawString(game.sf, "Name: " + towerManager.towerList[buildingTower].Name + "\nCost: " + towerManager.towerList[buildingTower].cost + "\nRange: " + towerManager.towerList[buildingTower].range + "\nAtackSpeed: " + towerManager.towerList[buildingTower].shootSpeed, new Vector2(0, 400), Color.White);
                }
                else
                {
                    if (selected != null)
                    {
                        selected.Draw(gameTime, spriteBatch, game.sf);
                    }
                }


                spriteBatch.DrawString(game.sf, "X: " + Mouse.GetState().X + " Y: " + "  Money: " + game.money + "  Score: " + game.score + "!" + "  Lives: " + game.Lives, new Vector2(0, 0), Color.White);
            }
            if (finished == true)
            {
                String text = "";
                if (Won)
                {
                    text = "Level completed!";
                }
                else
                {
                    text = "You suck!";
                }
                spriteBatch.DrawString(game.sf, text, new Vector2(300, 300), Color.White);
            }
        }

    }
}

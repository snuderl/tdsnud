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

        #region Fields
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
        private Camera camera;
        public Camera Camera { get { return camera; } }
        List<GameComponent> componentList;

        public Tower Building { get { return towerManager.towerList[buildingTower]; } }
        int buildingTower = 0;
        public TimeSpan pauseStart;
        public TimeSpan totalPauseTime;
        public bool Won { get; set; }
        public bool Lost { get; set; }

        public bool finished = false;
        public bool Paused { get; set; }
        SoundEffectInstance se;

        GridTexture grid;

        public enum Mode
        {
            normal, build
        }
        Mode mode = Mode.normal;


        float sinceTowerChange = 0;


        IGui selected;

        #endregion
       
        #region Methods


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
        private void AddComponent(GameComponent component)
        {
            game.Components.Add(component);
            componentList.Add(component);
        }

        public void Start()
        {
            AddComponent(towerManager);
            AddComponent(enemyManager);
            AddComponent(projectileManager);
            AddComponent(spawner);
            game.Components.Add(camera);
            spawner.Reset();
            se = game.music.CreateInstance();
            se.IsLooped = true;
            se.Play();
            grid = new GridTexture(game,camera, 48);
            game.Components.Add(grid);
        }

        public void Finished()
        {
            foreach (GameComponent c in componentList)
            {
                c.Enabled = false;
                game.Components.Remove(c);
            }
            game.Components.Remove(camera);
            game.Components.Remove(grid);
            game.Components.Remove(this);
            se.Stop(true);
        }

        /// <summary>
        /// Pauses the game
        /// </summary>
        public void Pause()
        {
            foreach (GameComponent c in componentList)
            {
                c.Enabled = false;

            }
            Paused = true;
            se.Pause();
        }



        /// <summary>
        /// Resumes the game
        /// </summary>
        /// <param name="gameTime"></param>
        public void UnPause()
        {
            foreach (GameComponent c in componentList)
            {
                c.Enabled = true;
            }
            Paused = false;
            se.Resume();
        }
        #endregion

        private Enemy isEnemyOnLocation(Rectangle selected)
        {
            foreach (Enemy e in EnemyManager.enemies)
            {
                Rectangle rec = e.destinationRectangle;
                if (selected.Intersects(rec))
                {
                    return e;
                }
            }
            return null;
        }

        #region Override
        public override void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (game.Lives <= 0)
            {
                Lost = true;
                finished = true;
            }
            else if (spawner.finished == true && enemyManager.enemies.Count == 0)
            {
                Won = true;
                finished = true;
            }
            if (!finished)
            {
                KeyboardState ks = Keyboard.GetState();
                MouseState ms = Mouse.GetState();
                if (ks.IsKeyDown(Keys.Space))
                {
                    Pause();
                }
                if (ks.IsKeyDown(Keys.O))
                {
                    UnPause();
                }
                if (!Paused)
                {


                    sinceTowerChange += elapsed;
                    if (mode == Mode.build)
                    {
                        if (ButtonState.Pressed == ms.LeftButton)
                        {
                            int x, y;
                            x = ms.X - camera.PointPosition.X - ((ms.X - camera.PointPosition.X) % 48);
                            y = ms.Y - camera.PointPosition.Y - ((ms.Y - camera.PointPosition.Y) % 48);

                            int xCoord, yCoord;
                            xCoord = x / 48;
                            yCoord = y / 48;
                            //Preveri ce se nahaja na mapi
                            if (xCoord >= 0 && (xCoord) <= (map[0].Length-1) && yCoord >= 0 && (yCoord) <= (map.Length-1))
                            {
                                Point mousePosition = new Point(xCoord, yCoord);
                                if (ObjectMap[mousePosition.Y][mousePosition.X] == null && !EnemyManager.isEnemyOnIt(mousePosition) && game.money >= Building.cost)
                                {
                                    towerManager.Build(new Vector2(x, y), Building);
                                }
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
                        selected = isEnemyOnLocation(mouse);

                        if (selected == null)
                        {
                            foreach (Tower t in towerManager.towers)
                            {
                                Rectangle rec = t.DestinationRectangle;
                                if (mouse.Intersects(rec))
                                {
                                    selected = t;
                                }
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    Rectangle dest = camera.offset(new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize));
                    Rectangle source = game.sourceRectDict["5"];
                    if (x == 0 && y==0)
                    {
                        source = game.sourceRectDict["1"];
                    }
                    else if (x == (map[0].Length - 1) && y == (map.Length - 1))
                    {
                        source = game.sourceRectDict["9"];
                    }
                    else if (x == 0 && y == (map.Length - 1))
                    {
                        source = game.sourceRectDict["7"];
                    }
                    else if (x == (map[0].Length - 1) && y == 0)
                    {
                        source = game.sourceRectDict["3"];
                    }
                    else
                    {
                        if (x == 0)
                        {
                            source = game.sourceRectDict["4"];
                        }
                        else if (y == 0)
                        {
                            source = game.sourceRectDict["2"];
                        }
                        else if (y == (map.Length - 1))
                        {
                            source = game.sourceRectDict["8"];
                        }
                        else if (x == (map[0].Length - 1))
                        {
                            source = game.sourceRectDict["6"];
                        }
                    }
                    spriteBatch.Draw(game.mountain, dest,source, Color.White);
                }
            }
            spriteBatch.Draw(game.boxT, camera.offset(new Vector2(end.X * 48, end.Y * 48)), Color.White);

            towerManager.Draw(gameTime, spriteBatch);
            EnemyManager.Draw(gameTime, spriteBatch);
            ProjectileManager.Draw(gameTime, spriteBatch);
            spawner.Draw(spriteBatch);

            grid.Draw(gameTime, spriteBatch);

            if (!Paused)
            {
                if (mode == Mode.build)
                {
                    MouseState ms = Mouse.GetState();
                    int x, y;
                    x = ms.X - camera.PointPosition.X - ((ms.X - camera.PointPosition.X) % 48);
                    y = ms.Y- camera.PointPosition.Y-((ms.Y-camera.PointPosition.Y) % 48);
                    
                    int xCoord, yCoord;
                    xCoord = x / 48;
                    yCoord = y / 48;
                    if (!(xCoord >= ObjectMap[0].Length || xCoord < 0 || yCoord < 0 || yCoord >= ObjectMap.Length))
                    {
                        Rectangle dest = Building.DestinationRectangle;
                        dest.Offset(xCoord * 48, yCoord*48);

                        dest = camera.offset(dest);
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
                        PrimitiveLine pl = new PrimitiveLine(game.GraphicsDevice);
                        pl.CreateCircle(selected.radius + 5, 40);
                        pl.Position = selected.center;
                        pl.Render(spriteBatch);
                    }
                    MouseState ms = Mouse.GetState();
                    Enemy e = isEnemyOnLocation(new Rectangle(ms.X, ms.Y, 6, 6));
                    if (e != null)
                    {

                        //Draw circle around targeted
                        PrimitiveLine pl = new PrimitiveLine(game.GraphicsDevice);
                        pl.CreateCircle(e.radius + 5, 40);
                        pl.Position = e.center;
                        pl.Render(spriteBatch);
                        DrawHealthBar(spriteBatch, e, new Point(ms.X, ms.Y), 40, 10);
                    }
                }


                spriteBatch.DrawString(game.sf, "X: " + Mouse.GetState().X + " Y: "+Mouse.GetState().Y + "  Money: " + game.money + "  Score: " + game.score + "!" + "  Lives: " + game.Lives, new Vector2(0, 0), Color.White);
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

        public void DrawHealthBar(SpriteBatch spriteBatch, Enemy enemy, Point position, int width, int height)
        {
            float heightMultiplier = (float)height / game.health.Height;
            float widthMuliplier = (float)44 / game.health.Width;
            //Draw the negative space for the health bar
            spriteBatch.Draw(game.health, new Rectangle(position.X,
    position.Y, (int)(game.health.Width * widthMuliplier), (int)(44 * heightMultiplier)), new Rectangle(0, 45, game.health.Width, 44), Color.Gray);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(game.health, new Rectangle(position.X,
                 position.Y, (int)(game.health.Width * widthMuliplier * ((double)enemy.health / enemy.MaxHealth)), (int)(44 * heightMultiplier)),
                 new Rectangle(0, 45, game.health.Width, 44), Color.Red);

            //Draw the box around the health bar
            spriteBatch.Draw(game.health, new Rectangle(position.X,
                position.Y, (int)(game.health.Width *widthMuliplier), (int)(44 * heightMultiplier)), new Rectangle(0, 0, game.health.Width, 44), Color.White);
        }



        public Level(TowerDefense game, int cellSize, int rows, int columns, Point end, List<Wave> waves, int id)
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
            enemyManager = new EnemyManager(game);
            projectileManager = new ProjectileManager(game);
            spawner = new Spawner(game, enemyManager.enemies, waves);
            camera = new Camera(game, Vector2.Zero, columns*cellSize, rows*cellSize);
            Paused = false;


            pathfinding = Pathfinding.createPath(IntObjectMap, new Point(0, 0), End);

        }
        #endregion
    }
}

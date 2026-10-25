using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HillClimber2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        int screenWidth;
        int screenHeight;
        int xAxisLength;
        int yAxisLength;

        List<Vector2> drawingPoints;
        List<Vector2> graphPoints;


        double m;
        double b;

        Vector2 endPoint;
        Vector2 startPoint;
        Random random = new Random();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            drawingPoints = new List<Vector2>();
            graphPoints = new List<Vector2>();
            m = (random.NextDouble() - 0.5) * 40;
            b = (random.NextDouble() - 0.5) * 40;
        }

        static double GetGraphValue(double x, double m, double b)
        {
            return m * x + b;
        }

        static double GetError(List<Vector2> points, double m, double b)
        {
            double error = 0;
            foreach (var point in points)
            {
                double x = point.X;
                double y = point.Y;
                double predictedY = GetGraphValue(x, m, b);
                error += Math.Pow(y - predictedY, 2);
            }
            return error / points.Count;
        }

        static void Mutate(ref double m, ref double b, double error, Random random)
        {
            m += (random.NextDouble() - 0.5) * error;
            b += (random.NextDouble() - 0.5) * error;
        }

        static void CreateLineOfBestFit(List<Vector2> points, ref double m, ref double b, Random random)
        {
            for (int i = 0; i < 10000; i++)
            {
                double oldM = m;
                double oldB = b;
                double oldError = GetError(points, oldM, oldB);
                Mutate(ref m, ref b, oldError, random);
                double mutatedError = GetError(points, m, b);
                if (mutatedError < oldError)
                {
                    oldM = m;
                    oldB = b;
                }
                else
                {
                    m = oldM;
                    b = oldB;
                }
            }
           // Console.WriteLine(m + " " + b);
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: Add your initialization logic here
            screenWidth = 1000;
            screenHeight = 1000;
            xAxisLength = 40;
            yAxisLength = 40;






            base.Initialize();
        }

        protected override void LoadContent()
        {
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Vector2 clickPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                drawingPoints.Add(clickPosition);

                Vector2 graphPosition = new Vector2(
                    (clickPosition.X / screenWidth) * xAxisLength - xAxisLength / 2,
                    (clickPosition.Y / screenHeight) * yAxisLength - yAxisLength / 2
                );
                graphPoints.Add(graphPosition);
                CreateLineOfBestFit(graphPoints, ref m, ref b, random);
                startPoint = new Vector2(0, ((float)GetGraphValue(-xAxisLength / 2, m, b) + xAxisLength / 2) * (screenHeight / yAxisLength));
                endPoint = new Vector2((float)xAxisLength * (screenWidth/ xAxisLength), ((float)GetGraphValue(xAxisLength / 2, m, b) + yAxisLength / 2) * (screenHeight / yAxisLength));
            }



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            // TODO: Add your drawing code here
            spriteBatch.DrawLine(new Vector2(screenWidth / 2, 0), new Vector2(screenWidth / 2, screenHeight), Color.Black);
            spriteBatch.DrawLine(new Vector2(0, screenHeight / 2), new Vector2(screenWidth, screenHeight / 2), Color.Black);

            for (int i = 0; i <= xAxisLength; i++)
            {
                float x = (float)i * (screenWidth / (float)xAxisLength);
                spriteBatch.DrawLine(new Vector2(x, screenHeight / 2 - 5), new Vector2(x, screenHeight / 2 + 5), Color.Black);
                spriteBatch.DrawString(Content.Load<SpriteFont>("Arial"), (i - xAxisLength / 2).ToString(), new Vector2(x, screenHeight / 2 + 10), Color.Black);
            }
            for (int i = 0; i <= yAxisLength; i++)
            {
                float y = (float)i * (screenHeight / (float)yAxisLength);
                spriteBatch.DrawLine(new Vector2(screenWidth / 2 - 5, y), new Vector2(screenWidth / 2 + 5, y), Color.Black);
                spriteBatch.DrawString(Content.Load<SpriteFont>("Arial"), (i - yAxisLength / 2).ToString(), new Vector2(screenWidth / 2 + 10, y), Color.Black);
            }
            for (int i = 0; i < drawingPoints.Count; i++)
            {
                Vector2 p = drawingPoints[i];
                spriteBatch.DrawPoint(p, Color.Red, 5);
            }

            if(startPoint != null && endPoint != null)
            {
                spriteBatch.DrawLine(startPoint, endPoint, Color.Green, 2);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

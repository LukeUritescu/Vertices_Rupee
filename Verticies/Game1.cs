using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;

namespace Vertices
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        VertexBuffer vertexBuffer;                              //Buffer for triangle
        VertexDeclaration basicEffectVertexDeclaration;         //

        Matrix worldMatrix;                                     //Matrix to hold world
        Matrix viewMatrix;                                      //View is the view from the camera
        Matrix projectionMatrix;                                //Projection is the 2D flattened view with occusion

        BasicEffect effect;                                //ShaderEffect used to draw on video card this is a simple mongame HLSL shader

        //World Transform variables
        #region World Tranfom variables
        float rotationX, orbitX;        //used to rotate and orbit objects
        float rotationY, orbitY;
        Vector3 worldTrans;             //translate is moving an object
        float worldScale;               //scales object in the work
        #endregion


        #region GameServices
        InputHandler input;
        GameConsole gameConsole;
        FPS fps;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Setup intial values for World Transform objects
            rotationX = 0.0f;
            rotationY = 0.0f;
            orbitX = 0.0f;
            orbitY = 0.0f;
            worldTrans = Vector3.Zero;
            worldScale = 1.0f;

            //Game components from MonogameLibrary.Util
            input = new InputHandler(this);
            gameConsole = new GameConsole(this);
            fps = new FPS(this);
            this.Components.Add(input);
            this.Components.Add(gameConsole);
            this.Components.Add(fps);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set up the initial View Matrixes
            // camera
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, -2, 20), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height,
                1.0f, 100.0f);

            gameConsole.GameConsoleWrite("Translate");
            gameConsole.GameConsoleWrite("w: y+ s:y- a:x- d:x+");
            gameConsole.GameConsoleWrite("Rotate");
            gameConsole.GameConsoleWrite("up down left right");

            gameConsole.GameConsoleWrite("+:scale up");
            gameConsole.GameConsoleWrite("-:scale down");
            gameConsole.GameConsoleWrite("r reset the triangle");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load any ResourceManagementMode.Automatic content
            this.InitializeEffect();
            this.SetUpVertices();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Elapsed time since last update
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //Identity
            worldMatrix = Matrix.Identity;
            //Manipulate world this changes the entire world matrix
            #region worldMatrix
            //Scale
            worldMatrix *= Matrix.CreateScale(worldScale);

            //Rotation
            worldMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(rotationX));
            worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(rotationY));


            //Translation
            worldMatrix *= Matrix.CreateTranslation(worldTrans);

            //Orbit
            worldMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(orbitX));
            worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(orbitY));

            #endregion

            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            #region Input

            //Scale
            if (input.KeyboardState.IsKeyDown(Keys.OemPlus))
            {
                worldScale += (0.001f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.OemMinus))
            {
                worldScale -= (0.001f * time);
            }

            // Rotation
            if (input.KeyboardState.IsKeyDown(Keys.Left))
            {
                rotationX += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Right))
            {
                rotationX -= (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Up))
            {
                rotationY += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Down))
            {
                rotationY -= (0.5f * time);
            }

            //Orbit
            if (input.KeyboardState.IsKeyDown(Keys.Q))
            {
                orbitX += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.E))
            {
                orbitX -= (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.T))
            {
                orbitY += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.G))
            {
                orbitY -= (0.5f * time);
            }



            //Translation
            if (input.KeyboardState.IsKeyDown(Keys.D))
            {
                worldTrans += new Vector3(0.01F * time, 0, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.A))
            {
                worldTrans -= new Vector3(0.01F * time, 0, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.W))
            {
                worldTrans += new Vector3(0, 0.01f * time, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.S))
            {
                worldTrans -= new Vector3(0, 0.01F * time, 0);
            }


            if (input.KeyboardState.HasReleasedKey(Keys.R))
            {
                //reset world matrix
                rotationX = 0.0f;
                rotationY = 0.0f;
                orbitX = 0.0f;
                orbitY = 0.0f;
                worldTrans = Vector3.Zero;
                worldScale = 1.0f;
            }


            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //graphics.GraphicsDevice.VertexDeclaration = basicEffectVertexDeclaration;
            //graphics.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);



            // This code would go between a device
            // BeginScene-EndScene block.
            //effect.Begin();


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 110);
                //pass.End();
            }
            //effect.End();

            base.Draw(gameTime);
        }

        private void SetUpVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[110];
            vertices[0] = new VertexPositionColor(new Vector3(-3, 2, 0), Color.Black);
            vertices[1] = new VertexPositionColor(new Vector3(-3, -1, 0), Color.Green);
            vertices[2] = new VertexPositionColor(new Vector3(0, -1, 0), Color.Black);

            vertices[3] = vertices[2];
            vertices[4] = new VertexPositionColor(new Vector3(0, 2, 0), Color.Black);
            vertices[5] = vertices[0];
            
            vertices[6] = new VertexPositionColor(new Vector3(-1.5f, 5, 0), Color.Black);
            vertices[7] = vertices[0];
            vertices[8] = vertices[4];
            
            vertices[9] = new VertexPositionColor(new Vector3(-1.5f, -4, 0), Color.Black);
            vertices[10] = vertices[1];
            vertices[11] = vertices[2];

            vertices[12] = new VertexPositionColor(new Vector3(-2.5f, 1.5f, 1), Color.Black);
            vertices[13] = new VertexPositionColor(new Vector3(-2.5f, -0.5f, 1), Color.Black);
            vertices[14] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, 1), Color.LimeGreen);

            vertices[15] = vertices[14];
            vertices[16] = new VertexPositionColor(new Vector3(-0.5f, 1.5f, 1), Color.Green);
            vertices[17] = vertices[12];
            
            vertices[18] = new VertexPositionColor(new Vector3(-1.5f, 3, 1), Color.Green);
            vertices[19] = vertices[12];
            vertices[20] = vertices[16];

            vertices[21] = new VertexPositionColor(new Vector3(-1.5f, -2, 1), Color.Green);
            vertices[22] = vertices[13];
            vertices[23] = vertices[14];

            vertices[24] = vertices[12];
            vertices[25] = vertices[6];
            vertices[26] = vertices[18];

            vertices[27] = vertices[0];
            vertices[28] = vertices[12];
            vertices[29] = vertices[6];

            vertices[30] = vertices[0];
            vertices[31] = vertices[13];
            vertices[32] = vertices[12];

            vertices[33] = vertices[0];
            vertices[34] = vertices[1];
            vertices[35] = vertices[13];

            vertices[36] = vertices[1];
            vertices[37] = vertices[13];
            vertices[38] = vertices[9];

            vertices[39] = vertices[21];
            vertices[40] = vertices[13];
            vertices[41] = vertices[9];

            vertices[42] = vertices[6];
            vertices[43] = vertices[18];
            vertices[44] = vertices[16];

            vertices[45] = vertices[6];
            vertices[46] = vertices[16];
            vertices[47] = vertices[4];

            vertices[48] = vertices[9];
            vertices[49] = vertices[21];
            vertices[50] = vertices[14];

            vertices[51] = vertices[9];
            vertices[52] = vertices[14];
            vertices[53] = vertices[2];

            vertices[54] = vertices[4];
            vertices[55] = vertices[2];
            vertices[56] = vertices[14];

            vertices[57] = vertices[16];
            vertices[58] = vertices[14];
            vertices[59] = vertices[4];

            vertices[60] = new VertexPositionColor(new Vector3(-2.5f, 1.5f, -1), Color.Green);
            vertices[61] = new VertexPositionColor(new Vector3(-2.5f, -0.5f, -1), Color.LimeGreen);
            vertices[62] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, -1), Color.DarkGreen);

            vertices[63] = vertices[62];
            vertices[64] = new VertexPositionColor(new Vector3(-0.5f, 1.5f, -1), Color.LimeGreen);
            vertices[65] = vertices[60];

            vertices[66] = new VertexPositionColor(new Vector3(-1.5f, 3, -1), Color.DarkGreen);
            vertices[67] = vertices[60];
            vertices[68] = vertices[64];

            vertices[69] = new VertexPositionColor(new Vector3(-1.5f, -2, -1), Color.Green);
            vertices[70] = vertices[61];
            vertices[71] = vertices[62];

            vertices[72] = vertices[60];
            vertices[73] = vertices[6];
            vertices[74] = vertices[66];

            vertices[75] = vertices[0];
            vertices[76] = vertices[60];
            vertices[77] = vertices[6];

            vertices[78] = vertices[0];
            vertices[79] = vertices[61];
            vertices[80] = vertices[60];

            vertices[81] = vertices[0];
            vertices[82] = vertices[1];
            vertices[83] = vertices[61];

            vertices[84] = vertices[1];
            vertices[85] = vertices[61];
            vertices[86] = vertices[9];

            vertices[87] = vertices[69];
            vertices[88] = vertices[61];
            vertices[89] = vertices[9];

            vertices[90] = vertices[6];
            vertices[91] = vertices[66];
            vertices[92] = vertices[64];

            vertices[93] = vertices[6];
            vertices[94] = vertices[64];
            vertices[95] = vertices[4];

            vertices[96] = vertices[9];
            vertices[97] = vertices[69];
            vertices[98] = vertices[62];

            vertices[99] = vertices[9];
            vertices[100] = vertices[62];
            vertices[101] = vertices[2];

            vertices[102] = vertices[4];
            vertices[103] = vertices[2];
            vertices[104] = vertices[62];

            vertices[105] = vertices[64];
            vertices[106] = vertices[62];
            vertices[107] = vertices[4];

            //vertexBuffer = new VertexBuffer(this.graphics.GraphicsDevice, 
            //       VertexPositionColor.SizeInBytes * vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor),
                vertices.Length, BufferUsage.WriteOnly | BufferUsage.None);
            vertexBuffer.SetData(vertices);
        }

        private void InitializeEffect()
        {
            //basicEffectVertexDeclaration = new VertexDeclaration(
            //    graphics.GraphicsDevice, VertexPositionColor.VertexElements);

            basicEffectVertexDeclaration = new VertexDeclaration(
                VertexPositionTexture.VertexDeclaration.GetVertexElements());

            //basicEffect = new BasicEffect(graphics.GraphicsDevice, null);
            effect = new BasicEffect(GraphicsDevice);
            effect.Alpha = 1.0f;
            effect.VertexColorEnabled = true;
        }
    }
}

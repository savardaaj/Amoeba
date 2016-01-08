using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amoeba
{
    public partial class FormView : Form
    {

        enum Position
        {
            Up, Down, Left, Right, UpRightDiagonal, UpLeftDiagonal, DownLeftDiagonal, DownRightDiagonal, Stop
        }

        private struct Player
        {
            public Rectangle playerBox;
            public int x;
            public int y;
            public int height;
            public int width;
            public Position position;
        }
        private Player player;
        
        private int foodObject_x;
        private int foodObject_y;
        string[] foodColors = new string[6] { "Red", "Blue", "Green", "Yellow", "Purple", "Orange" };

        private Rectangle[] foodBlocks;
        private FoodBlock[] foodStructs;
        private Brush randomFoodColor;
        Random randomObjectPlacement;

        private int createCount;
        private int indexCount;

        private struct FoodBlock
        {
            public int x;
            public int y;
            public int height;
            public int width;
            public Brush color;
            public Rectangle obj;
        }

        public FormView()
        {
            InitializeComponent();

            //For the random values
            randomObjectPlacement = new Random();

            //Array of Structs
            foodStructs = new FoodBlock[100];

            //Array of Rectangles
            foodBlocks = new Rectangle[100];

            player = new Player();

            player.playerBox = new Rectangle();
            player.x = 50;
            player.y = 50;
            player.height = 50;
            player.width = 50;
            player.position = Position.Down;
        }
        
        private void FormView_Paint(object sender, PaintEventArgs e)
        {

            //Draw player to screen
            e.Graphics.FillRectangle(Brushes.BlueViolet, player.x, player.y, player.height, player.width);

            //CreateCount sets how fast the foods will refresh *for now
            if (createCount > 10)
            {
                //Create a random placement and color for food block
                foodObject_x = randomObjectPlacement.Next(0, 758);
                foodObject_y = randomObjectPlacement.Next(0, 1014);
                randomFoodColor = new SolidBrush(Color.FromName(foodColors[randomObjectPlacement.Next(0, 6)]));

                //Set those randomly created values in the struct
                foodStructs[indexCount].x = foodObject_x;
                foodStructs[indexCount].y = foodObject_y;
                foodStructs[indexCount].color = randomFoodColor;
                foodStructs[indexCount].width = 10;
                foodStructs[indexCount].height = 10;

                Rectangle food = new Rectangle(foodStructs[indexCount].x, foodStructs[indexCount].y, foodStructs[indexCount].width, foodStructs[indexCount].height);
                //Store into array of FoodBlocks struct
                foodStructs[indexCount].obj = food;

                indexCount++;

                if (indexCount == 20)
                {
                    indexCount = 0;
                }
                createCount = 0;
            }
            else
            {
                createCount++;
            }



            for (int i = 0; i < foodStructs.Length; i++)
            {
                if (foodStructs[i].color != null)
                {
                    e.Graphics.FillRectangle(foodStructs[i].color, foodStructs[i].obj);
                }
                if (player.x < foodStructs[i].x + foodStructs[i].width && player.x + player.width > foodStructs[i].x &&
                    player.y < foodStructs[i].y + foodStructs[i].height && player.y + player.height > foodStructs[i].y)
                {
                    //Collision detected: remove food that was hit, make player bigger
                    foodStructs[i].width = 0;
                    foodStructs[i].height = 0;
                    foodStructs[i].x = 0;
                    foodStructs[i].y = 0;
                    foodStructs[i].color = null;
                    Console.WriteLine("Collision");

                    player.width += 5;
                    player.height += 5;
                    Invalidate();
                }
            }
        }

        //Updates players position on screen
        private void tmrMoving_Tick(object sender, EventArgs e)
        { 
            if(player.position == Position.Up)
            {
                player.y -= 10;
            }
            else if (player.position == Position.Down)
            {
                player.y += 10;
            }
            else if (player.position == Position.Left)
            {
                player.x -= 10;
            }
            else if (player.position == Position.Right)
            {
                player.x += 10;
            }
            else if (player.position == Position.Stop)
            {
                //Stops player from moving
                player.x = player.x;
                player.y = player.y;
            }

            Invalidate();
        }

        //Player movement controls, updates position
        private void FormView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.A)
            {
                player.position = Position.Left;
            }
            else if(e.KeyCode == Keys.D)
            {
                player.position = Position.Right;
            }
            else if (e.KeyCode == Keys.W)
            {
                player.position = Position.Up;
            }
            else if (e.KeyCode == Keys.S)
            {
                player.position = Position.Down;
            }
        }

        //Updates position to stop when keys are released
        private void FormView_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.W:
                case Keys.A:
                case Keys.S:
                case Keys.D:
                    player.position = Position.Stop;
                    break;

            }
        }
    }
}

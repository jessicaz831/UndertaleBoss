// Jessica Zhu
// May 15, 2018
// Create a Zelda-like boss fight game using math

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace ZeldaGameAssignment
{
    public partial class GameForm : Form
    {
        // use windows media player for background music
        WindowsMediaPlayer player = new WindowsMediaPlayer();

        // timer interval that controls how much time needs to pass before running the loop code
        int interval = 20;

        // state variable to exit the loop
        int loopState = NOT_RUNNING;

        // state variable values
        const int NOT_RUNNING = 0;
        const int RUNNING = 1;
        const int FRONT = 2;
        const int LEFT = 3;
        const int RIGHT = 4;
        const int BACK = 5;

        // stores player's direction
        int direction = FRONT;

        // stores player's health
        int playerHealth;
        const int PLAYER_HEALTH_TOTAL = 250;

        // stores enemy's health
        int enemyHealth;
        const int ENEMY_HEALTH_TOTAL = 500;

        // stores amount of damage player and enemy deals
        const int PLAYER_DAMAGE = 5;
        const int ENEMY_DAMAGE = 10;

        // creates new projectile
        bool isInAir = false;

        // projectile's speeds
        float projectileXSpeed, projectileYSpeed;
        const float TOTAL_SPEED = 8;

        // temporary variables used for the projectile calculations
        float rise, run, hypotenuse;

        // the last time that the loop ran its code
        int lastRunTime;

        // counter for animations
        int counter = 0;
        int counterHead = 0;

        // shows info screen before game that tells player what to do
        bool gameInfo = true;

        // store player movement states
        bool moveUp = false;
        bool moveDown = false;
        bool moveLeft = false;
        bool moveRight = false;

        // store player attack states
        bool weaponUp = false;
        bool weaponDown = false;
        bool weaponLeft = false;
        bool weaponRight = false;
        bool attack = false;

        // store win or lose states
        bool win = false;
        bool lose = false;

        // rectangle to draw my characters, weapons, images inside
        RectangleF enemyHeadBox;
        RectangleF enemyBox;        
        RectangleF playerBox;
        RectangleF weaponBox;
        RectangleF projectileBox;

        // create textbox
        // font for text
        Font titleFont = new Font("Comic Sans MS", 36.0f);
        Font infoFont = new Font("Comic Sans MS", 24.0f);
        // store title's location
        PointF titleLocation = new PointF(200, 50);
        PointF infoLocation = new PointF(120, 150);
        // store enemy's health font and location
        Font enemyHealthFont = new Font("Comic Sans MS", 14.0f);
        PointF enemyHealthLocation = new PointF(575, 50);
        // store player's health font
        Font playerHealthFont = new Font("Comic Sans MS", 10.0f);
        // store win screen locations
        PointF winLocation = new PointF(125, 100);
        PointF winInfoLocation = new PointF(300, 300);
        // store lose screen locations
        PointF loseLocation = new PointF(150, 100);
        PointF loseInfoLocation = new PointF(300, 300);

        public GameForm()
        {
            InitializeComponent();

            // file of song used for background music
            player.URL = "Undertale - Megalovania.MP3";
            
            // play music file
            player.controls.play();

            // setup pictureboxes
            GameSetup();
            // store player's starting health
            playerHealth = 250;
            // store enemy's starting health
            enemyHealth = 500;

        }

        // setup the graphics geometry
        void GameSetup()
        {
            // setup the weapon 'picturebox'
            weaponBox = new RectangleF(500, 485, 20, 38);

            // setup the enemy 'picturebox'
            enemyHeadBox = new RectangleF(460, 13, 110, 90);
            enemyBox = new RectangleF(450, 20, 140, 180);

            // setup the player 'picturebox'
            playerBox = new RectangleF(500, 500, 38, 70);

            // setup the projectile 'picturebox'
            projectileBox = new RectangleF(0, 0, 40, 40);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (loopState == RUNNING)
            {
                // draw the background
                e.Graphics.DrawImage(Properties.Resources.UndertaleBackground, 0, 0, ClientSize.Width, ClientSize.Height);
                // draw the enemy body
                e.Graphics.DrawImage(Properties.Resources.SansHeadless, enemyBox);
                // draw the projectile
                e.Graphics.DrawImage(Properties.Resources.Fireball, projectileBox);
                // draw the enemy's health
                e.Graphics.DrawString(enemyHealth + "/" + ENEMY_HEALTH_TOTAL, enemyHealthFont, Brushes.Cyan, enemyHealthLocation);
                // draw the player's health
                e.Graphics.DrawString(playerHealth + "/" + PLAYER_HEALTH_TOTAL, playerHealthFont, Brushes.Fuchsia, playerBox.X - 10, playerBox.Y - 20);

                if (playerBox.X <= 333)
                {
                    // if the player is on the left side of the screen, make enemy look towards the left
                    e.Graphics.DrawImage(Properties.Resources.SansHeadLeft, enemyHeadBox);
                }
                else if (playerBox.X > 333 && playerBox.X < 667)
                {
                    // if the player is in the middle of the screen, make enemy look down
                    e.Graphics.DrawImage(Properties.Resources.SansHeadFront, enemyHeadBox);
                }
                else if (playerBox.X >= 667)
                {
                    // if the player is on the right side of the screen, make enemy look towards the right
                    e.Graphics.DrawImage(Properties.Resources.SansHeadRight, enemyHeadBox);
                }

                // setup animations
                if (direction == FRONT)
                {
                    if (counter == 20)
                    {
                        // when counter reaches 20, reset the counter
                        counter = 1;
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskFrontWalkLeft, playerBox);
                    }
                    else if (counter < 20 && counter >= 10)
                    {
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskFrontWalkLeft, playerBox);
                    }
                    else if (counter < 10 && counter > 0)
                    {
                        // draw the character walking (first picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskFrontWalkRight, playerBox);
                    }
                    else
                    {
                        // draw the character facing forwards
                        e.Graphics.DrawImage(Properties.Resources.FriskFront, playerBox);
                    }
                }
                else if (direction == BACK)
                {
                    if (counter == 20)
                    {
                        // when counter reaches 20, reset the counter
                        counter = 1;
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskBackWalkLeft, playerBox);
                    }
                    else if (counter < 20 && counter >= 10)
                    {
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskBackWalkLeft, playerBox);
                    }
                    else if (counter < 10 && counter > 0)
                    {
                        // draw the character walking (first picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskBackWalkRight, playerBox);
                    }
                    else
                    {
                        // draw the character facing back
                        e.Graphics.DrawImage(Properties.Resources.FriskBack, playerBox);
                    }
                }
                else if (direction == LEFT)
                {
                    if (counter == 20)
                    {
                        // when the counter reaches 20, reset the counter
                        counter = 1;
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskLeft, playerBox);
                    }
                    else if (counter < 20 && counter >= 10)
                    {
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskLeft, playerBox);
                    }
                    else if (counter < 10 && counter > 0)
                    {
                        // draw the character walking (first picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskLeftWalk, playerBox);
                    }
                    else
                    {
                        // draw the character facing left
                        e.Graphics.DrawImage(Properties.Resources.FriskLeft, playerBox);
                    }
                }
                else if (direction == RIGHT)
                {
                    if (counter == 20)
                    {
                        // when the counter reaches 20, reset the counter
                        counter = 1;
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskRight, playerBox);
                    }
                    else if (counter < 20 && counter >= 10)
                    {
                        // draw the character walking (second picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskRight, playerBox);
                    }
                    else if (counter < 10 && counter > 0)
                    {
                        // draw the character walking (first picture)
                        e.Graphics.DrawImage(Properties.Resources.FriskRightWalk, playerBox);
                    }
                    else
                    {
                        // draw the character facing right
                        e.Graphics.DrawImage(Properties.Resources.FriskRight, playerBox);
                    }
                }

                // change direction of the weapon
                if (weaponUp == true)
                {
                    // draw the weapon facing up
                    e.Graphics.DrawImage(Properties.Resources.RealKnifeUp, weaponBox);
                }
                else if (weaponDown == true)
                {
                    // draw the weapon facing down
                    e.Graphics.DrawImage(Properties.Resources.RealKnifeDown, weaponBox);
                }
                else if (weaponLeft == true)
                {
                    // draw the weapon facing left
                    e.Graphics.DrawImage(Properties.Resources.RealKnifeLeft, weaponBox);
                }
                else if (weaponRight == true)
                {
                    // draw the weapon facing right
                    e.Graphics.DrawImage(Properties.Resources.RealKnifeRight, weaponBox);
                }
            }
            else
            {
                if (gameInfo == true)
                {
                    // draw the title screen with instructions
                    e.Graphics.DrawString("Filled With Determination", titleFont, Brushes.Red, titleLocation);
                    // draw the instructions
                    e.Graphics.DrawString("Welcome to Filled With Determination!\r\nUse the arrow keys to walk around and WASD to\r\nattack with your knife.\r\nAvoid the fireballs! They will deal damage to you.\r\nPress F5 to start!", infoFont, Brushes.Black, infoLocation);
                }

                else if (win == true)
                {
                    // draw the victory screen
                    e.Graphics.DrawString("You are filled with determination!\r\nYou defeated Sans!", titleFont, Brushes.Red, winLocation);
                    // draw the instructions to restart
                    e.Graphics.DrawString("Press enter to play again!", infoFont, Brushes.Black, winInfoLocation);
                }

                else if (lose == true)
                {
                    // draw the defeat screen
                    e.Graphics.DrawString("You lost your determination!\r\nSans defeated you!", titleFont, Brushes.Red, loseLocation);
                    // draw the instructions to restart
                    e.Graphics.DrawString("Press enter to play again!", infoFont, Brushes.Black, loseInfoLocation);
                }
            }
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                // run loop
                loopState = RUNNING;
                // run timer
                CustomTimer();
                // hide title screen
                gameInfo = false;
            }

            // programming arrow keys movement

            // Player moves up when user presses up arrow, sets direction to BACK and moveUp to true
            if (e.KeyCode == Keys.Up)
            {
                moveUp = true;
                direction = BACK;
            }
            // Player moves down when user presses down arrow, sets direction to FRONT and moveDown to true
            else if (e.KeyCode == Keys.Down)
            {
                moveDown = true;
                direction = FRONT;
            }
            // Player moves left when user presses left arrow, sets direction to LEFT and moveLeft to true
            else if (e.KeyCode == Keys.Left)
            {
                moveLeft = true;
                direction = LEFT;
            }
            // Player moves right when user presses right arrow, sets direction to RIGHT and moveRight to true
            else if (e.KeyCode == Keys.Right)
            {
                moveRight = true;
                direction = RIGHT;
            }

            // programming attack direction
            
            // sets direction to BACK when user presses W
            if (e.KeyCode == Keys.W)
            {
                direction = BACK;
                // sets weapon to face up
                weaponUp = true;
                // sets all other weapon directions as false
                weaponDown = false;
                weaponLeft = false;
                weaponRight = false;
                // allows user to deal damage to boss
                attack = true;
            }
            // sets direction to FRONT when user presses S
            else if (e.KeyCode == Keys.S)
            {
                direction = FRONT;
                // sets weapon to face down
                weaponDown = true;
                // sets all other weapon directions as false
                weaponUp = false;
                weaponLeft = false;
                weaponRight = false;
                // allows user to deal damage to boss
                attack = true;
            }
            // sets direction to LEFT when user presses A
            else if (e.KeyCode == Keys.A)
            {
                direction = LEFT;
                // sets weapon to face left
                weaponLeft = true;
                // sets all other weapon directions as false
                weaponDown = false;
                weaponUp = false;
                weaponRight = false;
                // allows user to deal damage to boss
                attack = true;
            }
            // sets direction to RIGHT when user presses D
            else if (e.KeyCode == Keys.D)
            {
                direction = RIGHT;
                // sets weapon to face right
                weaponRight = true;
                // sets all other weapon directions as false
                weaponDown = false;
                weaponLeft = false;
                weaponUp = false;
                // allows user to deal damage to boss
                attack = true;
            }
            // restart game when player presses enter after winning or losing
            else if (e.KeyCode == Keys.Enter)
            {
                if (win == true || lose == true)
                {
                    Application.Restart();
                }
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            // When the user lets go of up arrow, turn off upward movement
            if (e.KeyCode == Keys.Up)
            {
                moveUp = false;
            }
            // When the user lets go of down arrow, turn off downward movement
            else if (e.KeyCode == Keys.Down)
            {
                moveDown = false;
            }
            // When the user lets go of left arrow, turn off left movement
            else if (e.KeyCode == Keys.Left)
            {
                moveLeft = false;
            }
            // When the user lets go of right arrow, turn off right movement
            else if (e.KeyCode == Keys.Right)
            {
                moveRight = false;
            }
            
            // When the user lets go of W, hide weapon and do not allow user to deal damage
            if (e.KeyCode == Keys.W)
            {
                weaponUp = false;
                attack = false;
            }
            // When the user lets go of S, hide weapon and do not allow user to deal damage
            else if (e.KeyCode == Keys.S)
            {
                weaponDown = false;
                attack = false;
            }
            // When the user lets go of A, hide weapon and do not allow user to deal damage
            else if (e.KeyCode == Keys.A)
            {
                weaponLeft = false;
                attack = false;
            }
            // When the user lets go of D, hide weapon and do not allow user to deal damage
            else if (e.KeyCode == Keys.D)
            {
                weaponRight = false;
                attack = false;
            }
        }

        void CheckLeftBoundary()
        {
            // stop the x value from being negative
            if (playerBox.X < 0)
            {
                playerBox.X = 0;
            }
        }

        void CheckTopBoundary()
        {
            // stop the y value from being negative
            if (playerBox.Y < 0)
            {
                playerBox.Y = 0;
            }
        }

        void CheckRightBoundary()
        {
            // stop the player's right x value from being bigger than the screen's width
            if (playerBox.X > this.ClientSize.Width - playerBox.Width)
            {
                playerBox.X = this.ClientSize.Width - playerBox.Width;
            }
        }

        void CheckBottomBoundary()
        {
            // stop the player's bottom y value from being bigger than the screen's height
            if (playerBox.Y > this.ClientSize.Height - playerBox.Height)
            {
                playerBox.Y = this.ClientSize.Height - playerBox.Height;
            }
        }

        // stops the user from going past any of the edges of the form
        void BoundaryDetection()
        {
            CheckLeftBoundary();
            CheckTopBoundary();
            CheckRightBoundary();
            CheckBottomBoundary();

            playerBox.Location = new PointF (playerBox.X, playerBox.Y);
        }

        void CreateProjectile()
        {
            // if a projectile is not already made, create a new one
            if (isInAir == false)
            {
                // projectile starts off at the enemy
                projectileBox.Location = new PointF(enemyBox.X + 110, enemyBox.Y + 125);

                // rise = y2 - y1
                rise = playerBox.Y - enemyBox.Y - 125;

                // run = x2 - x1
                run = playerBox.X - enemyBox.X - 110;

                // c^2 = a^2 + b^2
                // need to convert the Math.Sqrt into a float
                hypotenuse = (float)Math.Sqrt(Math.Pow(rise, 2) + Math.Pow(run, 2));

                // calculate the projectile's x and y speed components
                projectileYSpeed = rise / hypotenuse * TOTAL_SPEED;
                projectileXSpeed = run / hypotenuse * TOTAL_SPEED;

                // set projectile isInAir to true so only one projectile is made at a time
                isInAir = true;
            }
        }

        // move the projectile according to its calculated speeds
        void MoveProjectile()
        {
            // calculate the new x and y locations for the projectile -- original location + speed
            int x = (int)(projectileBox.X + projectileXSpeed);
            int y = (int)(projectileBox.Y + projectileYSpeed);

            projectileBox.Location = new PointF(x, y);
            
            // if the projectile hits the target, remove 10 health points from player
            if (projectileBox.IntersectsWith(playerBox))
            {
                playerHealth = playerHealth - 10;
            }

            // if the projectile hits the target or a boundary, create a new one
            if (projectileBox.IntersectsWith(playerBox) == true || projectileBox.X > ClientSize.Width || projectileBox.Y > ClientSize.Height || projectileBox.X < 0 || projectileBox.Y < 0)
            {
                isInAir = false;
            }            
        }

        // check if the player won or lost
        void CheckWin()
        {
            // if player health is less than or equal to 0, display lose screen and turn off loop
            if (playerHealth <= 0)
            {
                lose = true;
                // ensure the title screen doesn't appear
                gameInfo = false;
                loopState = NOT_RUNNING;
            }

            // if player health is less than or equal to 0, display win screen and turn off loop
            else if (enemyHealth <= 0)
            {
                win = true;
                // ensure the title screen doesn't appear
                gameInfo = false;
                loopState = NOT_RUNNING;
            }
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // when user closes program, turn off loop so it doesn't keep running in the background
            loopState = NOT_RUNNING;
            // stop background music
            player.controls.stop();
        }

        void CustomTimer()
        {
            // set up the timer
            loopState = RUNNING;
            // the lastRunTime is 'right now'
            lastRunTime = Environment.TickCount;

            // create a loop to do 'something' -- it counts up a label once per second 
            while (loopState == RUNNING)
            {
                // Environment.TickCount gives the current time
                // Environment.TickCount - lastRunTime gives the amount of time that has passed since the 'inside' of the if statement below ran
                if (Environment.TickCount - lastRunTime >= interval)
                {
                    // update the last run time to 'right now'
                    lastRunTime = Environment.TickCount;

                    // if user is moving in any direction, increase counter by 1 for animations
                    if (moveUp == true || moveDown == true || moveLeft == true || moveRight == true)
                    {
                        counter = counter + 1;
                    }
                    // if player is not moving, set counter to 0
                    else if (moveUp == false && moveDown == false && moveLeft == false && moveRight == false)
                    {
                        counter = 0;
                    }

                    // make sure player cannot pass boundaries
                    BoundaryDetection();
                    // create the projectile
                    CreateProjectile();
                    // move the projectile
                    MoveProjectile();
                    // check if player won or lost
                    CheckWin();

                    // if counter reaches 21, reset counter
                    if (counterHead == 21)
                    {
                        counterHead = 0;
                    }
                    // increase counter by 1
                    else
                    {
                        counterHead = counterHead + 1;
                    }

                    // if the counter is greater than 10, make enemy's head move up
                    if (counterHead > 10)
                    {
                        enemyHeadBox.Y = enemyHeadBox.Y - 1;
                        enemyHeadBox.Location = new PointF(enemyHeadBox.X, enemyHeadBox.Y);
                    }
                    // if the counter is less than 10, make enemy's head move down
                    else
                    {
                        enemyHeadBox.Y = enemyHeadBox.Y + 1;
                        enemyHeadBox.Location = new PointF(enemyHeadBox.X, enemyHeadBox.Y);
                    }

                    // if weapon intersects with enemy and player is allowed to deal damage, remove 5 health points from enemy's health
                    if (weaponBox.IntersectsWith(enemyBox) && attack == true || weaponBox.IntersectsWith(enemyHeadBox) && attack == true)
                    {
                        enemyHealth = enemyHealth - 5;
                    }

                    // When upward movement is on, decrease the player's y value by 5
                    if (moveUp == true)
                    {
                        playerBox.Y = playerBox.Y - 5;
                    }
                    // When downward movement is on, increase the player's y value by 5
                    if (moveDown == true)
                    {
                        playerBox.Y = playerBox.Y + 5;
                    }
                    // When left movement is on, decrease the player's x value by 5
                    if (moveLeft == true)
                    {
                        playerBox.X = playerBox.X - 5;
                    }
                    // When right movement is on, increase the player's x value by 5
                    if (moveRight == true)
                    {
                        playerBox.X = playerBox.X + 5;
                    }

                    if (weaponUp == true)
                    {
                        // change weapon box dimensions and location
                        weaponBox = new RectangleF(playerBox.X + 10, playerBox.Y - 30, 20, 38);
                    }
                    if (weaponDown == true)
                    {
                        // change the weapon box dimensions and location
                        weaponBox = new RectangleF(playerBox.X + 10, playerBox.Y + 60, 20, 38);
                    }
                    if (weaponLeft == true)
                    {
                        // change the weapon box dimensions and location
                        weaponBox = new RectangleF(playerBox.X - 30, playerBox.Y + 30, 38, 20);
                    }
                    if (weaponRight == true)
                    {
                        // change the weapon box dimensions and location
                        weaponBox = new RectangleF(playerBox.X + 30, playerBox.Y + 30, 38, 20);
                    }

                    Refresh();
                }

                // Prevent freezing
                Application.DoEvents();
            }
        }
    }
}

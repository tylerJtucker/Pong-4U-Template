
/*
 * Description: A basic PONG simulator
 * Author: Tyler Tucker        
 * Date: 2019-02-04      
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush drawBrush = new SolidBrush(Color.White);
        SolidBrush drawBrushP1 = new SolidBrush(Color.Red);
        SolidBrush drawBrushP2 = new SolidBrush(Color.Blue);
        Pen drawPenP1 = new Pen(Color.Red);
        Pen drawPenP2 = new Pen(Color.Blue);
        Pen drawPenBall = new Pen(Color.GhostWhite);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        const int BALL_SPEED = 4;
        Rectangle ball;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 4;
        Rectangle p1, p2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 3;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }
        
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            p1.Width = p2.Width = 10;    //height for both paddles set the same
            p1.Height = p2.Height = 40;  //width for both paddles set the same


            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            //ball size
            ball.Width = ball.Height = 12;

            //ball starting position
            ball.X = this.Width / 2 - ball.Width/2;
            ball.Y = this.Height / 2 - ball.Width /2; 

        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position
            if (ballMoveRight == true)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + BALL_SPEED;
            }
            if (ballMoveRight == false)
            {
                ball.X = ball.X - BALL_SPEED;
            }
            if (ballMoveDown == false)
            {
                ball.Y = ball.Y - BALL_SPEED;
            }
            #endregion

            #region paddle positions

            if (aKeyDown == true && p1.Y > 0)
            {
                //move player 1 paddle up
                p1.Y = p1.Y - PADDLE_SPEED;
            }

            if (zKeyDown == true && p1.Y < this.Height - p1.Height)
            {
                //move player 1 paddle down
                p1.Y = p1.Y + PADDLE_SPEED;
            }

            if (jKeyDown == true && p2.Y > 0)
            {
                //move player 1 paddle up
                p2.Y = p2.Y - PADDLE_SPEED;
            }

            if (mKeyDown == true && p2.Y < this.Height - p2.Height)
            {
                //move player 1 paddle down
                p2.Y = p2.Y + PADDLE_SPEED;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                //change direction
                ballMoveDown = true;
                //play a collision sound
                collisionSound.Play();
            }
            if (ball.Y > this.Height - ball.Height) // if ball hits bottom line
            {
                //change direction
                ballMoveDown = false;
                //play a collision sound
                collisionSound.Play();
            }
            #endregion

            #region ball collision with paddles

            if (p1.IntersectsWith(ball))
            {
                ballMoveRight = true;
                collisionSound.Play();
            }

            if (p2.IntersectsWith(ball))
            {
                ballMoveRight = false;
                collisionSound.Play();
            }
            /*  ENRICHMENT
             *  Instead of using two if statments as noted above see if you can create one
             *  if statement with multiple conditions to play a sound and change direction
             */

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall
            {
                //play score sound
                //update player 2 score
                scoreSound.Play();
                player2Score++;

                //if statement to check to see if player 2 has won the game. If true run 
                if(player2Score >= gameWinScore)
                {
                    GameOver("Player 2");
                }
                else
                {
                    ballMoveRight = !ballMoveRight;
                    SetParameters();
                }

            }

            if (ball.X > this.Width)  // ball hits right wall
            {
                //play score sound
                //update player 1 score
                scoreSound.Play();
                player1Score++;

                //if statement to check to see if player 1 has won the game.
                if (player1Score >= gameWinScore)
                {
                    GameOver("Player 1");
                }
                else
                {
                    ballMoveRight = !ballMoveRight;
                    SetParameters();
                }

            }

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {         
            //stop the gameUpdateLoop
            //show a message on the startLabel to indicate a winner, (need to Refresh).
            //pause for two seconds 
            //use the startLabel to ask the user if they want to play again

            gameUpdateLoop.Stop();
            startLabel.Visible = true;
            startLabel.Text = winner + " Takes the Big W ";
            this.Refresh();
            Thread.Sleep(2000);
            startLabel.Text = "Tryna Play Again?  Hit Space";
            this.Refresh();
            newGameOk = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //draw paddles using FillRectangle
            e.Graphics.DrawRectangle(drawPenP1, p1);
            e.Graphics.DrawRectangle(drawPenP2, p2);

            //draw ball using FillRectangle
            e.Graphics.DrawEllipse(drawPenBall, ball);

            //draw scores to the screen using DrawString
            e.Graphics.DrawString("Player 1: " + player1Score, drawFont, drawBrushP1, 15, 15);
            e.Graphics.DrawString("Player 2: " + player2Score, drawFont, drawBrushP2, this.Width - 110, 15);
        }

    }
}

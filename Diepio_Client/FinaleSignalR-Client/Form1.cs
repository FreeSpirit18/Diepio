using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using FinaleSignalR_Client.Controls;
using Microsoft.AspNetCore.SignalR.Client;



// Afk players dont go away so create Player class and all that jazz

namespace FinaleSignalR_Client
{
    public partial class Form1 : Form
    {
        HubConnection connection;
        int playerspeed;
        int playerCount = 0;
        int id;
        PictureBox playerBox;
        MapControl mapControl;

        int mapMinX = 0;
        int mapMinY = 0;
        int mapMaxX = 0;
        int mapMaxY = 0;

        //List<Rectangle> obstacles;

        public Form1()
        {
            InitializeComponent();


            playerspeed = 5;
            this.KeyPreview = true;
            this.mapControl = new MapControl();
            this.Controls.Add(this.mapControl);
            //this.obstacles = new List<Rectangle>();
            //obstacles.Add(new Rectangle(100, 100, 50, 50));
            

            //((System.ComponentModel.ISupportInitialize)(Player)).EndInit();

            //Conects to a given url
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7181/chatHub")
                .WithAutomaticReconnect()
                .Build();

            // These define what happens during their scenarios
            connection.Reconnecting += (sender) =>
            {
                messages.Invoke((MethodInvoker)delegate
                {
                    var newMessage = "Attempting to connect to the server...";
                    messages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            connection.Reconnected += (sender) =>
            {
                messages.Invoke((MethodInvoker)delegate
                {
                    var newMessage = "Reconnected to the server";
                    messages.Items.Clear();
                    messages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            connection.Closed += (sender) =>
            {
                messages.Invoke((MethodInvoker)delegate
                {
                    var newMessage = "Connection Closed";
                    messages.Items.Add(newMessage);
                    openConnection.Enabled = true;
                    sendMessage.Enabled = true;
                });

                return Task.CompletedTask;
            };
        }

        private void createPlayer(string id)
        {
            var Player = new PictureBox();
            Player.BackColor = System.Drawing.SystemColors.ControlDark;
            Player.Location = new System.Drawing.Point(666, 422);
            Player.Name = id;
            Player.Size = new System.Drawing.Size(64, 64);
            Player.TabIndex = 0;
            Player.TabStop = false;

            this.mapMaxX = mapControl.Width - Player.Width;
            this.mapMaxY = mapControl.Height - Player.Height;

            //adding player to map
            this.mapControl.Controls.Add(Player);

            playerBoxes[playerCount] = Player;
            
            
            //this.Controls.Add(playerBoxes[playerCount]);
            playerCount++;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (user, message) => 
            {
                messages.Invoke((MethodInvoker)delegate
                {
                    var parsedMessage = message.Split('|');
                    if (parsedMessage.Length > 0)
                    {
                        if (parsedMessage[0] == "All" || parsedMessage[0] == id.ToString())
                        {
                            switch (parsedMessage[1]) 
                            {
                                case "RequestAccepted":
                                    createPlayer(id.ToString());
                                    playerBox = playerBoxes[0];
                                    ServerTimer.Start();
                                    break;
                                case "EnemyCreated":
                                    if (parsedMessage[2] != id.ToString())
                                    {
                                        createPlayer(parsedMessage[2]);
                                    }
                                    break;
                                case "Coords":
                                    moveEnemy(parsedMessage[2], int.Parse(parsedMessage[3]), int.Parse(parsedMessage[4]));
                                    break;
                            }
                        }
                        
                    }
                    var newMessage = $"{user}: {message}";
                    messages.Items.Add(newMessage);
                });
            });

            try
            {
                await connection.StartAsync();
                messages.Items.Add("Connection Started");
                Random rnd = new Random();
                id = rnd.Next(100000);
                canICreateAvatar(id);

                openConnection.Enabled = false;
                sendMessage.Enabled = true;
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }

        private void moveEnemy(string id, int left, int top)
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (playerBoxes[i].Name == id)
                {
                    playerBoxes[i].Left = left;
                    playerBoxes[i].Top = top;
                }
            }
        }

        private async void canICreateAvatar(int id)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", id.ToString(), "Request|CreateAvatar");
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }

        private async void sendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", id.ToString(), "Chat|"+messageInput.Text);
            }
            catch (Exception ex) 
            {
                messages.Items.Add(ex.Message);
            }
        }

        private void playerBox_Click(object sender, EventArgs e)
        {

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                LeftTimer.Start();
            }

            if (e.KeyCode == Keys.Right)
            {
                RightTimer.Start();
            }

            if (e.KeyCode == Keys.Up)
            {
                UpTimer.Start();
            }

            if (e.KeyCode == Keys.Down)
            {
                DownTimer.Start();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                LeftTimer.Stop();
            }
            if (e.KeyCode == Keys.Right)
            {
                RightTimer.Stop();
            }
            if (e.KeyCode == Keys.Up)
            {
                UpTimer.Stop();
            }
            if (e.KeyCode == Keys.Down)
            {
                DownTimer.Stop();
            }
        }
        
        private void DownTimer_Tick(object sender, EventArgs e)
        {

            if (playerBox.Top + playerspeed < mapMaxY)
            {
                int newPlayerTop = playerBox.Top + playerspeed;

                // Check if the new position collides with any obstacle
                if (!CollidesWithObstacle(playerBox.Left, newPlayerTop, playerBox.Width, playerBox.Height))
                    playerBox.Top += playerspeed;
            }
        }

        private void LeftTimer_Tick_1(object sender, EventArgs e)
        {
            if (playerBox.Left - playerspeed > mapMinX)
            {
                int newPlayerLeft = playerBox.Left - playerspeed;

                // Check if the new position collides with any obstacle
                if (!CollidesWithObstacle(newPlayerLeft, playerBox.Top, playerBox.Width, playerBox.Height))

                    playerBox.Left -= playerspeed;
            }
        }

        private void UpTimer_Tick_1(object sender, EventArgs e)
        {
            if (playerBox.Top - playerspeed > mapMinY)
            {
                int newPlayerTop = playerBox.Top - playerspeed;

                // Check if the new position collides with any obstacle
                if (!CollidesWithObstacle(playerBox.Left, newPlayerTop, playerBox.Width, playerBox.Height))
                    playerBox.Top -= playerspeed;
            }
        }

        private void RightTimer_Tick_1(object sender, EventArgs e)
        {
            if (playerBox.Left + playerspeed < mapMaxX)
            {
                int newPlayerLeft = playerBox.Left + playerspeed;
                if (!CollidesWithObstacle(newPlayerLeft, playerBox.Top, playerBox.Width, playerBox.Height))
                    playerBox.Left += playerspeed;
            }
        }

        // Function to check collision with obstacles
        private bool CollidesWithObstacle(int x, int y, int width, int height)
        {
            Rectangle playerRect = new Rectangle(x, y, width, height);

            foreach (Rectangle obstacle in this.mapControl.obstacles)
            {
                if (playerRect.IntersectsWith(obstacle))
                {
                    return true; // Collision detected
                }
            }

            return false; // No collision detected
        }

        private async void ServerTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(playerBox.Left.ToString());
                await connection.InvokeAsync("SendMessage", id.ToString(), $"Coords|{playerBox.Left}|{playerBox.Top}");
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }
    }
}

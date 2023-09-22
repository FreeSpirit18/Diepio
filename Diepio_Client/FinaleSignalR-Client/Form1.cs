using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;



// Afk players dont go away so create Player class and all that jazz

namespace FinaleSignalR_Client
{

    public class Bullet
    {
        public PictureBox BulletPictureBox { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; } = 10;

        public Bullet(Point startPosition, Vector2 direction)
        {
            BulletPictureBox = new PictureBox
            {
                Size = new Size(10, 10), // example size
                BackColor = Color.Red,
                Location = startPosition,
            };
            Direction = direction;
        }

        public void Move()
        {
            BulletPictureBox.Left += (int)(Direction.X * Speed);
            BulletPictureBox.Top += (int)(Direction.Y * Speed);
        }
    }



    public partial class Form1 : Form
    {
        HubConnection connection;
        int playerspeed;
        int playerCount = 0;
        int id;
        PictureBox playerBox;
        private bool isShooting = false;
        private DateTime lastBulletFiredTime;
        private TimeSpan bulletCooldown = TimeSpan.FromMilliseconds(150);


        public Form1()
        {
            InitializeComponent();


            playerspeed = 5;
            this.KeyPreview = true;



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


            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;


            Timer bulletMovementTimer = new Timer();
            bulletMovementTimer.Interval = 1;
            bulletMovementTimer.Tick += BulletMovementTimer_Tick;
            bulletMovementTimer.Start();
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
            playerBoxes[playerCount] = Player;
            this.Controls.Add(playerBoxes[playerCount]);
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
                                case "BULLET":
                                    Vector2 bulletDirection = new Vector2(float.Parse(parsedMessage[4]), float.Parse(parsedMessage[5]));
                                    Point startPoint = new Point(int.Parse(parsedMessage[2]), int.Parse(parsedMessage[3]));
                                    var bullet = new Bullet(startPoint, bulletDirection);
                                    bullets.Add(bullet);
                                    this.Controls.Add(bullet.BulletPictureBox);
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
                await connection.InvokeAsync("SendMessage", id.ToString(), "Chat|" + messageInput.Text);
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
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                LeftTimer.Start();
            }

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                RightTimer.Start();
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                UpTimer.Start();
            }

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                DownTimer.Start();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                LeftTimer.Stop();
            }

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                RightTimer.Stop();
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                UpTimer.Stop();
            }

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                DownTimer.Stop();
            }
        }


        private void DownTimer_Tick(object sender, EventArgs e)
        {

            if (playerBox.Top < ClientSize.Height - playerBox.Height - 10)
            {
                playerBox.Top += playerspeed;
            }
        }

        private void LeftTimer_Tick_1(object sender, EventArgs e)
        {
            if (playerBox.Left > 10)
            {
                playerBox.Left -= playerspeed;
            }
        }

        private void UpTimer_Tick_1(object sender, EventArgs e)
        {
            if (playerBox.Top > 10)
            {
                playerBox.Top -= playerspeed;
            }
        }

        private void RightTimer_Tick_1(object sender, EventArgs e)
        {
            if (playerBox.Left < ClientSize.Width - playerBox.Height - 10)
            {
                playerBox.Left += playerspeed;
            }
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

        List<Bullet> bullets = new List<Bullet>();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isShooting = true;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isShooting = false;
            }
        }

        private async void BulletMovementTimer_Tick(object sender, EventArgs e)
        {
            // The shooting logic
            if (isShooting && (DateTime.Now - lastBulletFiredTime) > bulletCooldown)
            {
                Point targetPoint = this.PointToClient(Cursor.Position);

                Vector2 start = new Vector2(playerBox.Location.X, playerBox.Location.Y);
                Vector2 target = new Vector2(targetPoint.X, targetPoint.Y);

                Vector2 direction = Vector2.Normalize(target - start);

                Bullet bullet = new Bullet(playerBox.Location, direction);
                bullets.Add(bullet);
                this.Controls.Add(bullet.BulletPictureBox);

                try
                {
                    await connection.InvokeAsync("SendMessage", id.ToString(), $"BULLET|{playerBox.Location.X}|{playerBox.Location.Y}|{direction.X}|{direction.Y}");
                }
                catch (Exception ex)
                {
                    messages.Items.Add($"Error sending bullet data: {ex.Message}");
                }

                lastBulletFiredTime = DateTime.Now;
            }

            // Bullet movement logic
            List<Bullet> bulletsToRemove = new List<Bullet>();
            foreach (Bullet bullet in bullets)
            {
                bullet.Move();  // Assuming you have a Move method in the Bullet class

                // Check if bullet is out of form bounds
                if (bullet.BulletPictureBox.Left < 0 ||
                    bullet.BulletPictureBox.Right > this.Width ||
                    bullet.BulletPictureBox.Top < 0 ||
                    bullet.BulletPictureBox.Bottom > this.Height)
                {
                    bulletsToRemove.Add(bullet);
                    this.Controls.Remove(bullet.BulletPictureBox);
                }
            }

            // Remove out-of-bounds bullets from the list
            foreach (Bullet bullet in bulletsToRemove)
            {
                bullets.Remove(bullet);
            }
        }
    }
}

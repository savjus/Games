using System;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TicTacToeForm());
        }
    }

    public class TicTacToeForm : Form
    {
        private Button[,] buttons = new Button[3, 3];
        private bool Oturn;
        private Button reset;
        private Label Xscore;
        private Label Oscore;
        private int XscoreCount;
        private int OscoreCount;
        
        private Random random = new Random();

        public TicTacToeForm()
        {
            this.Text = "Tic Tac Toe";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(300, 400);

            InitializeBoard();
            InitializeResetButton();
            InitializeScoreLabels();
        }

        public void InitializeScoreLabels()
        {
            Xscore = new Label();
            Xscore.Text = "X:" + XscoreCount;
            Xscore.Size = new Size(100, 50);
            Xscore.Location = new Point(0, 350);
            this.Controls.Add(Xscore);

            Oscore = new Label();
            Oscore.Text = "O:" + OscoreCount;
            Oscore.Size = new Size(100, 50);
            Oscore.Location = new Point(200, 350);
            this.Controls.Add(Oscore);
        }

        public void InitializeBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Size = new Size(100, 100);
                    buttons[i, j].Location = new Point(i * 100, j * 100);
                    buttons[i, j].Click += Button_Click;
                    this.Controls.Add(buttons[i, j]);
                }
            }
        }

        public void InitializeResetButton()
        {
            reset = new Button();
            reset.Text = "Reset";
            reset.Size = new Size(300, 50);
            reset.Location = new Point(0, 300);
            reset.Click += Reset_Click;
            this.Controls.Add(reset);
        }

        public void Button_Click(Object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text == "")
            {
                button.Text = Oturn ? "O" : "X";
                if (CheckForWinner())
                {
                    if (Oturn)
                    {
                        MessageBox.Show("O wins!");
                        OscoreCount++;
                        Oscore.Text = "O:" + OscoreCount;
                    }
                    else
                    {
                        MessageBox.Show("X wins!");
                        XscoreCount++;
                        Xscore.Text = "X:" + XscoreCount;
                    }
                    ResetBoard();
                }
                else if (CheckForDraw())
                {
                    MessageBox.Show("It's a draw!");
                    ResetBoard();
                }
                else
                {
                    Oturn = !Oturn;
                    if (Oturn)
                    {
                        MakeAIMove();
                    }
                }
            }
        }

        public void MakeAIMove()
        {
            while (true)
            {
                int i = random.Next(3);
                int j = random.Next(3);
                if (buttons[i, j].Text == "")
                {
                    buttons[i, j].Text = "O";
                    if (CheckForWinner())
                    {
                        MessageBox.Show("O wins!");
                        OscoreCount++;
                        Oscore.Text = "O:" + OscoreCount;
                        ResetBoard();
                    }
                    else if (CheckForDraw())
                    {
                        MessageBox.Show("It's a draw!");
                        ResetBoard();
                    }
                    Oturn = !Oturn;
                    break;
                }
            }
        }

        public bool CheckForWinner()
        {
            for (int i = 0; i < 3; i++)
            {
                if (buttons[i, 0].Text != "" && buttons[i, 0].Text == buttons[i, 1].Text && buttons[i, 1].Text == buttons[i, 2].Text)
                {
                    return true;
                }
                if (buttons[0, i].Text != "" && buttons[0, i].Text == buttons[1, i].Text && buttons[0, i].Text == buttons[2, i].Text)
                {
                    return true;
                }
            }
            if (buttons[0, 0].Text != "" && buttons[0, 0].Text == buttons[1, 1].Text && buttons[1, 1].Text == buttons[2, 2].Text)
            {
                return true;
            }
            if (buttons[0, 2].Text != "" && buttons[0, 2].Text == buttons[1, 1].Text && buttons[1, 1].Text == buttons[2, 0].Text)
            {
                return true;
            }
            return false;
        }

        public bool CheckForDraw()
        {
            foreach (Button button in buttons)
            {
                if (button.Text == "")
                {
                    return false;
                }
            }
            return true;
        }

        public void Reset_Click(Object sender, EventArgs e)
        {
            ResetBoard();
        }

        public void ResetBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j].Text = "";
                }
            }
            Oturn = false;
        }
    }
}
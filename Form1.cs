using static System.Windows.Forms.LinkLabel;
using System.IO;
using System.Reflection.Emit;
using static System.Windows.Forms.AxHost;
using System.Drawing;

namespace HangMan
{//Aktuelle Aufgabe: 

    internal partial class Form1 : Form
    {
        //Fields
        List<System.Windows.Forms.Label> labels = new List<System.Windows.Forms.Label>();
        List<string> words = new List<string>();
        //string path = "C:\\Users\\sezgi\\Documents\\Visual Studio Mein repos Ordner\\Projekte&Aufgaben Umschulung\\Vertiefung\\WinForms\\HangMan\\HangMan\\TextFiles\\Words.txt";
        string randomWord = "";
        string randomWordToLower = "";
        string LetterInputToLower = "";
        int missedCounter = 0;
        bool missedLetter = false;
        bool textFileFound = true;

        //Constructor
        public Form1()
        {
            InitializeComponent();
        }

        //Methods        
        #region Buttons         
        private void bt_Start_Click(object sender, EventArgs e)
        {
            ResetAll();
            WordAdder();
            if (textFileFound == true)
            {
                LabelAdderForLetters();
                GenerateRandomWord();
                TransmitFirstLetterToFirstLabel();
                DisplayWordLength();
                DrawLinesForLetters();
                EnableOrDisableButtons(bt_SubmitLetter, true, bt_SubmitWord, true);
            }
        }

        private void bt_SubmitLetter_Click(object sender, EventArgs e)
        {
            ConvertInputWordToLowerCase();
            ConvertInputLetterToLowerCase();
            TransmitCorrectLetterToLabel();
            DisplayIncorrectLettersAndNumberOfFailedAttempts();
            CheckIfCorrectWordGuessedByLetterInput();
            RemoveContentFromTextboxLetter();
        }

        private void bt_SubmitWord_Click(object sender, EventArgs e)
        {
            ConvertInputLetterToLowerCase();
            ConvertInputWordToLowerCase();
            DisplayWinOrLostAfterWordInput();
            RemoveContentFromTextboxWord();
        }

        private void EnableOrDisableButtons(Button button, bool enabled, Button button2, bool enabled2)
        {
            button.Enabled = enabled;
            button2.Enabled = enabled2;
        }
        #endregion

        #region Views
        //Header, Word, Word Length, Won, Lost, Drawings, Letter Submission to Label,
        //Missed Number and output of incorrect letters


        private async void DisplayHeaderWhenWon()
        {
            await Task.Delay(1000);
            label_Header.Text = "Du hast das korrekte Wort erraten";
        }

        private async void DisplayHeaderWhenLost()
        {
            await Task.Delay(1000);
            label_Header.Text = $"Das richtige Wort wäre {randomWord} gewesen";
        }

        private async void DisplayWord()
        {
            await Task.Delay(1000);
            label_Word.Text = randomWord;
        }

        private void DisplayWordLength()
        {
            label_WordLength.Text = "";
            label_WordLength.Text = "Word Length: ";
            label_WordLength.Text += randomWord.Length.ToString();
        }

        private void DisplayWon()
        {
            try
            {
                EnableOrDisableButtons(bt_SubmitWord, false, bt_SubmitLetter, false);
                label_FirstLetter.Visible = false;
                label0_Letter.Visible = true;
                string gewonnen = "GEWONNEN! ";
                Graphics graphics = pn1_draw.CreateGraphics();
                Pen pen = new Pen(Color.Black, width: 4);
                int num1 = 10;
                int num3 = 50;
                graphics.Clear(pn1_draw.BackColor);
                for (int i = 0; i < gewonnen.Length - 1; i++) //Für saubere Linien nebeneinander erhalten Zahl1 und Zahl3 bei jeder neuen Linie immer +50
                {
                    graphics.DrawLine(pen, num1, 100, num3, 100);
                    num1 += 50;
                    num3 += 50;
                }
                for (int k = 0; k < labels.Count; k++)
                {
                    labels[k].Text = "";
                }
                for (int j = 0; j < gewonnen.Length - 1; j++)
                {
                    labels[j].Text = gewonnen[j].ToString();
                }
            }
            catch { }
        }

        private void DisplayLost()
        {
            try
            {
                EnableOrDisableButtons(bt_SubmitWord, false, bt_SubmitLetter, false);
                label_FirstLetter.Visible = false;
                label0_Letter.Visible = true;
                string verloren = "VERLOREN! ";
                Graphics graphics = pn1_draw.CreateGraphics();
                Pen pen = new Pen(Color.Black, width: 4);
                int num1 = 10;
                int num3 = 50;
                graphics.Clear(pn1_draw.BackColor);
                for (int i = 0; i < verloren.Length - 1; i++) //Für saubere Linien nebeneinander erhalten Zahl1 und Zahl3 bei jeder neuen Linie immer +50
                {
                    graphics.DrawLine(pen, num1, 100, num3, 100);
                    num1 += 50;
                    num3 += 50;
                }
                for (int k = 0; k < labels.Count; k++)
                {
                    labels[k].Text = "";
                }
                for (int j = 0; j < verloren.Length - 1; j++)
                {
                    labels[j].Text = verloren[j].ToString();
                }
            }
            catch { }
        }

        private void DisplayWinOrLostAfterWordInput()
        {
            double num;
            if (bt_SubmitWord.Text.Length >= 1 && !double.TryParse(textBox_SubmitWord.Text, out num))
            {
                string wordInputToLower = textBox_SubmitWord.Text.ToLower();
                if (wordInputToLower == randomWordToLower)
                {
                    DisplayWon();
                    DisplayHeaderWhenWon();
                    DisplayWord();
                }
                else if (wordInputToLower != randomWordToLower)
                {
                    DisplayLost();
                    DrawHangmanCompletely();
                    DisplayHeaderWhenLost();
                    DisplayWord();
                }
            }
        }

        private void CheckIfCorrectWordGuessedByLetterInput()
        {
            textBox_test.Clear();
            foreach (var element in labels)
            {
                if (element.Text.Length >= 1)
                {
                    textBox_test.Text += element.Text;
                }
            }
            if (textBox_test.Text == randomWordToLower)
            {
                DisplayHeaderWhenWon();
                DisplayWord();
                DisplayWon();
            }
        }

        private void DrawHangmanCompletely()
        {
            Graphics graphics = panel_HangMan.CreateGraphics();
            Pen pen = new Pen(Color.Black, width: 4);
            graphics.DrawEllipse(pen, 97, 1, 70, 70);
            graphics.DrawLine(pen, 135, 72, 135, 180);
            graphics.DrawLine(pen, 135, 90, 90, 150);
            graphics.DrawLine(pen, 135, 90, 180, 150);
            graphics.DrawLine(pen, 45, 320, 135, 180);
            graphics.DrawLine(pen, 225, 320, 135, 180);
        }

        private void DrawHangmanPartAfterIncorrectLetter()
        {
            if (missedLetter == true)
            {
                Graphics graphics = panel_HangMan.CreateGraphics();
                Pen pen = new Pen(Color.Black, width: 4);
                if (missedCounter == 1)
                    graphics.DrawEllipse(pen, 97, 1, 70, 70);
                if (missedCounter == 2)
                    graphics.DrawLine(pen, 135, 72, 135, 180);
                if (missedCounter == 3)
                    graphics.DrawLine(pen, 135, 90, 90, 150);
                if (missedCounter == 4)
                    graphics.DrawLine(pen, 135, 90, 180, 150);
                if (missedCounter == 5)
                    graphics.DrawLine(pen, 45, 320, 135, 180);
                if (missedCounter == 6)
                {
                    graphics.DrawLine(pen, 225, 320, 135, 180);
                    DisplayHeaderWhenLost();
                    DisplayWord();
                    DisplayLost();
                }
                missedLetter = false;
            }
        }

        private void DrawLinesForLetters()
        {
            Graphics graphics = pn1_draw.CreateGraphics();
            Pen pen = new Pen(Color.Black, width: 4);
            int num1 = 10;
            int num3 = 50;

            graphics.Clear(pn1_draw.BackColor);

            for (int i = 0; i < randomWord.Length; i++) //Für saubere Linien nebeneinander erhalten Zahl1 und Zahl3 bei jeder neuen Linie immer +50
            {
                graphics.DrawLine(pen, num1, 100, num3, 100);
                num1 += 50;
                num3 += 50;
            }
        }

        private void TransmitCorrectLetterToLabel()
        {
            if (textBox_SubmitLetter.Text.Length == 1)
            {
                label_FirstLetter.Visible = false;
                for (int i = 0; i < randomWord.Length; i++)
                {
                    if (randomWordToLower[i].ToString() == LetterInputToLower)
                    {
                        labels[i].Text = LetterInputToLower;
                    }
                }
                if (label0_Letter.Text.Length >= 1) //if the input was correct, invisible label0 gets a lowercase. label_Firstletter.visible gets true.(uppercase) 
                    label_FirstLetter.Visible = true;
            }
        }

        public void TransmitFirstLetterToFirstLabel()
        {
            label_FirstLetter.Text = randomWord[0].ToString(); //to uppercase the first letter
        }

        private void DisplayIncorrectLettersAndNumberOfFailedAttempts()
        {
            int num;
            if (randomWordToLower.All(element => !element.ToString().Equals(LetterInputToLower)) && !label_MissedLetters.Text.Contains(LetterInputToLower) && !int.TryParse(textBox_SubmitLetter.Text, out num))
            {
                missedCounter = missedCounter + 1;
                label_Missed.Text = "Missed: ";
                label_Missed.Text += Convert.ToInt32(missedCounter);
                label_MissedLetters.Text += LetterInputToLower + ", ";
                missedLetter = true;
                DrawHangmanPartAfterIncorrectLetter();
            }
        }

        #endregion

        #region Lists
        //private void SaveTextFileContentToList()
        //{
        //    try
        //    {
        //        words.Clear();
        //        using (StreamReader reader = new StreamReader(path))
        //        {
        //            string? line = reader.ReadLine();
        //            while (line != null)
        //            {
        //                words.Add(line);
        //                line = reader.ReadLine();
        //            }
        //        }
        //    }
        //    catch { label_Header.Text = "Textdatei konnte nicht geladen werden"; textFileFound = false; }
        //}

        private void WordAdder()
        {
            words.Clear();
            words.Add("Vogelspinne");
            words.Add("Elefant");
            words.Add("Gorilla");
            words.Add("Flusspferd");
            words.Add("Tiger");
            words.Add("Wolf");
            words.Add("Giraffe");
            words.Add("Taube");
            words.Add("Clownfisch");
            words.Add("Hase");
            words.Add("Pinguin");
            words.Add("Adler");
            words.Add("Krokodil");
            words.Add("Nashorn");
            words.Add("Hai");
            words.Add("Papagei");
            words.Add("Delphin");
            words.Add("Frosch");
            words.Add("Schnecke");
            words.Add("Eule");
            words.Add("Flamingo");
            words.Add("Wal");
        }


        private void LabelAdderForLetters()
        {
            labels.Clear();
            labels.Add(label0_Letter);
            labels.Add(label1_Letter);
            labels.Add(label2_Letter);
            labels.Add(label3_Letter);
            labels.Add(label4_Letter);
            labels.Add(label5_Letter);
            labels.Add(label6_Letter);
            labels.Add(label7_Letter);
            labels.Add(label8_Letter);
            labels.Add(label9_Letter);
            labels.Add(label10_Letter);
            labels.Add(label11_Letter);
            labels.Add(label12_Letter);
        }
        #endregion

        #region Clear Text Boxes
        private void RemoveContentFromTextboxWord()
        {
            textBox_SubmitWord.Clear();
        }

        private void RemoveContentFromTextboxLetter()
        {
            textBox_SubmitLetter.Clear();
        }
        #endregion

        #region Conversions to lowercase letters
        private void ConvertInputLetterToLowerCase()
        {
            LetterInputToLower = textBox_SubmitLetter.Text.ToLower();
        }

        private void ConvertInputWordToLowerCase()
        {
            randomWordToLower = randomWord.ToLower();
        }
        #endregion


        private void ResetAll()
        {
            Graphics graphics = panel_HangMan.CreateGraphics();
            graphics.Clear(panel_HangMan.BackColor);
            randomWord = "";
            label_Header.Text = "";
            label_Word.Text = "";
            label_WordLength.Text = "";
            label_Missed.Text = "Missed: ";
            label_MissedLetters.Text = "";
            missedCounter = 0;
            missedLetter = false;
            textFileFound = true;
            randomWordToLower = "";
            LetterInputToLower = "";

            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].Text = "";
            }
        }

        private void GenerateRandomWord()
        {
            Random random = new Random();
            int randomIndex = random.Next(0, words.Count);
            randomWord = words[randomIndex];
        }

    }
}

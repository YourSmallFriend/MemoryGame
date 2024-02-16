using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace memory_game

{
    public partial class Form1 : Form
    {
        // Lijst om de nummers bij te houden
        List<int> numbers = new List<int> { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6 };

        // Variabelen voor het bijhouden van de eerste en tweede keuze en het aantal pogingen
        string firstChoice;
        string secondChoice;
        int tries;

        // Lijst van PictureBoxes voor de afbeeldingen
        List<PictureBox> pictures = new List<PictureBox>();
        PictureBox picA;
        PictureBox picB;

        // Tijd gerelateerde variabelen
        int totalTime = 60;
        int countDownTime;

        // Boolean om bij te houden of het spel voorbij is
        bool gameOver = false;
        public Form1()
        {
            InitializeComponent();
            LoadPictures(); // Laad de afbeeldingen bij het starten van het formulier
        }

        private void RestartGameEvent(object sender, EventArgs e)
        {
            RestartGame(); // Herstart het spel wanneer de restart knop wordt ingedrukt
        }

        private void LoadPictures()
        {
            int leftPos = 20;
            int topPos = 20;
            int rows = 0;
            for (int i = 0; i < 12; i++)
            {
                PictureBox newPic = new PictureBox();
                newPic.Height = 50;
                newPic.Width = 50;
                newPic.BackColor = Color.Navy;
                newPic.SizeMode = PictureBoxSizeMode.StretchImage;
                newPic.Click += NewPic_Click;
                pictures.Add(newPic);
                if (rows < 3)
                {
                    rows++;
                    newPic.Left = leftPos;
                    newPic.Top = topPos;
                    this.Controls.Add(newPic);
                    leftPos = leftPos + 60;
                }
                if (rows == 3)
                {
                    leftPos = 20;
                    topPos += 60;
                    rows = 0;
                }
            }
            RestartGame(); // Start het spel door de afbeeldingen opnieuw in te stellen
        }

        private void NewPic_Click(object sender, EventArgs e)
        {
            if (gameOver) // Controleer of het spel voorbij is
            {
                return;
            }
            
            if (firstChoice == null)
            {
                picA = sender as PictureBox;
                if (picA.Tag != null && picA.Image == null)
                {
                    string imageName = (string)picA.Tag;
                    picA.Image = Properties.Resources.ResourceManager.GetObject(imageName) as Image;
                    firstChoice = (string)picA.Tag;
                }
            }
            else if (secondChoice == null)
            {
                picB = sender as PictureBox;

                if (picB.Tag != null && picB.Image == null)
                {
                    string imageName = (string)picB.Tag;
                    picB.Image = Properties.Resources.ResourceManager.GetObject(imageName) as Image;
                    secondChoice = (string)picB.Tag;
                    CheckPictures(picA, picB);
                }
            }
            //if (firstChoice == null) // Eerste klik
            //{
                //picA = sender as PictureBox;
                //if (picA.Tag != null && picA.Image == null) // Controleer of de afbeelding nog niet is geselecteerd
                //{
                   // picA.Image = Image.FromFile("pics/" + (string)picA.Tag + ".png");
                   // firstChoice = (string)picA.Tag;
               // }
           // }
            //else if (secondChoice == null) // Tweede klik
            //{
              //  picB = sender as PictureBox;
               // if (picB.Tag != null && picB.Image == null) // Controleer of de afbeelding nog niet is geselecteerd
                //{
                  //  picB.Image = Image.FromFile("pics/" + (string)picB.Tag + ".png");
                   // secondChoice = (string)picB.Tag;
                   // await Task.Delay(500); // Wacht 0,5 seconde voor feedback
                   // CheckPictures(picA, picB); // Controleer of de afbeeldingen overeenkomen
                //}
            //}
        }

        private void RestartGame()
        {
            // Een nieuwe willekeurige volgorde van de nummers genereren
            var randomList = numbers.OrderBy(x => Guid.NewGuid()).ToList();
            numbers = randomList;
            // Afbeeldingen herstellen naar beginpositie
            for (int i = 0; i < pictures.Count; i++)
            {
                pictures[i].Image = null;
                pictures[i].Tag = numbers[i].ToString();
            }
            tries = 0; // Reset het aantal pogingen
            lblStatus.Text = "fout geraden: " + tries + " keer."; // Update het label voor status
            lblTime.Text = "Tijd over: " + totalTime; // Update het label voor tijd
            gameOver = false; // Het spel is niet voorbij
            MessageBox.Show("je moet 2 kaarten met elkaar matchen, het spel eindigd niet voordat alle kaarten zijn omgedraaid.", "Memory");
            GameTimer.Start(); // Start de timer
            countDownTime = totalTime; // Reset de tijd
        }

        private async void CheckPictures(PictureBox A, PictureBox B)
        {
            if (firstChoice == secondChoice) // Controleer of de afbeeldingen overeenkomen
            {
                A.Tag = null; // Verwijder de tag van de eerste afbeelding
                B.Tag = null; // Verwijder de tag van de tweede afbeelding
                SoundPlayer player = new SoundPlayer();
                player.Stream = Properties.Resources.correct1;
                player.Play();
                await Task.Delay(750); // Wacht 0,5 seconde voor feedback verranderd naar 0.75.
            }
            else
            {
                tries++; // Verhoog het aantal pogingen
                lblStatus.Text = "fout" + tries + " keer."; // Update het label voor status
                SoundPlayer player = new SoundPlayer();
                player.Stream = Properties.Resources.wrong;
                player.Play();
                await Task.Delay(750); // Wacht 0,5 seconde voor feedback verranderd 
            }
            firstChoice = null; // Reset de eerste keuze
            secondChoice = null; // Reset de tweede keuze
            foreach (PictureBox pics in pictures.ToList()) // Verwijder de afbeeldingen die correct zijn geraden
            {
                if (pics.Tag != null)
                {
                    pics.Image = null;
                }
            }
            if (pictures.All(o => o.Tag == pictures[0].Tag)) // Controleer of alle afbeeldingen correct zijn geraden
            {
                GameTimer.Stop(); // Stop de timer
                gameOver = true; // Het spel is voorbij
                // Toon een bericht met de statistieken
                MessageBox.Show("Je hebt gewonnen!!\n" + "Je had " + tries + " fouten\n" + "Je had " + countDownTime + " seconden over\n" + "Klik op 'Restart' om opnieuw te starten", "Memory");
            }
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            Application.Restart(); // Herstart de applicatie
            Application.Exit(); // Sluit de oude applicatie af
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            countDownTime--; // Verminder de resterende tijd
            lblTime.Text = "Tijd over: " + countDownTime; // Update het label voor tijd
            if (countDownTime < 1) // Controleer of de tijd op is
            {
                GameTimer.Stop(); // Stop de timer
                gameOver = true; 
                // Toon een bericht dat de tijd om is
                MessageBox.Show("Tijd is om, je heb verloren\n" + "Klik op 'Restart' om opnieuw te starten", "Memory");
                // Toon alle afbeeldingen die nog niet zijn geraden
            }
        }
    }
}